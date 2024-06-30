using System;
using System.Globalization;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a single signed 32-bit integer. </summary>
public sealed class IntTag : ValueTag<IntTag, int>
{
    /// <summary> Type of this tag (Int). </summary>
    public override TagType Type
    {
        get { return TagType.Int; }
    }

    /// <summary> Value/payload of this tag (a single signed 32-bit integer). </summary>
    public override int Value { get; set; }


    /// <summary> Creates an unnamed NbtInt tag with the default value of 0. </summary>
    public IntTag() { }


    /// <summary> Creates an unnamed NbtInt tag with the given value. </summary>
    /// <param name="value"> Value to assign to this tag. </param>
    public IntTag(int value)
    {
        Value = value;
    }


    /// <summary> Creates a copy of given NbtInt tag. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public IntTag(IntTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Value = other.Value;
    }


    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.ReadInt32();
            return false;
        }
        Value = readStream.ReadInt32();
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        readStream.ReadInt32();
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.Int);
        writeStream.Write(name);
        writeStream.Write(Value);
    }


    internal override void WriteData(TagWriter writeStream)
    {
        writeStream.Write(Value);
    }


    /// <inheritdoc />
    public override object Clone()
    {
        return new IntTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Int[");
        sb.Append(Value);
        sb.Append(']');
    }

    protected override bool EqualsInternal(IntTag other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
