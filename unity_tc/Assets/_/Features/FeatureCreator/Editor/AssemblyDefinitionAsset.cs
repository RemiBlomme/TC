namespace FeatureCreator.Editor
{
    public class AssemblyDefinitionAsset
    {
        public string name;
        public string rootNamespace;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public string[] defineConstraints;
        public string[] versionDefines;
        public bool noEngineReferences;

        public AssemblyDefinitionAsset(string name, bool isEditorOnly = false)
        {
            this.name = name;
            rootNamespace = name;
            includePlatforms = isEditorOnly ? new[] { "Editor" } : null;
        }
    }
}