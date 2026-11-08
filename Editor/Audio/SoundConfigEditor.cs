using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SanderSaveli.UDK.Audio.Editor
{
    [CustomEditor(typeof(SoundConfig))]
    [CanEditMultipleObjects]
    internal sealed class SoundConfigEditor : UnityEditor.Editor
    {
        private const float PREVIEW_PADDING = 8f;
        private const float BUTTON_HEIGHT = 30f;
        private const float BUTTON_SPACING = 8f;
        private static readonly Color PLAY_COLOR = new Color(0.2f, 0.6f, 0.2f);
        private static readonly Color STOP_COLOR = new Color(0.75f, 0.2f, 0.2f);

        private VisualElement _inspectorRoot;
        private VisualElement _footer;
        private IMGUIContainer _fields;
        private Button _playButton;
        private Button _stopButton;
        private Label _description;

        private void OnEnable()
        {
            SoundKeyGenerator.ValidationChanged += OnValidationChanged;
            SoundKeyGenerator.QueueValidation();
        }

        private void OnDisable()
        {
            SoundKeyGenerator.ValidationChanged -= OnValidationChanged;
            _footer?.RemoveFromHierarchy();
            SoundConfigPreview.StopIfOwned((SoundConfig)target);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _footer?.RemoveFromHierarchy();
            VisualElement root = new VisualElement();
            _inspectorRoot = root;
            _fields = new IMGUIContainer(DrawFields);
            root.Add(_fields);
            _footer = new VisualElement();
            _footer.style.flexShrink = 0f;
            _footer.style.paddingLeft = PREVIEW_PADDING;
            _footer.style.paddingRight = PREVIEW_PADDING;
            _footer.style.paddingTop = PREVIEW_PADDING;
            _footer.style.paddingBottom = PREVIEW_PADDING;
            _description = new Label();
            _description.style.whiteSpace = WhiteSpace.Normal;
            _footer.Add(_description);
            VisualElement buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            _playButton = CreateButton("Play", PLAY_COLOR, Play);
            _playButton.style.marginRight = BUTTON_SPACING;
            _stopButton = CreateButton("Stop", STOP_COLOR, Stop);
            buttons.Add(_playButton);
            buttons.Add(_stopButton);
            _footer.Add(buttons);
            root.Add(_footer);
            root.RegisterCallback<AttachToPanelEvent>(evt => root.schedule.Execute(() => PinFooter(root)));
            root.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                if (_inspectorRoot != root)
                    return;

                _footer.RemoveFromHierarchy();
                SoundConfigPreview.StopIfOwned((SoundConfig)target);
            });
            root.schedule.Execute(RefreshFooter).Every(100);
            RefreshFooter();
            return root;
        }

        public override void OnInspectorGUI()
        {
            DrawFields();
            Color previousColor = GUI.backgroundColor;
            try
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.backgroundColor = PLAY_COLOR;
                    using (new EditorGUI.DisabledScope(!CanPlay()))
                    {
                        if (GUILayout.Button("Play", GUILayout.Height(BUTTON_HEIGHT)))
                            Play();
                    }

                    GUILayout.Space(BUTTON_SPACING);
                    GUI.backgroundColor = STOP_COLOR;
                    using (new EditorGUI.DisabledScope(!CanStop()))
                    {
                        if (GUILayout.Button("Stop", GUILayout.Height(BUTTON_HEIGHT)))
                            Stop();
                    }
                }
            }
            finally
            {
                GUI.backgroundColor = previousColor;
            }
        }

        private void DrawFields()
        {
            if (DrawDefaultInspector())
                SoundKeyGenerator.QueueValidation();

            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                if (GUILayout.Button("Generate IDs"))
                {
                    for (int i = 0; i < targets.Length; i++)
                        AssetDatabase.SaveAssetIfDirty(targets[i]);

                    SoundKeyGenerator.Generate();
                }
            }

            EditorGUILayout.HelpBox("Generate the shared keys from all SoundConfig assets. "
                + "Configs in different themes may share an ID such as UI/Click.\n"
                + "Output: " + SoundKeyGenerator.OUTPUT_PATH, MessageType.Info);
            if (!string.IsNullOrEmpty(SoundKeyGenerator.Error))
                EditorGUILayout.HelpBox(SoundKeyGenerator.Error, MessageType.Error);
        }

        private void PinFooter(VisualElement root)
        {
            if (_inspectorRoot != root || root.panel == null)
                return;

            if (_footer.parent == null)
                root.Add(_footer);

            ScrollView inspectorScroll = root.GetFirstAncestorOfType<ScrollView>();
            if (inspectorScroll?.parent != null)
                inspectorScroll.parent.Add(_footer);
        }

        private Button CreateButton(string text, Color color, System.Action action)
        {
            Button button = new Button(action) { text = text };
            button.style.height = BUTTON_HEIGHT;
            button.style.flexGrow = 1f;
            button.style.flexBasis = 0f;
            button.style.backgroundColor = color;
            button.style.color = Color.white;
            return button;
        }

        private void RefreshFooter()
        {
            _playButton.SetEnabled(CanPlay());
            _stopButton.SetEnabled(CanStop());
            bool active = SoundConfigPreview.IsActive && SoundConfigPreview.Config == target;
            _description.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
            if (active)
                _description.text = SoundConfigPreview.Description;
        }

        private bool CanPlay()
        {
            return targets.Length == 1 && !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private bool CanStop()
        {
            return SoundConfigPreview.IsActive && SoundConfigPreview.Config == target;
        }

        private void Play()
        {
            SoundConfigPreview.Play((SoundConfig)target);
        }

        private void Stop()
        {
            SoundConfigPreview.Stop((SoundConfig)target);
        }

        private void OnValidationChanged()
        {
            Repaint();
            _fields?.MarkDirtyRepaint();
        }
    }
}
