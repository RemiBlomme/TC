using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DebugMenu.Runtime
{
    public static class DebugMenuManager
    {
        #region Callbacks

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AfterAssembliesLoaded() => ProcessAssemblies();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad() => ProcessOwners();

        #endregion

        #region Main

        private static void ProcessAssemblies()
        {
            _isLoaded = false;
            _methodsInfos = new();

            var filteredAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => IsValidAssemblyName(x.FullName));

            var classTypes = filteredAssemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass);

            ProcessFields(classTypes);
            ProcessMethods(classTypes);
        }

        private static void ProcessOwners()
        {
            GetAllOwner();
            _isLoaded = true;
        }

        private static void ProcessFields(IEnumerable<Type> classTypes)
        {
            List<FieldInfo[]> fields = new();
            foreach (var classType in classTypes)
            {
                var type = classType.GetFields(_flags)
                    .Where(x => x.GetCustomAttributes(typeof(DebugMenuAttribute), false).FirstOrDefault() != null).ToArray();
                if (type.Length == 0) continue;
                fields.Add(type);
            }

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                for (int j = 0; j < field.Length; j++)
                {
                    var fieldInfo = field[j];

                    if (!fieldInfo.FieldType.IsEnum && !_allowedFieldTypes.Contains(fieldInfo.FieldType)) continue;

                    var attribute = fieldInfo.GetCustomAttribute<DebugMenuAttribute>();
                    DebugInfo debugInfo = new(fieldInfo);
                    _methodsInfos.Add(attribute.m_menuName, debugInfo);
                }
            }
        }

        private static void ProcessMethods(IEnumerable<Type> classTypes)
        {
            var methods = classTypes.SelectMany(x => x.GetMethods(_flags)).ToArray();

            for (int i = 0; i < methods.Length; i++)
            {
                var info = methods[i];
                var attributes = info.GetCustomAttributes(typeof(DebugMenuAttribute), false);
                if (attributes == null || attributes.Length == 0) continue;

                for (int j = 0; j < attributes.Length; j++)
                {
                    var attribute = attributes[j] as DebugMenuAttribute;
                    DebugInfo debugInfo = new(info);
                    _methodsInfos.Add(attribute.m_menuName, debugInfo);
                }
            }
        }

        private static void GetAllOwner()
        {
            var keys = _methodsInfos.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                if (_methodsInfos[key].IsStatic) continue;

                var type = _methodsInfos[key].GetDeclaringType;

                object owner = GameObject.FindAnyObjectByType(type, FindObjectsInactive.Include);
                if (owner == null)
                {
                    Debug.LogWarning($"[Debug Menu] Owner object not found for type : {type}");
                    _methodsInfos.Remove(key);
                    continue;
                }
                _methodsInfos[key].SetOwner(owner);
            }
        }

        #endregion

        #region Uitls

        private static bool IsValidAssemblyName(string assemblyName)
        {
            for (int i = 0; i < EXCLUDED_ASSEMBLIES.Length; i++)
            {
                if (assemblyName.StartsWith(EXCLUDED_ASSEMBLIES[i], StringComparison.InvariantCultureIgnoreCase)) return false;
            }
            return true;
        }

        #endregion

        #region Public

        public static Dictionary<string, DebugInfo> _methodsInfos;

        #endregion

        #region Private

        private static bool _isLoaded;
        private static string[] EXCLUDED_ASSEMBLIES = {
            "Unity", "System", "Mono", "netstandard", "Bee","RG","JetBrains","log4net","mscorlib", };
        private static BindingFlags _flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static Type[] _allowedFieldTypes = { typeof(bool) };

        #endregion
    }
}