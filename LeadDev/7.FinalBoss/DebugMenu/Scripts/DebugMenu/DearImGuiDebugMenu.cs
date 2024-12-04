using DebugMenu.Runtime;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DearImGuiDebugMenu : MonoBehaviour
{
    TreeRoot<string, DebugInfo> debugTree;
    void OnEnable()
    {
        ImGuiUn.Layout += OnLayout;
        Dictionary<string, DebugInfo> methodsInfos = DebugMenuManager._methodsInfos;
        debugTree = new TreeRoot<string, DebugInfo>("Base");
        Debug.Log(debugTree.ToString());
        PopulateTree(debugTree, methodsInfos, '/');
    }

    void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
    }

    void OnLayout()
    {
        ImGui.PushID("Debug");
        ImGui.BeginChild("Scrolling");

        BuildImGuiTreeRecursive(debugTree);

        ImGui.EndChild();
        ImGui.End();
        ImGui.OpenPopup("Debug");
    }

    private void BuildImGuiTreeRecursive(TreeNode<string, DebugInfo> debugTreeNode)
    {
        if (ImGui.TreeNodeEx(debugTreeNode.key))
        {

            foreach (TreeLeaf<string, DebugInfo> leaf in debugTreeNode.Leaves)
            {
                if (ImGui.Button(leaf.key))
                {
                    leaf.value.Invoke();
                }
                if (!leaf.value.IsClass)
                {
                    ImGui.SameLine(150, 20);
                    if (leaf.value.IsEnum)
                    {
                        Type type = leaf.value.m_currentFieldValue.GetType();

                        Enum.TryParse(type, leaf.value.m_currentFieldValue.ToString(), out object result);
                        string[] allValues = Enum.GetNames(type);

                        int index = 0;
                        for (int i = 0; i < allValues.Length; i++)
                        {
                            if (result.ToString() == allValues[i])
                            {
                                index = i;
                                break;
                            }
                        }

                        if (ImGui.BeginCombo("##combo", allValues[index])) // The second parameter is the label previewed before opening the combo.
                        {
                            for (int n = 0; n < allValues.Length; n++)
                            {
                                bool is_selected = (result.ToString() == allValues[n]); // You can store your selection however you want, outside or inside your objects
                                if (ImGui.Selectable(allValues[n], is_selected))
                                {
                                    leaf.value.m_currentFieldValue = allValues[n];
                                }

                                if (is_selected)
                                {
                                    ImGui.SetItemDefaultFocus();   // You may set the initial focus when opening the combo (scrolling + for keyboard navigation support)
                                }
                            }
                            ImGui.EndCombo();
                        }
                    }
                    else
                    {
                        var value = leaf.value.m_currentFieldValue as bool?;
                        var color = value.Value ? Color.green : Color.red;
                        ImGui.TextColored(color, value.ToString());

                        //if (leaf.value.m_fieldInfo.FieldType is bool)
                        //{
                        //bool fieldValue = (bool)leaf.value.m_currentFieldValue;
                        //bool result = ImGui.Checkbox(leaf.value.m_currentFieldValue.ToString(), ref fieldValue);
                        //if (result != fieldValue)
                        //{
                        //    leaf.value.m_currentFieldValue = result;
                        //}
                        //}
                    }

                }
            }
            foreach (string key in debugTreeNode.Children.Keys)
            {

                BuildImGuiTreeRecursive(debugTreeNode.Children[key]);

            }
            ImGui.TreePop();
        }
    }

    private static void PopulateTree(TreeRoot<string, DebugInfo> root, Dictionary<string, DebugInfo> items, char pathSeparator)
    {

        foreach (string path in items.Keys)
        {
            string[] paths = path.Split(pathSeparator);
            DebugInfo debugInfo = items[path];
            PopulateTreeRecursive(root, paths, 0, debugInfo);
        }
    }


    private static void PopulateTreeRecursive(TreeNode<string, DebugInfo> node, string[] path, int pathLevel, DebugInfo item)
    {
        if (path.Count() == 0)
        {
            node.Leaves.Add(new TreeLeaf<string, DebugInfo>("orphan", item));
        }
        else if (pathLevel < path.Count())
        {
            if (pathLevel == path.Count() - 1)
            {
                node.Leaves.Add(new TreeLeaf<string, DebugInfo>(path.Last(), item));
            }
            else
            {
                TreeNode<string, DebugInfo> child;
                if (node.Children.TryGetValue(path[pathLevel], out child))
                {
                    PopulateTreeRecursive(child, path, pathLevel + 1, item);
                }
                else
                {
                    child = new TreeNode<string, DebugInfo>(path[pathLevel]);
                    node.Children.Add(path[pathLevel], child);
                    PopulateTreeRecursive(child, path, pathLevel + 1, item);
                }
            }
        }
    }


    private class TreeRoot<TKey, TLeaves> : TreeNode<TKey, TLeaves>
    {
        public TreeRoot(TKey key) : base(key)
        {
        }
    }
    private class TreeNode<TKey, TLeaves>
    {
        public Dictionary<TKey, TreeNode<TKey, TLeaves>> Children = new();
        public List<TreeLeaf<TKey, TLeaves>> Leaves = new();
        public TKey key;

        public TreeNode(TKey key)
        {
            this.key = key;
        }
    }

    public class TreeLeaf<TKey, TLeaves>
    {
        public TLeaves value;
        public TKey key;

        public TreeLeaf(TKey key, TLeaves value)
        {
            this.value = value;
            this.key = key;
        }
    }
}