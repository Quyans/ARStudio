namespace TriLib
{
    /// <summary>
    /// Enum used to define metadata types.
    /// </summary>
    public enum AssimpMetadataType
    {
        AI_BOOL = 0,
        AI_INT32 = 1,
        AI_UINT64 = 2,
        AI_FLOAT = 3,
        AI_DOUBLE = 4,
        AI_AISTRING = 5,
        AI_AIVECTOR3D = 6
    }

    /// <summary>
    /// Represents an Assimp metadata.
    /// </summary>
    public class AssimpMetadata
    {
        public AssimpMetadataType MetadataType;
        public uint MetadataIndex;
        public string MetadataKey;
        public object MetadataValue;

        public AssimpMetadata(AssimpMetadataType metadataType, uint metadataIndex, string metadataKey, object metadataValue)
        {
            MetadataType = metadataType;
            MetadataIndex = metadataIndex;
            MetadataKey = metadataKey;
            MetadataValue = metadataValue;
        }
    }
}