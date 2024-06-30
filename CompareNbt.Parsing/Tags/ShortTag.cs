using System;
using System.Globalization;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a single signed 16-bit integer. </summary>
public sealed class ShortTag : Tag<ShortTag>
{
    /// <summary> Type of this tag (Short). </summary>
    public override TagType Type
    {
        get { return TagType.Short; }
    }

    /// <summary> Value/payload of this tag (a single signed 16-bit integer). </summary>
    public short Value { get; set; }


    /// <summary> Creates an unnamed NbtShort tag with the default value of 0. </summary>
    public ShortTag() { }


    /// <summary> Creates an unnamed NbtShort tag with the given value. </summary>
    /// <param name="value"> Value to assign to this tag. </param>
    public ShortTag(short value)
    {
        Value = value;
    }


    /// <summary> Creates a copy of given NbtShort tag. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public ShortTag(ShortTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Value = other.Value;
    }


    #region Reading / Writing

    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.ReadInt16();
            return false;
        }
        Value = readStream.ReadInt16();
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        readStream.ReadInt16();
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.Short);
        writeStream.Write(name);
        writeStream.Write(Value);
    }


    internal override void WriteData(TagWriter writeStream)
    {
        writeStream.Write(Value);
    }

    #endregion


    /// <inheritdoc />
    public override object Clone()
    {
        return new ShortTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Short[");
        sb.Append(Value);
        sb.Append(']');
    }

    protected override bool EqualsInternal(ShortTag other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
