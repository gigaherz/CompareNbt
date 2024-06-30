namespace CompareNbt.Parsing;

// Represents state of a node in the NBT file tree, used by NbtReader
internal sealed class NbtReaderNode
{
    public string? ParentName;
    public TagType ParentTagType;
    public TagType ListType;
    public int ParentTagLength;
    public int ListIndex;
}
