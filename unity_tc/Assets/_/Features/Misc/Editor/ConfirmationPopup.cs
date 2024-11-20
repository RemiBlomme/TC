using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Misc.Editor.CustomUI;

namespace Misc.Editor
{
    public class ConfirmationPopup : EditorWindow
    {
        private const int WIDTH = 140;
        private const int HEIGHT = 80;

        public event Action<bool> Confirmed;


        public static ConfirmationPopup ShowConfirmPopup()
        {
            var window = CreateInstance<ConfirmationPopup>();
            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            screenPoint.x -= WIDTH * .5f;

            window.position = new Rect(screenPoint.x, screenPoint.y, WIDTH, HEIGHT);
            window.ShowPopup();

            return window;
        }

        private void CreateGUI()
        {
            VisualElement body;

            rootVisualElement.Add(body = new VisualElementBuilder()
                .Add(CreateButton("Cancel", OnCancel))
                .Add(CreateButton("Confirm", OnConfirm))
                .Build());

            body.style.flexDirection = FlexDirection.Row;
            rootVisualElement.style.justifyContent = Justify.SpaceAround;
            rootVisualElement.style.alignItems = Align.Center;
        }

        private void OnLostFocus() => OnCancel();

        private void OnCancel()
        {
            Confirmed?.Invoke(false);
            Close();
        }

        private void OnConfirm()
        {
            Confirmed?.Invoke(true);
            Close();
        }
    }
}