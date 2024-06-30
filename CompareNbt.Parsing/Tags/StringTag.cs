using System;
using System.Globalization;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a single string. String is stored in UTF-8 encoding. </summary>
public sealed class StringTag : ValueTag<StringTag, string>
{
    /// <summary> Type of this tag (String). </summary>
    public override TagType Type
    {
        get { return TagType.String; }
    }

    /// <summary> Value/payload of this tag (a single string). May not be <c>null</c>. </summary>
    public override string Value
    {
        get { return stringVal; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            stringVal = value;
        }
    }

    string stringVal = "";


    /// <summary> Creates an unnamed NbtString tag with the default value (empty string). </summary>
    public StringTag() { }


    /// <summary> Creates an unnamed NbtString tag with the given value. </summary>
    /// <param name="value"> String value to assign to this tag. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    public StringTag(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }


    /// <summary> Creates a copy of given NbtString tag. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public StringTag(StringTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Value = other.Value;
    }


    #region Reading / Writing

    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.SkipString();
            return false;
        }
        Value = readStream.ReadString();
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        readStream.SkipString();
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.String);
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
        return new StringTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_String[");
        sb.Append(Value);
        sb.Append(']');
    }

    protected override bool EqualsInternal(StringTag other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
