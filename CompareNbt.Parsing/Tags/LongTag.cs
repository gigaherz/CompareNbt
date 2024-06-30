using System;
using System.Globalization;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a single signed 64-bit integer. </summary>
public sealed class LongTag : ValueTag<LongTag, long>
{
    /// <summary> Type of this tag (Long). </summary>
    public override TagType Type
    {
        get { return TagType.Long; }
    }

    /// <summary> Value/payload of this tag (a single signed 64-bit integer). </summary>
    public override long Value { get; set; }


    /// <summary> Creates an unnamed NbtLong tag with the default value of 0. </summary>
    public LongTag() { }


    /// <summary> Creates an unnamed NbtLong tag with the given value. </summary>
    /// <param name="value"> Value to assign to this tag. </param>
    public LongTag(long value)
    {
        Value = value;
    }


    /// <summary> Creates a copy of given NbtLong tag. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public LongTag(LongTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Value = other.Value;
    }


    #region Reading / Writing

    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.ReadInt64();
            return false;
        }
        Value = readStream.ReadInt64();
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        readStream.ReadInt64();
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.Long);
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
        return new LongTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Long[");
        sb.Append(Value);
        sb.Append(']');
    }

    protected override bool EqualsInternal(LongTag other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
