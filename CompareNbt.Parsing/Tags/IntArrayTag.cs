using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing an array of signed 32-bit integers. </summary>
public sealed class IntArrayTag : ArrayTag<IntArrayTag, int>
{
    /// <summary> Type of this tag (ByteArray). </summary>
    public override TagType Type => TagType.IntArray;

    /// <summary> Value/payload of this tag (an array of signed 32-bit integers). Value is stored as-is and is NOT cloned. May not be <c>null</c>. </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    public override int[] Value
    {
        get { return elements; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            elements = value;
        }
    }

    int[] elements;


    /// <summary> Creates an unnamed NbtIntArray tag, containing an empty array of ints. </summary>
    public IntArrayTag()
        : this(Array.Empty<int>()) 
    {
    }


    /// <summary> Creates an unnamed NbtIntArray tag, containing the given array of ints. </summary>
    /// <param name="value"> Int array to assign to this tag's Value. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    /// <remarks> Given int array will be cloned. To avoid unnecessary copying, call one of the other constructor
    /// overloads (that do not take a int[]) and then set the Value property yourself. </remarks>
    public IntArrayTag(int[] value)
    {
        ArgumentNullException.ThrowIfNull(value);
        elements = (int[])value.Clone();
    }


    /// <summary> Creates a deep copy of given NbtIntArray. </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    /// <remarks> Int array of given tag will be cloned. </remarks>
    public IntArrayTag(IntArrayTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        elements = (int[])other.Value.Clone();
    }


    /// <summary> Gets or sets an integer at the given index. </summary>
    /// <param name="tagIndex"> The zero-based index of the element to get or set. </param>
    /// <returns> The integer at the specified index. </returns>
    /// <exception cref="IndexOutOfRangeException"> <paramref name="tagIndex"/> is outside the array bounds. </exception>
    public new int this[int tagIndex]
    {
        get { return Value[tagIndex]; }
        set { Value[tagIndex] = value; }
    }


    internal override bool ReadTag(TagReader readStream)
    {
        int length = readStream.ReadInt32();
        if (length < 0)
        {
            throw new NbtFormatException("Negative length given in TAG_Int_Array");
        }

        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.Skip(length * sizeof(int));
            return false;
        }

        Value = new int[length];
        for (int i = 0; i < length; i++)
        {
            Value[i] = readStream.ReadInt32();
        }
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        int length = readStream.ReadInt32();
        if (length < 0)
        {
            throw new NbtFormatException("Negative length given in TAG_Int_Array");
        }
        readStream.Skip(length * sizeof(int));
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.IntArray);
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
        return new IntArrayTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Int_Array[");
        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", elements.Length);
        sb.Append(" ints]");
    }

    protected override bool EqualsInternal(IntArrayTag other)
    {
        return Value.AsSpan() == other.Value.AsSpan();
    }

    public override int GetHashCode()
    {
        return Value.Aggregate(0, (hash, element) => HashCode.Combine(hash, element.GetHashCode()));
    }
}
