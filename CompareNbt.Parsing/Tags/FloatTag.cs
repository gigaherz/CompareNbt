using System;
using System.Globalization;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a single-precision floating point number. </summary>
public sealed class FloatTag : ValueTag<FloatTag, float>
{
    /// <summary> Type of this tag (Float). </summary>
    public override TagType Type
    {
        get { return TagType.Float; }
    }

    /// <summary> Value/payload of this tag (a single-precision floating point number). </summary>
    public override float Value { get; set; }


    /// <summary> Creates an unnamed NbtFloat tag with the default value of 0f. </summary>
    public FloatTag() { }


    /// <summary> Creates an unnamed NbtFloat tag with the given value. </summary>
    /// <param name="value"> Value to assign to this tag. </param>
    public FloatTag(float value)
    {
        Value = value;
    }


    /// <summary> Creates a copy of given NbtFloat tag. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public FloatTag(FloatTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Value = other.Value;
    }


    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.ReadSingle();
            return false;
        }
        Value = readStream.ReadSingle();
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        readStream.ReadSingle();
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.Float);
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
        return new FloatTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Float[");
        sb.Append(Value);
        sb.Append(']');
    }

    protected override bool EqualsInternal(FloatTag other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
