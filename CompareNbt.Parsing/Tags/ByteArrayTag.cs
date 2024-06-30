using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing an array of bytes. </summary>
public sealed class ByteArrayTag : ArrayTag<ByteArrayTag, byte>
{
    /// <summary> Type of this tag (ByteArray). </summary>
    public override TagType Type => TagType.ByteArray;

    /// <summary> Value/payload of this tag (an array of bytes). Value is stored as-is and is NOT cloned. May not be <c>null</c>. </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    public override byte[] Value
    {
        get { return elements; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            elements = value;
        }
    }

    byte[] elements;


    /// <summary> Creates an unnamed NbtByte tag, containing an empty array of bytes. </summary>
    public ByteArrayTag()
        : this(Array.Empty<byte>())
    { 
    }


    /// <summary> Creates an unnamed NbtByte tag, containing the given array of bytes. </summary>
    /// <param name="value"> Byte array to assign to this tag's Value. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    /// <remarks> Given byte array will be cloned. To avoid unnecessary copying, call one of the other constructor
    /// overloads (that do not take a byte[]) and then set the Value property yourself. </remarks>
    public ByteArrayTag(byte[] value)
    {
        elements = value;
    }

    /// <summary> Creates a deep copy of given NbtByteArray. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    /// <remarks> Byte array of given tag will be cloned. </remarks>
    public ByteArrayTag(ByteArrayTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        elements = (byte[])other.Value.Clone();
    }


    /// <summary> Gets or sets a byte at the given index. </summary>
    /// <param name="tagIndex"> The zero-based index of the element to get or set. </param>
    /// <returns> The byte at the specified index. </returns>
    /// <exception cref="IndexOutOfRangeException"> <paramref name="tagIndex"/> is outside the array bounds. </exception>
    public new byte this[int tagIndex]
    {
        get { return Value[tagIndex]; }
        set { Value[tagIndex] = value; }
    }


    internal override bool ReadTag(TagReader readStream)
    {
        int length = readStream.ReadInt32();
        if (length < 0)
        {
            throw new NbtFormatException("Negative length given in TAG_Byte_Array");
        }

        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.Skip(length);
            return false;
        }
        Value = readStream.ReadBytes(length);
        if (Value.Length < length)
        {
            throw new EndOfStreamException();
        }
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        int length = readStream.ReadInt32();
        if (length < 0)
        {
            throw new NbtFormatException("Negative length given in TAG_Byte_Array");
        }
        readStream.Skip(length);
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.ByteArray);
        WriteData(writeStream);
    }


    internal override void WriteData(TagWriter writeStream)
    {
        writeStream.Write(Value.Length);
        writeStream.Write(Value, 0, Value.Length);
    }


    /// <inheritdoc />
    public override object Clone()
    {
        return new ByteArrayTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Byte_Array[");
        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", elements.Length);
        sb.Append(" bytes]");
    }

    protected override bool EqualsInternal(ByteArrayTag other)
    {
        return Value.AsSpan() == other.Value.AsSpan();
    }

    public override int GetHashCode()
    {
        return Value.Aggregate(0, (hash, element) => HashCode.Combine(hash, element.GetHashCode()));
    }
}
