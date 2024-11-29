using System;
using System.Reflection;
using UnityEngine;

namespace DebugMenu.Runtime
{
    public class DebugInfo
    {
        #region Public Fields

        public readonly MethodInfo m_methodInfo;
        public readonly FieldInfo m_fieldInfo;
        public object m_currentFieldValue;

        public bool IsClass { get { return m_methodInfo != null; } }
        public bool IsStatic { get { return IsClass ? m_methodInfo.IsStatic : m_fieldInfo.IsStatic; } }
        public bool IsEnum { get { return m_fieldInfo.FieldType.IsEnum; } }
        public Type GetDeclaringType { get { return IsClass ? m_methodInfo.DeclaringType : m_fieldInfo.DeclaringType; } }

        #endregion

        #region Ctor

        public DebugInfo(MethodInfo info)
        {
            m_methodInfo = info;
        }

        public DebugInfo(FieldInfo info)
        {
            m_fieldInfo = info;
        }

        #endregion

        #region Public API

        public void Invoke()
        {
            if (IsClass) InvokeMethod();
            else UpdateField();
        }

        public void SetOwner(object owner)
        {
            _owner = owner;
            if (IsClass || IsStatic) return;
            m_currentFieldValue = m_fieldInfo.GetValue(owner);
        }

        #endregion

        #region Main

        private void InvokeMethod()
        {
            if (!IsValid()) return;

            object owner = IsStatic ? null : _owner;
            m_methodInfo.Invoke(owner, new object[] { });
        }

        private void UpdateField()
        {
            if (!IsValid()) return;

            object owner = IsStatic ? null : _owner;
            object currentValue = m_fieldInfo.GetValue(owner);
            var type = currentValue.GetType();
            object newValue = null;

            if (m_fieldInfo.FieldType.IsEnum)
            {
                Enum.TryParse(type, currentValue.ToString(), out object result);
                var allValues = Enum.GetNames(type);

                int index = 0;
                for (int i = 0; i < allValues.Length; i++)
                {
                    if (result.ToString() == allValues[i])
                    {
                        index = i;
                        break;
                    }
                }
                index++;
                index %= allValues.Length;

                newValue = Enum.Parse(type, allValues[index]);
                m_fieldInfo.SetValue(owner, newValue);
            }
            else if (type == typeof(bool))
            {
                newValue = !(currentValue as bool?).Value;
                m_fieldInfo.SetValue(owner, newValue);
            }
            else
            {
                Debug.LogError($"[Debug Menu] Unsuported Type : {type}");
                return;
            }

            m_currentFieldValue = newValue;
        }

        #endregion

        #region Uitls

        private bool IsValid()
        {
            bool isStatic = IsStatic;
            if (_owner == null && !isStatic) return false;
            return true;
        }

        #endregion

        #region Private And Protected

        private object _owner;

        #endregion
    }
}