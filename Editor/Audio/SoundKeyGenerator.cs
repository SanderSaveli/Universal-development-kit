using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SanderSaveli.UDK.Audio.Editor;
using UnityEditor;
using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    [InitializeOnLoad]
    internal static class SoundKeyGenerator
    {
        internal const string OUTPUT_PATH = "Assets/Generated/Audio/Sounds.Generated.cs";
        internal static string Error => _lastError;

        internal static event Action ValidationChanged;

        private static bool _queued;
        private static string _lastError;
        private static readonly Dictionary<SoundCatalog, string> CATALOG_ERRORS = new Dictionary<SoundCatalog, string>();

        static SoundKeyGenerator()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            QueueValidation();
        }

        internal static void QueueValidation()
        {
            if (_queued || BuildPipeline.isBuildingPlayer)
                return;

            _queued = true;
            EditorApplication.delayCall += ValidateDelayed;
        }

        internal static bool Generate()
        {
            try
            {
                if (!TryGetSource(out string source, out string error))
                {
                    ReportError(error);
                    return false;
                }

                if (File.Exists(OUTPUT_PATH) && File.ReadAllText(OUTPUT_PATH).Replace("\r\n", "\n") == source)
                {
                    Validate();
                    return true;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(OUTPUT_PATH));
                File.WriteAllText(OUTPUT_PATH, source, new UTF8Encoding(false));
                AssetDatabase.ImportAsset(OUTPUT_PATH);
                Validate();
                return true;
            }
            catch (Exception exception)
            {
                ReportError("Could not generate sound keys: " + exception.Message);
                return false;
            }
        }

        internal static bool IsCurrent(out string error)
        {
            if (!TryGetSource(out string source, out error))
                return false;

            if (!File.Exists(OUTPUT_PATH) || File.ReadAllText(OUTPUT_PATH).Replace("\r\n", "\n") != source)
            {
                error = "Sound keys are missing or outdated. Use Generate IDs in any SoundConfig inspector, "
                    + "wait for script compilation, then build again.";
                return false;
            }

            return true;
        }

        internal static string GetCatalogError(SoundCatalog catalog)
        {
            return CATALOG_ERRORS.TryGetValue(catalog, out string error) ? error : null;
        }

        internal static bool ValidateCatalogs(out string error)
        {
            Validate();
            StringBuilder errors = new StringBuilder();
            if (!string.IsNullOrEmpty(_lastError))
                errors.AppendLine(_lastError);

            foreach (KeyValuePair<SoundCatalog, string> entry in CATALOG_ERRORS)
            {
                if (!string.IsNullOrEmpty(entry.Value))
                    errors.AppendLine(AssetDatabase.GetAssetPath(entry.Key) + ":\n" + entry.Value);
            }

            error = errors.ToString().TrimEnd();
            return error.Length == 0;
        }

        internal static void Validate()
        {
            _lastError = IsCurrent(out string currentError) ? null : currentError;
            CATALOG_ERRORS.Clear();
            if (!TryReadGeneratedIds(out List<string> ids, out string readError))
            {
                _lastError = readError;
                ValidationChanged?.Invoke();
                return;
            }

            string[] catalogGuids = AssetDatabase.FindAssets("t:SoundCatalog");
            for (int i = 0; i < catalogGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(catalogGuids[i]);
                SoundCatalog catalog = AssetDatabase.LoadAssetAtPath<SoundCatalog>(path);
                if (catalog == null)
                    continue;

                List<string> catalogIds = new List<string>();
                if (catalog.Sounds != null)
                {
                    for (int j = 0; j < catalog.Sounds.Count; j++)
                        catalogIds.Add(catalog.Sounds[j] == null ? null : catalog.Sounds[j].Id ?? string.Empty);
                }

                SoundCatalogValidation.TryValidate(ids, catalogIds, out string catalogError);
                CATALOG_ERRORS.Add(catalog, catalogError);
            }

            ValidationChanged?.Invoke();
        }

        private static bool TryReadGeneratedIds(out List<string> ids, out string error)
        {
            ids = new List<string>();
            if (!File.Exists(OUTPUT_PATH))
            {
                error = "Sound keys have not been generated. Use Generate IDs in a SoundConfig inspector.";
                return false;
            }

            string source = File.ReadAllText(OUTPUT_PATH);
            MatchCollection matches = Regex.Matches(source,
                "new global::SanderSaveli\\.UDK\\.Audio\\.SoundId\\(\"([^\"]*)\"\\)");
            for (int i = 0; i < matches.Count; i++)
                ids.Add(matches[i].Groups[1].Value);

            if (!SoundKeySourceBuilder.TryBuild(ids, out string expected, out error))
                return false;

            if (source.Replace("\r\n", "\n") != expected)
            {
                error = "Generated sound keys have an unexpected format. Use Generate IDs in a SoundConfig inspector.";
                return false;
            }

            return true;
        }

        private static void ValidateDelayed()
        {
            _queued = false;
            if (BuildPipeline.isBuildingPlayer || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                QueueValidation();
                return;
            }

            Validate();
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                QueueValidation();
        }

        private static bool TryGetSource(out string source, out string error)
        {
            source = null;
            List<string> ids = new List<string>();
            HashSet<string> uniqueIds = new HashSet<string>(StringComparer.Ordinal);
            string[] soundGuids = AssetDatabase.FindAssets("t:SoundConfig");
            Dictionary<string, string> pathsById = new Dictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < soundGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(soundGuids[i]);
                SoundConfig config = AssetDatabase.LoadAssetAtPath<SoundConfig>(path);
                if (config == null || string.IsNullOrWhiteSpace(config.Id))
                {
                    error = path + ": SoundConfig has an empty sound ID.";
                    return false;
                }

                if (!uniqueIds.Add(config.Id))
                    continue;

                ids.Add(config.Id);
                pathsById.Add(config.Id, path);
            }

            if (SoundKeySourceBuilder.TryBuild(ids, out source, out error))
                return true;

            StringBuilder details = new StringBuilder(error);
            foreach (KeyValuePair<string, string> entry in pathsById)
                details.Append("\n").Append(entry.Key).Append(": ").Append(entry.Value);

            error = details.ToString();
            return false;
        }

        private static void ReportError(string error)
        {
            if (_lastError == error)
                return;

            _lastError = error;
            Debug.LogError(error);
        }
    }
}
