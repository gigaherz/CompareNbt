using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a double-precision floating point number. </summary>
public sealed class DoubleTag : ValueTag<DoubleTag, double>
{
    /// <summary> Type of this tag (Double). </summary>
    public override TagType Type
    {
        get { return TagType.Double; }
    }

    /// <summary> Value/payload of this tag (a double-precision floating point number). </summary>
    public override double Value { get; set; }


    /// <summary> Creates an unnamed NbtDouble tag with the default value of 0. </summary>
    public DoubleTag() { }


    /// <summary> Creates an unnamed NbtDouble tag with the given value. </summary>
    /// <param name="value"> Value to assign to this tag. </param>
    public DoubleTag(double value)
    {
        Value = value;
    }


    /// <summary> Creates a copy of given NbtDouble tag. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public DoubleTag(DoubleTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Value = other.Value;
    }


    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.ReadDouble();
            return false;
        }
        Value = readStream.ReadDouble();
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        readStream.ReadDouble();
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.Double);
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
        return new DoubleTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Double[");
        sb.Append(Value);
        sb.Append(']');
    }

    protected override bool EqualsInternal(DoubleTag other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
