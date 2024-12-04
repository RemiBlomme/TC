using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Level.Data
{
    [CreateAssetMenu(menuName = "SO/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] private AssetReference _lightingSceneAssetReference;
        [SerializeField] private AssetReference[] _sceneAssetReferences;
        [SerializeField] private SceneData[] _sceneData;


        public void AddScene(AssetReference assetReference)
        {
            _sceneAssetReferences = _sceneAssetReferences.Append(assetReference).ToArray();
        }
        public void RemoveScene(AssetReference assetReference)
        {
            AssetReference[] newSceneAssetReference = new AssetReference[_sceneAssetReferences.Length - 1];
            int newIndex = 0;

            foreach (var item in _sceneAssetReferences)
            {
                if (item == assetReference) continue;
                newSceneAssetReference[newIndex++] = item;
            }

            _sceneAssetReferences = newSceneAssetReference;
        }
        public AssetReference[] GetScenes() => _sceneAssetReferences;
        public AssetReference GetActiveScene() => _lightingSceneAssetReference;
        public SceneData[] GetSceneData() => _sceneData;
    }

    [Serializable]
    public struct SceneData
    {
        public int InputPriority;
        public AssetReference SceneAssetReference;
    }
}