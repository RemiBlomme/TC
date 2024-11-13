using Level.Data;
using Paps.UnityToolbarExtenderUIToolkit;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ExtentedToolBar.Editor
{
    [MainToolbarElement(id: "AwesomeDropdownField", ToolbarAlign.Left)]
    public class ToolBar : DropdownField
    {
        //private DropdownField _dropdownField;

        public void InitializeElement()
        {
            EditorApplication.projectChanged += () => { choices = GetAllLevelDataNames(); };
            
            //Add(_dropdownField = new()
            //{
            //    label = "Level selector",
            //    choices = GetAllLevelDataNames()
            //});

            label = "Level selector";
            choices = GetAllLevelDataNames();
        }

        private List<string> GetAllLevelDataNames()
        {
            List<string> names = new();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(LevelDataSO)}");
            for (int i = 0; i < guids.Length; i++)
            {
                names.Add(AssetDatabase.LoadAssetAtPath<LevelDataSO>(AssetDatabase.GUIDToAssetPath(guids[i])).name);
            }

            return names;
        }
    }
}