using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImGUI.Runtime
{
    public class ImGUIDisplayer : MonoBehaviour
    {
        [SerializeField] private GameObject _gui;
        [SerializeField] private KeyCode _key = KeyCode.F1;

        void Update()
        {
            if (Input.GetKeyDown(_key)) _gui.SetActive(!_gui.activeInHierarchy);
        }
    }
}