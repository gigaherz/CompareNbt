using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a list of unnamed tags, all of the same kind. </summary>
public sealed class ListTag : Tag<ListTag>, IList<Tag>, IList, ICollection<Tag>, ICollection
{
    /// <summary> Type of this tag (List). </summary>
    public override TagType Type
    {
        get { return TagType.List; }
    }

    readonly List<Tag> tags = new List<Tag>();

    /// <summary> Gets or sets the tag type of this list. All tags in this NbtTag must be of the same type. </summary>
    /// <exception cref="ArgumentException"> If the given NbtTagType does not match the type of existing list items (for non-empty lists). </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If the given NbtTagType is a recognized tag type. </exception>
    public TagType ListType
    {
        get { return listType; }
        set
        {
            if (value == TagType.End)
            {
                // Empty lists may have type "End", see: https://github.com/fragmer/fNbt/issues/12
                if (tags.Count > 0)
                {
                    throw new ArgumentException("Only empty list tags may have TagType of End.");
                }
            }
            else if (value < TagType.Byte || value > TagType.LongArray && value != TagType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (tags.Count > 0)
            {
                TagType actualType = tags[0].Type;
                // We can safely assume that ALL tags have the same TagType as the first tag.
                if (actualType != value)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture,
                                               "Given NbtTagType ({0}) does not match actual element type ({1})",
                                               value, actualType);
                    throw new ArgumentException(msg);
                }
            }
            listType = value;
        }
    }

    TagType listType;


    /// <summary> Creates an unnamed NbtList with empty contents and undefined ListType. </summary>
    public ListTag()
        : this(TagType.Unknown)
    {
    }


    /// <summary> Creates an NbtList with the given name and contents, and inferred ListType. 
    /// If given tag array is empty, NbtTagType remains Unknown. </summary>
    /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
    /// <param name="tags"> Collection of tags to insert into the list. All tags are expected to be of the same type.
    /// ListType is inferred from the first tag. List may be empty, but may not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="tags"/> is <c>null</c>. </exception>
    /// <exception cref="ArgumentException"> If given tags are of mixed types. </exception>
    public ListTag(IEnumerable<Tag> tags)
        : this(tags, TagType.Unknown)
    {
        // the base constructor will allow null "tags," but we don't want that in this constructor
        ArgumentNullException.ThrowIfNull(tags);
    }



    /// <summary> Creates an NbtList with the given name, empty contents, and an explicitly specified ListType. </summary>
    /// <param name="givenListType"> Name to assign to this tag.
    /// If givenListType is Unknown, ListType will be inferred from the first tag added to this NbtList. </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="givenListType"/> is not a valid tag type. </exception>
    public ListTag(TagType givenListType)
        : this([], givenListType)
    { 
    }


    /// <summary> Creates an NbtList with the given name and contents, and an explicitly specified ListType. </summary>
    /// <param name="tags"> Collection of tags to insert into the list.
    /// All tags are expected to be of the same type (matching givenListType). May be empty or <c>null</c>. </param>
    /// <param name="givenListType"> Name to assign to this tag. May be Unknown (to infer type from the first element of tags). </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="givenListType"/> is not a valid tag type. </exception>
    /// <exception cref="ArgumentException"> If given tags do not match <paramref name="givenListType"/>, or are of mixed types. </exception>
    public ListTag(IEnumerable<Tag> tags, TagType givenListType)
    {
        ListType = givenListType;

        AddRange(tags);
    }


    /// <summary> Creates a deep copy of given NbtList. </summary>
    /// <param name="other"> An existing NbtList to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public ListTag(ListTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        listType = other.listType;
        foreach (Tag tag in other.tags)
        {
            tags.Add((Tag)tag.Clone());
        }
    }


    /// <summary> Gets or sets the tag at the specified index. </summary>
    /// <returns> The tag at the specified index. </returns>
    /// <param name="tagIndex"> The zero-based index of the tag to get or set. </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagIndex"/> is not a valid index in the NbtList. </exception>
    /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
    /// <exception cref="ArgumentException"> Given tag's type does not match ListType. </exception>
    public override Tag this[int tagIndex]
    {
        get { return tags[tagIndex]; }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.Parent != null)
            {
                throw new ArgumentException("A tag may only be added to one compound/list at a time.");
            }
            if (value == this || value == Parent)
            {
                throw new ArgumentException("A list tag may not be added to itself or to its child tag.");
            }
            if (listType != TagType.Unknown && value.Type != listType)
            {
                throw new ArgumentException("Items must be of type " + listType);
            }
            tags[tagIndex] = value;
            value.Parent = this;
        }
    }


    /// <summary> Gets or sets the tag with the specified name. </summary>
    /// <param name="tagIndex"> The zero-based index of the tag to get or set. </param>
    /// <typeparam name="T"> Type to cast the result to. Must derive from NbtTag. </typeparam>
    /// <returns> The tag with the specified key. </returns>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagIndex"/> is not a valid index in the NbtList. </exception>
    /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
    public T Get<T>(int tagIndex) where T : Tag
    {
        return (T)tags[tagIndex];
    }


    /// <summary> Adds all tags from the specified collection to the end of this NbtList. </summary>
    /// <param name="newTags"> The collection whose elements should be added to this NbtList. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="newTags"/> is <c>null</c>. </exception>
    /// <exception cref="ArgumentException"> If given tags do not match ListType, or are of mixed types. </exception>
    public void AddRange(IEnumerable<Tag> newTags)
    {
        ArgumentNullException.ThrowIfNull(newTags);
        foreach (Tag tag in newTags)
        {
            Add(tag);
        }
    }


    /// <summary> Copies all tags in this NbtList to an array. </summary>
    /// <returns> Array of NbtTags. </returns>
    public Tag[] ToArray()
    {
        return tags.ToArray();
    }


    /// <summary> Copies all tags in this NbtList to an array, and casts it to the desired type. </summary>
    /// <typeparam name="T"> Type to cast every member of NbtList to. Must derive from NbtTag. </typeparam>
    /// <returns> Array of NbtTags cast to the desired type. </returns>
    /// <exception cref="InvalidCastException"> If contents of this list cannot be cast to the given type. </exception>
    public T[] ToArray<T>() where T : Tag
    {
        var result = new T[tags.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (T)tags[i];
        }
        return result;
    }


    #region Reading / Writing

    internal override bool ReadTag(TagReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            SkipTag(readStream);
            return false;
        }

        ListType = readStream.ReadTagType();

        int length = readStream.ReadInt32();
        if (length < 0)
        {
            throw new NbtFormatException("Negative list size given.");
        }

        for (int i = 0; i < length; i++)
        {
            Tag newTag = ListType switch
            {
                TagType.Byte => new ByteTag(),
                TagType.Short => new ShortTag(),
                TagType.Int => new IntTag(),
                TagType.Long => new LongTag(),
                TagType.Float => new FloatTag(),
                TagType.Double => new DoubleTag(),
                TagType.ByteArray => new ByteArrayTag(),
                TagType.String => new StringTag(),
                TagType.List => new ListTag(),
                TagType.Compound => new CompoundTag(),
                TagType.IntArray => new IntArrayTag(),
                TagType.LongArray => new LongArrayTag(),
                // should never happen, since ListType is checked beforehand
                _ => throw new NbtFormatException("Unsupported tag type found in a list: " + ListType),
            };
            newTag.Parent = this;
            if (newTag.ReadTag(readStream))
            {
                tags.Add(newTag);
            }
        }
        return true;
    }


    internal override void SkipTag(TagReader readStream)
    {
        // read list type, and make sure it's defined
        ListType = readStream.ReadTagType();

        int length = readStream.ReadInt32();
        if (length < 0)
        {
            throw new NbtFormatException("Negative list size given.");
        }

        switch (ListType)
        {
            case TagType.Byte:
                readStream.Skip(length);
                break;
            case TagType.Short:
                readStream.Skip(length * sizeof(short));
                break;
            case TagType.Int:
                readStream.Skip(length * sizeof(int));
                break;
            case TagType.Long:
                readStream.Skip(length * sizeof(long));
                break;
            case TagType.Float:
                readStream.Skip(length * sizeof(float));
                break;
            case TagType.Double:
                readStream.Skip(length * sizeof(double));
                break;
            default:
                for (int i = 0; i < length; i++)
                {
                    switch (listType)
                    {
                        case TagType.ByteArray:
                            new ByteArrayTag().SkipTag(readStream);
                            break;
                        case TagType.String:
                            readStream.SkipString();
                            break;
                        case TagType.List:
                            new ListTag().SkipTag(readStream);
                            break;
                        case TagType.Compound:
                            new CompoundTag().SkipTag(readStream);
                            break;
                        case TagType.IntArray:
                            new IntArrayTag().SkipTag(readStream);
                            break;
                    }
                }
                break;
        }
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.List);
        writeStream.Write(name);
        WriteData(writeStream);
    }


    internal override void WriteData(TagWriter writeStream)
    {
        if (ListType == TagType.Unknown)
        {
            throw new NbtFormatException("NbtList had no elements and an Unknown ListType");
        }
        writeStream.Write(ListType);
        writeStream.Write(tags.Count);
        foreach (Tag tag in tags)
        {
            tag.WriteData(writeStream);
        }
    }

    #endregion


    #region Implementation of IEnumerable<NBtTag> and IEnumerable

    /// <summary> Returns an enumerator that iterates through all tags in this NbtList. </summary>
    /// <returns> An IEnumerator&gt;NbtTag&lt; that can be used to iterate through the list. </returns>
    public IEnumerator<Tag> GetEnumerator()
    {
        return tags.GetEnumerator();
    }

    #endregion


    #region Implementation of IList<NbtTag> and ICollection<NbtTag>

    /// <summary> Determines the index of a specific tag in this NbtList </summary>
    /// <returns> The index of tag if found in the list; otherwise, -1. </returns>
    /// <param name="tag"> The tag to locate in this NbtList. </param>
    public int IndexOf(Tag? tag)
    {
        if (tag == null) return -1;
        return tags.IndexOf(tag);
    }


    /// <summary> Inserts an item to this NbtList at the specified index. </summary>
    /// <param name="tagIndex"> The zero-based index at which newTag should be inserted. </param>
    /// <param name="newTag"> The tag to insert into this NbtList. </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagIndex"/> is not a valid index in this NbtList. </exception>
    /// <exception cref="ArgumentNullException"> <paramref name="newTag"/> is <c>null</c>. </exception>
    public void Insert(int tagIndex, Tag newTag)
    {
        ArgumentNullException.ThrowIfNull(newTag);
        if (listType != TagType.Unknown && newTag.Type != listType)
        {
            throw new ArgumentException("Items must be of type " + listType);
        }
        if (newTag.Parent != null)
        {
            throw new ArgumentException("A tag may only be added to one compound/list at a time.");
        }
        tags.Insert(tagIndex, newTag);
        if (listType == TagType.Unknown)
        {
            listType = newTag.Type;
        }
        newTag.Parent = this;
    }


    /// <summary> Removes a tag at the specified index from this NbtList. </summary>
    /// <param name="index"> The zero-based index of the item to remove. </param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="index"/> is not a valid index in the NbtList. </exception>
    public void RemoveAt(int index)
    {
        Tag tag = this[index];
        tags.RemoveAt(index);
        tag.Parent = null;
    }


    /// <summary> Adds a tag to this NbtList. </summary>
    /// <param name="newTag"> The tag to add to this NbtList. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="newTag"/> is <c>null</c>. </exception>
    /// <exception cref="ArgumentException"> If <paramref name="newTag"/> does not match ListType. </exception>
    public void Add(Tag newTag)
    {
        ArgumentNullException.ThrowIfNull(newTag);
        NbtStructuralChecks.ThrowIfHasParent(newTag);
        NbtStructuralChecks.ThrowIfCircularDependency(newTag, this);
        if (listType != TagType.Unknown && newTag.Type != listType)
        {
            throw new ArgumentException("Items in this list must be of type " + listType + ". Given type: " +
                                        newTag.Type);
        }
        tags.Add(newTag);
        newTag.Parent = this;
        if (listType == TagType.Unknown)
        {
            listType = newTag.Type;
        }
    }


    /// <summary> Removes all tags from this NbtList. </summary>
    public void Clear()
    {
        for (int i = 0; i < tags.Count; i++)
        {
            tags[i].Parent = null;
        }
        tags.Clear();
    }


    /// <summary> Determines whether this NbtList contains a specific tag. </summary>
    /// <returns> true if given tag is found in this NbtList; otherwise, false. </returns>
    /// <param name="item"> The tag to locate in this NbtList. </param>
    public bool Contains(Tag item)
    {
        return tags.Contains(item);
    }


    /// <summary> Copies the tags of this NbtList to an array, starting at a particular array index. </summary>
    /// <param name="array"> The one-dimensional array that is the destination of the tag copied from NbtList.
    /// The array must have zero-based indexing. </param>
    /// <param name="arrayIndex"> The zero-based index in array at which copying begins. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="array"/> is <c>null</c>. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> arrayIndex is less than 0. </exception>
    /// <exception cref="ArgumentException"> Given array is multidimensional; arrayIndex is equal to or greater than the length of array;
    /// the number of tags in this NbtList is greater than the available space from arrayIndex to the end of the destination array;
    /// or type NbtTag cannot be cast automatically to the type of the destination array. </exception>
    public void CopyTo(Tag[] array, int arrayIndex)
    {
        tags.CopyTo(array, arrayIndex);
    }


    /// <summary> Removes the first occurrence of a specific NbtTag from the NbtCompound.
    /// Looks for exact object matches, not name matches. </summary>
    /// <returns> true if tag was successfully removed from this NbtList; otherwise, false.
    /// This method also returns false if tag is not found. </returns>
    /// <param name="tag"> The tag to remove from this NbtList. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="tag"/> is <c>null</c>. </exception>
    public bool Remove(Tag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        if (!tags.Remove(tag))
        {
            return false;
        }
        tag.Parent = null;
        return true;
    }


    /// <summary> Gets the number of tags contained in the NbtList. </summary>
    /// <returns> The number of tags contained in the NbtList. </returns>
    public int Count
    {
        get { return tags.Count; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool ICollection<Tag>.IsReadOnly
    {
        get { return false; }
    }

    void ICollection.CopyTo(Array array, int index)
    {
        CopyTo((Tag[])array, index);
    }

    bool ICollection.IsSynchronized
    {
        get { return false; }
    }

    #endregion


    #region Implementation of IList and ICollection

    void IList.Remove(object? value)
    {
        if (value is null) return;
        Remove((Tag)value);
    }


    object? IList.this[int tagIndex]
    {
        get { return tags[tagIndex]; }
        set 
        {
            ArgumentNullException.ThrowIfNull(value);
            this[tagIndex] = (Tag)value; 
        }
    }


    int IList.Add(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add((Tag)value);
        return tags.Count - 1;
    }


    bool IList.Contains(object? value)
    {
        if (value is null) return false;
        return tags.Contains((Tag)value);
    }


    int IList.IndexOf(object? value)
    {
        if (value is null) return -1;
        return tags.IndexOf((Tag)value);
    }


    void IList.Insert(int index, object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Insert(index, (Tag)value);
    }


    bool IList.IsFixedSize
    {
        get { return false; }
    }

    /// <inheritdoc/>
    public object SyncRoot
    {
        get { return (tags as ICollection).SyncRoot; }
    }

    bool IList.IsReadOnly
    {
        get { return false; }
    }

    #endregion


    /// <inheritdoc />
    public override object Clone()
    {
        return new ListTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_List[");
        sb.AppendFormat(CultureInfo.InvariantCulture, "{0} entries {{", tags.Count);

        if (Count > 0)
        {
            sb.Append('\n');
            foreach (Tag tag in tags)
            {
                tag.PrettyPrint(sb, indentString, indentLevel + 1);
                sb.Append('\n');
            }
            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append(indentString);
            }
        }
        sb.Append("}}]");
    }

    protected override bool EqualsInternal(ListTag other)
    {
        if (other == null) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            if (!this[i].Equals(other[i])) return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        return this.Aggregate(0, (hash, element) => HashCode.Combine(hash, element.GetHashCode()));
    }
}
