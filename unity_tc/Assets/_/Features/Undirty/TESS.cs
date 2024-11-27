using System.Collections.Generic;
using UnDirty;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Undirty
{
    public class TESS : UBehaviour
    {
        [SerializeField] private AssetReference _prefabToSpawn;
        private List<GameObject> _cubeList = new();

        private void Start()
        {
            Spawn(_prefabToSpawn, OnSpawn, 5);    // 1/5
            VerboseLog("[TEST 1]: Spawned 1");

            for (int i = 0; i < 3; i++)
            {
                Spawn(_prefabToSpawn, OnSpawn, 2);    // 4/5
            }
            VerboseLog("[TEST 2]: Spawned 3");

            Spawn(_prefabToSpawn, OnSpawn, 5);    // 5/5
            VerboseLog("[TEST 3]: Spawned 1");

            Spawn(_prefabToSpawn, OnSpawn, 6);    // 6/6
            VerboseLog("[TEST 4]: Spawned 1");

            Spawn(_prefabToSpawn, OnSpawn, 6);    // 7/7!!!
            VerboseLog("[TEST 5]: Spawned 1");

        }

        private void OnSpawn(Object obj)
        {
            var go = obj as GameObject;
            _cubeList.Add(go);

            if (_cubeList.Count == 7)
            {
                Release(go);    // 6/7
                VerboseLog("[TEST 6]: Release");

                Spawn(_prefabToSpawn, OnSecondLastSpawn);    // 7/7
                VerboseLog("[TEST 7]: Spawned 1");

                Spawn(_prefabToSpawn);    // 8/8!!!
                VerboseLog("[TEST 8]: Spawned 1");
            }
        }

        private void OnSecondLastSpawn(Object obj)
        {
            var go = obj as GameObject;
            Release(go);    // 7/8
            VerboseLog("[TEST 9]: Release again");

            Spawn(_prefabToSpawn);    // 8/8
            VerboseLog("[TEST 10]: Spawned 1");

            Spawn(_prefabToSpawn);    // 9/9!!!
            VerboseLog("[TEST 11]: Spawned 1");
        }
    }
}