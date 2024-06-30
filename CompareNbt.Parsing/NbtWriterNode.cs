namespace CompareNbt.Parsing;

// Represents state of a node in the NBT file tree, used by NbtWriter
internal sealed class NbtWriterNode
{
    public TagType ParentType;
    public TagType ListType;
    public int ListSize;
    public int ListIndex;
}
