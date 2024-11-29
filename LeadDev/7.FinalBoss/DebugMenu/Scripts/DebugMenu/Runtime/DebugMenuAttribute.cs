using System;

namespace DebugMenu.Runtime
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Enum)]
    public class DebugMenuAttribute : System.Attribute
    {
        /// <param name="menuName"> Full path for the menu (ex : MyMenu/MySubMenu/MyName</param>
        public DebugMenuAttribute(string menuName) => m_menuName = menuName;

        public string m_menuName;
    }
}