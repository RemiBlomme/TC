using System.Threading.Tasks;
using UnDirty;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Undirty
{
    public class TESS : UBehavior
    {
        [SerializeField] private AssetReference _prefabToSpawn;


        private void Start()
        {
            //Spawn(_prefabToSpawn, Vector3.one, Quaternion.identity, 5, OnSpawn);

            for (int i = 0; i < 3; i++)
            {
                Spawn(_prefabToSpawn, Vector3.one, Quaternion.identity, 1, OnSpawn);
            }
        }

        private void OnSpawn(Object obj)
        {
            VerboseLog("ZE fini");
        }
    }
}