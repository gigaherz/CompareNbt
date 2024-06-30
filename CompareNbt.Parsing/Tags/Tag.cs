using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> Base class for different kinds of named binary tags. </summary>
public abstract class Tag : ICloneable
{
    /// <summary> Parent compound tag, either NbtList or NbtCompound, if any.
    /// May be <c>null</c> for detached tags. </summary>
    public Tag? Parent { get; internal set; }

    /// <summary> Type of this tag. </summary>
    public abstract TagType Type { get; }

    /// <summary> Returns true if tags of this type have a value attached.
    /// All tags except Compound, List, and End have values. </summary>
    public bool HasValue
    {
        get
        {
            switch (Type)
            {
                case TagType.Compound:
                case TagType.End:
                case TagType.List:
                case TagType.Unknown:
                    return false;
                default:
                    return true;
            }
        }
    }

    internal abstract bool ReadTag(TagReader readStream);

    internal abstract void SkipTag(TagReader readStream);

    internal abstract void WriteTag(TagWriter writeReader, string name);

    // WriteData does not write the tag's ID byte or the name
    internal abstract void WriteData(TagWriter writeStream);


    #region Shortcuts

    /// <summary> Gets or sets the tag with the specified name. May return <c>null</c>. </summary>
    /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
    /// <param name="tagName"> The name of the tag to get or set. Must match tag's actual name. </param>
    /// <exception cref="InvalidOperationException"> If used on a tag that is not NbtCompound. </exception>
    /// <remarks> ONLY APPLICABLE TO NbtCompound OBJECTS!
    /// Included in NbtTag base class for programmers' convenience, to avoid extra type casts. </remarks>
    public virtual Tag? this[string tagName]
    {
        get { throw new InvalidOperationException("String indexers only work on NbtCompound tags."); }
        set { throw new InvalidOperationException("String indexers only work on NbtCompound tags."); }
    }

    /// <summary> Gets or sets the tag at the specified index. </summary>
    /// <returns> The tag at the specified index. </returns>
    /// <param name="tagIndex"> The zero-based index of the tag to get or set. </param>
    /// <exception cref="ArgumentOutOfRangeException"> tagIndex is not a valid index in this tag. </exception>
    /// <exception cref="ArgumentNullException"> Given tag is <c>null</c>. </exception>
    /// <exception cref="ArgumentException"> Given tag's type does not match ListType. </exception>
    /// <exception cref="InvalidOperationException"> If used on a tag that is not NbtList, NbtByteArray, or NbtIntArray. </exception>
    /// <remarks> ONLY APPLICABLE TO NbtList, NbtByteArray, and NbtIntArray OBJECTS!
    /// Included in NbtTag base class for programmers' convenience, to avoid extra type casts. </remarks>
    public virtual Tag this[int tagIndex]
    {
        get { throw new InvalidOperationException("Integer indexers only work on NbtList tags."); }
        set { throw new InvalidOperationException("Integer indexers only work on NbtList tags."); }
    }

    /// <summary> Returns the value of this tag, cast as a byte.
    /// Only supported by NbtByte tags. </summary>
    /// <exception cref="InvalidCastException"> When used on a tag other than NbtByte. </exception>
    public byte ByteValue
    {
        get
        {
            if (Type == TagType.Byte)
            {
                return ((ByteTag)this).Value;
            }
            else
            {
                throw new InvalidCastException("Cannot get ByteValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as a short (16-bit signed integer).
    /// Only supported by NbtByte and NbtShort. </summary>
    /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
    public short ShortValue
    {
        get
        {
            switch (Type)
            {
                case TagType.Byte:
                    return ((ByteTag)this).Value;
                case TagType.Short:
                    return ((ShortTag)this).Value;
                default:
                    throw new InvalidCastException("Cannot get ShortValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as an int (32-bit signed integer).
    /// Only supported by NbtByte, NbtShort, and NbtInt. </summary>
    /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
    public int IntValue
    {
        get
        {
            switch (Type)
            {
                case TagType.Byte:
                    return ((ByteTag)this).Value;
                case TagType.Short:
                    return ((ShortTag)this).Value;
                case TagType.Int:
                    return ((IntTag)this).Value;
                default:
                    throw new InvalidCastException("Cannot get IntValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as a long (64-bit signed integer).
    /// Only supported by NbtByte, NbtShort, NbtInt, and NbtLong. </summary>
    /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
    public long LongValue
    {
        get
        {
            switch (Type)
            {
                case TagType.Byte:
                    return ((ByteTag)this).Value;
                case TagType.Short:
                    return ((ShortTag)this).Value;
                case TagType.Int:
                    return ((IntTag)this).Value;
                case TagType.Long:
                    return ((LongTag)this).Value;
                default:
                    throw new InvalidCastException("Cannot get LongValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as a long (64-bit signed integer).
    /// Only supported by NbtFloat and, with loss of precision, by NbtDouble, NbtByte, NbtShort, NbtInt, and NbtLong. </summary>
    /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
    public float FloatValue
    {
        get
        {
            switch (Type)
            {
                case TagType.Byte:
                    return ((ByteTag)this).Value;
                case TagType.Short:
                    return ((ShortTag)this).Value;
                case TagType.Int:
                    return ((IntTag)this).Value;
                case TagType.Long:
                    return ((LongTag)this).Value;
                case TagType.Float:
                    return ((FloatTag)this).Value;
                case TagType.Double:
                    return (float)((DoubleTag)this).Value;
                default:
                    throw new InvalidCastException("Cannot get FloatValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as a long (64-bit signed integer).
    /// Only supported by NbtFloat, NbtDouble, and, with loss of precision, by NbtByte, NbtShort, NbtInt, and NbtLong. </summary>
    /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
    public double DoubleValue
    {
        get
        {
            switch (Type)
            {
                case TagType.Byte:
                    return ((ByteTag)this).Value;
                case TagType.Short:
                    return ((ShortTag)this).Value;
                case TagType.Int:
                    return ((IntTag)this).Value;
                case TagType.Long:
                    return ((LongTag)this).Value;
                case TagType.Float:
                    return ((FloatTag)this).Value;
                case TagType.Double:
                    return ((DoubleTag)this).Value;
                default:
                    throw new InvalidCastException("Cannot get DoubleValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as a byte array.
    /// Only supported by NbtByteArray tags. </summary>
    /// <exception cref="InvalidCastException"> When used on a tag other than NbtByteArray. </exception>
    public byte[] ByteArrayValue
    {
        get
        {
            if (Type == TagType.ByteArray)
            {
                return ((ByteArrayTag)this).Value;
            }
            else
            {
                throw new InvalidCastException("Cannot get ByteArrayValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as an int array.
    /// Only supported by NbtIntArray tags. </summary>
    /// <exception cref="InvalidCastException"> When used on a tag other than NbtIntArray. </exception>
    public int[] IntArrayValue
    {
        get
        {
            if (Type == TagType.IntArray)
            {
                return ((IntArrayTag)this).Value;
            }
            else
            {
                throw new InvalidCastException("Cannot get IntArrayValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as a long array.
    /// Only supported by NbtLongArray tags. </summary>
    /// <exception cref="InvalidCastException"> When used on a tag other than NbtLongArray. </exception>
    public long[] LongArrayValue
    {
        get
        {
            if (Type == TagType.LongArray)
            {
                return ((LongArrayTag)this).Value;
            }
            else
            {
                throw new InvalidCastException("Cannot get LongArrayValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    /// <summary> Returns the value of this tag, cast as a string.
    /// Returns exact value for NbtString, and stringified (using InvariantCulture) value for NbtByte, NbtDouble, NbtFloat, NbtInt, NbtLong, and NbtShort.
    /// Not supported by NbtCompound, NbtList, NbtByteArray, NbtIntArray, or NbtLongArray. </summary>
    /// <exception cref="InvalidCastException"> When used on an unsupported tag. </exception>
    public string StringValue
    {
        get
        {
            switch (Type)
            {
                case TagType.String:
                    return ((StringTag)this).Value;
                case TagType.Byte:
                    return ((ByteTag)this).Value.ToString(CultureInfo.InvariantCulture);
                case TagType.Double:
                    return ((DoubleTag)this).Value.ToString(CultureInfo.InvariantCulture);
                case TagType.Float:
                    return ((FloatTag)this).Value.ToString(CultureInfo.InvariantCulture);
                case TagType.Int:
                    return ((IntTag)this).Value.ToString(CultureInfo.InvariantCulture);
                case TagType.Long:
                    return ((LongTag)this).Value.ToString(CultureInfo.InvariantCulture);
                case TagType.Short:
                    return ((ShortTag)this).Value.ToString(CultureInfo.InvariantCulture);
                default:
                    throw new InvalidCastException("Cannot get StringValue from " + GetCanonicalTagName(Type));
            }
        }
    }

    #endregion


    /// <summary> Returns a canonical (Notchy) name for the given NbtTagType,
    /// e.g. "TAG_Byte_Array" for NbtTagType.ByteArray </summary>
    /// <param name="type"> NbtTagType to name. </param>
    /// <returns> String representing the canonical name of a tag,
    /// or null of given TagType does not have a canonical name (e.g. Unknown). </returns>
    public static string? GetCanonicalTagName(TagType type)
    {
        return type switch
        {
            TagType.Byte => "TAG_Byte",
            TagType.ByteArray => "TAG_Byte_Array",
            TagType.Compound => "TAG_Compound",
            TagType.Double => "TAG_Double",
            TagType.End => "TAG_End",
            TagType.Float => "TAG_Float",
            TagType.Int => "TAG_Int",
            TagType.IntArray => "TAG_Int_Array",
            TagType.LongArray => "TAG_Long_Array",
            TagType.List => "TAG_List",
            TagType.Long => "TAG_Long",
            TagType.Short => "TAG_Short",
            TagType.String => "TAG_String",
            _ => null,
        };
    }


    /// <summary> Prints contents of this tag, and any child tags, to a string.
    /// Indents the string using multiples of the given indentation string. </summary>
    /// <returns> A string representing contents of this tag, and all child tags (if any). </returns>
    public override string ToString()
    {
        return ToString(DefaultIndentString);
    }


    /// <summary> Creates a deep copy of this tag. </summary>
    /// <returns> A new NbtTag object that is a deep copy of this instance. </returns>
    public abstract object Clone();


    /// <summary> Prints contents of this tag, and any child tags, to a string.
    /// Indents the string using multiples of the given indentation string. </summary>
    /// <param name="indentString"> String to be used for indentation. </param>
    /// <returns> A string representing contents of this tag, and all child tags (if any). </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="indentString"/> is <c>null</c>. </exception>
    public string ToString(string indentString)
    {
        ArgumentNullException.ThrowIfNull(indentString);
        var sb = new StringBuilder();
        PrettyPrint(sb, indentString, 0);
        return sb.ToString();
    }


    internal abstract void PrettyPrint(StringBuilder sb, string indentString, int indentLevel);

    /// <summary> String to use for indentation in NbtTag's and NbtFile's ToString() methods by default. </summary>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    public static string DefaultIndentString
    {
        get { return defaultIndentString; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            defaultIndentString = value;
        }
    }

    static string defaultIndentString = "  ";

    public override abstract bool Equals(object? other);
    public override abstract int GetHashCode();
}

public abstract class Tag<T> : Tag, IEquatable<T>
    where T : Tag<T>
{
    public override bool Equals(object? other)
    {
        if (other == this) return true;
        if (other == null) return false;
        if (other.GetType() != GetType()) return false;
        return EqualsInternal((T)other);
    }

    public bool Equals(T? other)
    {
        if (other == this) return true;
        if (other == null) return false;
        return EqualsInternal(other);
    }

    protected abstract bool EqualsInternal(T other);

    public override abstract int GetHashCode();
}
