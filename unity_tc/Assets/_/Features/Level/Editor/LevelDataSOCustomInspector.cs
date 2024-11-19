using UnityEngine.UIElements;
using UnityEditor;
using Misc.Editor;
using Level.Data;

using static Misc.Editor.CustomUI;


namespace Level.Editor
{
    [CustomEditor(typeof(LevelDataSO))]
    public class LevelDataSOCustomInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root;
            VisualElement header;
            Button editButton;

            root = new VisualElementBuilder()
                .Add(header = new VisualElementBuilder()
                    .Add(editButton = CreateButton("Edit").AddListener(() => WindowSceneManager.ShowWindow(target as LevelDataSO)))
                    .Build())
                .Add(CreateSpace(minHeight: 10))
                .Build()
                .AddDefaultInspector(this);

            ApplyHeaderStyle(header.style);
            ApplyButtonStyle(editButton.style);

            return root;
        }

        private void ApplyButtonStyle(IStyle style)
        {
            style.width = Length.Percent(100);
            style.height = 30;
        }

        private void ApplyHeaderStyle(IStyle style)
        {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.Center;
        }
    }
}