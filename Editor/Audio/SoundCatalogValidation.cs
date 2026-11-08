using System;
using System.Collections.Generic;
using System.Text;

namespace SanderSaveli.UDK.Audio.Editor
{
    internal static class SoundCatalogValidation
    {
        internal static bool TryValidate(IReadOnlyList<string> generatedIds, IReadOnlyList<string> catalogIds,
            out string error)
        {
            HashSet<string> expected = new HashSet<string>(generatedIds, StringComparer.Ordinal);
            Dictionary<string, int> firstIndices = new Dictionary<string, int>(StringComparer.Ordinal);
            StringBuilder errors = new StringBuilder();
            for (int i = 0; i < catalogIds.Count; i++)
            {
                string id = catalogIds[i];
                if (string.IsNullOrWhiteSpace(id))
                {
                    errors.AppendLine("Entry " + i + (id == null
                        ? " has no SoundConfig reference." : " has an empty sound ID."));
                    continue;
                }

                if (firstIndices.TryGetValue(id, out int previousIndex))
                    errors.AppendLine("Duplicate ID '" + id + "' at entries " + previousIndex + " and " + i + ".");
                else
                    firstIndices.Add(id, i);

                if (!expected.Contains(id))
                    errors.AppendLine("Entry " + i + ": ID '" + id + "' has no generated key. Check spelling and case, then Generate IDs.");
            }

            for (int i = 0; i < generatedIds.Count; i++)
            {
                if (!firstIndices.ContainsKey(generatedIds[i]))
                    errors.AppendLine("Missing ID '" + generatedIds[i] + "'. Add exactly one matching SoundConfig.");
            }

            error = errors.ToString().TrimEnd();
            return error.Length == 0;
        }
    }
}
