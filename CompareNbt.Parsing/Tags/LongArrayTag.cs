using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing an array of signed 64-bit integers. </summary>
public sealed class LongArrayTag : ArrayTag<LongArrayTag, long>
{
    /// <summary> Type of this tag (LongArray). </summary>
    public override TagType Type => TagType.LongArray;

    /// <summary> Value/payload of this tag (an array of signed 64-bit integers). Value is stored as-is and is NOT cloned. May not be <c>null</c>. </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    public override long[] Value
    {
        get { return elements; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            elements = value;
        }
    }

    private long[] elements;

    /// <summary> Creates an unnamed NbtLongArray tag, containing an empty array of longs. </summary>
    public LongArrayTag()
        : this(Array.Empty<long>())
    { 
    }

    /// <summary> Creates an unnamed NbtLongArray tag, containing the given array of longs. </summary>
    /// <param name="value"> Long array to assign to this tag's Value. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    /// <remarks> Given long array will be cloned. To avoid unnecessary copying, call one of the other constructor
    /// overloads (that do not take a long[]) and then set the Value property yourself. </remarks>
    public LongArrayTag(long[] value)
    {
        ArgumentNullException.ThrowIfNull(value);
        elements = (long[])value.Clone();
    }


    /// <summary> Creates a deep copy of given NbtLongArray. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    /// <remarks> Long array of given tag will be cloned. </remarks>
    public LongArrayTag(LongArrayTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        elements = (long[])other.elements.Clone();
    }


    /// <summary> Gets or sets a long at the given index. </summary>
    /// <param name="index"> The zero-based index of the element to get or set. </param>
    /// <returns> The long at the specified index. </returns>
    /// <exception cref="IndexOutOfRangeException"> <paramref name="index"/> is outside the array bounds. </exception>
    public new long this[int index]
    {
        get { return Value[index]; }
        set { Value[index] = value; }
    }


    internal override bool ReadTag(TagReader readStream)
    {
        int length = readStream.ReadInt32();

        if (length < 0)
        {
            throw new NbtFormatException("Negative length given in TAG_Long_Array");
        }

        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.Skip(length * sizeof(long));
            return false;
        }

        Value = new long[length];

        for (int i = 0; i < length; i++)
        {
            Value[i] = readStream.ReadInt64();
        }

        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        int length = readStream.ReadInt32();

        if (length < 0)
        {
            throw new NbtFormatException("Negative length given in TAG_Long_Array");
        }

        readStream.Skip(length * sizeof(long));
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.LongArray);
        writeStream.Write(name);
        WriteData(writeStream);
    }


    internal override void WriteData(TagWriter writeStream)
    {
        writeStream.Write(Value.Length);

        for (int i = 0; i < Value.Length; i++)
        {
            writeStream.Write(Value[i]);
        }
    }


    /// <inheritdoc />
    public override object Clone()
    {
        return new LongArrayTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }

        sb.Append("TAG_Long_Array[");
        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", elements.Length);
        sb.Append(" longs]");
    }

    protected override bool EqualsInternal(LongArrayTag other)
    {
        return Value.AsSpan() == other.Value.AsSpan();
    }

    public override int GetHashCode()
    {
        return Value.Aggregate(0, (hash, element) => HashCode.Combine(hash, element.GetHashCode()));
    }
}
