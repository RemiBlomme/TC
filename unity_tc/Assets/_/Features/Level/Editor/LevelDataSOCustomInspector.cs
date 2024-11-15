using Level.Data;
using Misc.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace Level.Editor
{
    [CustomEditor(typeof(LevelDataSO))]
    public class LevelDataSOCustomInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            Button button1;
            Button button2;

            var topButtons = new VisualElementBuilder()
                .Add(button1 = CustomUI.CreateButton("Add scene").AddListener(() => WindowSceneCreator.ShowWindow(target as LevelDataSO)))
                .Add(button2 = CustomUI.CreateButton("Remove scene").AddListener(() => WindowSceneRemove.ShowWindow(target as LevelDataSO)))
                .Build();
            topButtons.style.flexDirection = FlexDirection.Row;
            topButtons.style.justifyContent = Justify.Center;
            button1.style.width = Length.Percent(50);
            button2.style.width = Length.Percent(50);

            var root = new VisualElementBuilder()
                .Add(topButtons)
                .Build();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            return root;
        }
    }
}