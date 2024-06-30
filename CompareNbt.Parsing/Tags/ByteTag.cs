using System;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a single byte. </summary>
public sealed class ByteTag : ValueTag<ByteTag, byte>
{
    /// <summary> Type of this tag (Byte). </summary>
    public override TagType Type
    {
        get { return TagType.Byte; }
    }

    /// <summary> Value/payload of this tag (a single byte). </summary>
    public override byte Value { get; set; }


    /// <summary> Creates an unnamed NbtByte tag with the default value of 0. </summary>
    public ByteTag() { }


    /// <summary> Creates an unnamed NbtByte tag with the given value. </summary>
    /// <param name="value"> Value to assign to this tag. </param>
    public ByteTag(byte value)
    {
        Value = value;
    }


    /// <summary> Creates a copy of given NbtByte tag. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public ByteTag(ByteTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Value = other.Value;
    }


    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.ReadByte();
            return false;
        }
        Value = readStream.ReadByte();
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        readStream.ReadByte();
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.Byte);
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
        return new ByteTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Byte[");
        sb.Append(Value);
        sb.Append(']');
    }

    protected override bool EqualsInternal(ByteTag other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
