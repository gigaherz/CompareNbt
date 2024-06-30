using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CompareNbt.Parsing.Tags;

/// <summary> A tag containing a set of other named tags. Order is not guaranteed. </summary>
public sealed class CompoundTag : Tag<CompoundTag>, IDictionary<string, Tag>
{
    /// <summary> Type of this tag (Compound). </summary>
    public override TagType Type
    {
        get { return TagType.Compound; }
    }

    readonly Dictionary<string, Tag> tags = [];


    /// <summary> Creates an empty unnamed NbtByte tag. </summary>
    public CompoundTag() { }

    /// <summary> Creates a deep copy of given NbtCompound. </summary>
    /// <param name="other"> An existing NbtCompound to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
    public CompoundTag(CompoundTag other)
    {
        ArgumentNullException.ThrowIfNull(other);
        foreach (var (key, value) in other.tags)
        {
            Add(key, (Tag)value.Clone());
        }
    }


    /// <summary> Gets or sets the tag with the specified name. May return <c>null</c>. </summary>
    /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
    /// <param name="tagName"> The name of the tag to get or set. Must match tag's actual name. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>; or if trying to assign null value. </exception>
    /// <exception cref="ArgumentException"> <paramref name="tagName"/> does not match the given tag's actual name;
    /// or given tag already has a Parent. </exception>
    public override Tag this[string tagName]
    {
        get
        {
            return tags[tagName];
        }
        set
        {
            ArgumentNullException.ThrowIfNull(tagName);
            ArgumentNullException.ThrowIfNull(value);

            if (tags.TryGetValue(tagName, out var current))
            {
                if (current == value)
                    return; // already set
            }

            NbtStructuralChecks.ThrowIfHasParent(value);
            NbtStructuralChecks.ThrowIfCircularDependency(value, this);

            tags[tagName] = value;
            value.Parent = this;

            if (current != null)
            {
                current.Parent = null;
            }
        }
    }


    /// <summary> Gets the tag with the specified name. May return <c>null</c>. </summary>
    /// <param name="tagName"> The name of the tag to get. </param>
    /// <typeparam name="T"> Type to cast the result to. Must derive from NbtTag. </typeparam>
    /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
    /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
    public T? Get<T>(string tagName) where T : Tag
    {
        ArgumentNullException.ThrowIfNull(tagName);
        if (tags.TryGetValue(tagName, out var result))
        {
            return (T)result;
        }
        return null;
    }


    /// <summary> Gets the tag with the specified name. May return <c>null</c>. </summary>
    /// <param name="tagName"> The name of the tag to get. </param>
    /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
    /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
    public Tag? Get(string tagName)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        if (tags.TryGetValue(tagName, out var result))
        {
            return result;
        }
        return null;
    }


    /// <summary> Gets the tag with the specified name. </summary>
    /// <param name="tagName"> The name of the tag to get. </param>
    /// <param name="result"> When this method returns, contains the tag associated with the specified name, if the tag is found;
    /// otherwise, null. This parameter is passed uninitialized. </param>
    /// <typeparam name="T"> Type to cast the result to. Must derive from NbtTag. </typeparam>
    /// <returns> true if the NbtCompound contains a tag with the specified name; otherwise, false. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
    /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
    public bool TryGet<T>(string tagName, out T? result) where T : Tag
    {
        ArgumentNullException.ThrowIfNull(tagName);
        if (tags.TryGetValue(tagName, out var tempResult))
        {
            result = (T)tempResult;
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }


    /// <summary> Gets the tag with the specified name. </summary>
    /// <param name="tagName"> The name of the tag to get. </param>
    /// <param name="result"> When this method returns, contains the tag associated with the specified name, if the tag is found;
    /// otherwise, null. This parameter is passed uninitialized. </param>
    /// <returns> true if the NbtCompound contains a tag with the specified name; otherwise, false. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
    public bool TryGet(string tagName, out Tag? result)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        if (tags.TryGetValue(tagName, out var tempResult))
        {
            result = tempResult;
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    /// <summary> Determines whether this NbtCompound contains a tag with a specific name. </summary>
    /// <param name="tagName"> Tag name to search for. May not be <c>null</c>. </param>
    /// <returns> true if a tag with given name was found; otherwise, false. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
    public bool Contains(string tagName)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        return tags.ContainsKey(tagName);
    }


    /// <summary> Removes the tag with the specified name from this NbtCompound. </summary>
    /// <param name="tagName"> The name of the tag to remove. </param>
    /// <returns> true if the tag is successfully found and removed; otherwise, false.
    /// This method returns false if name is not found in the NbtCompound. </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
    public bool Remove(string tagName)
    {
        ArgumentNullException.ThrowIfNull(tagName);
        if (!tags.TryGetValue(tagName, out var tag))
        {
            return false;
        }
        tags.Remove(tagName);
        tag.Parent = null;
        return true;
    }


    internal void RenameTag(string oldName, string newName)
    {
        ArgumentNullException.ThrowIfNull(oldName);
        ArgumentNullException.ThrowIfNull(newName);
        if (newName == oldName) return;
        if (tags.TryGetValue(newName, out _))
        {
            throw new ArgumentException("Cannot rename: a tag with the name already exists in this compound.");
        }
        if (!tags.TryGetValue(oldName, out Tag? tag))
        {
            throw new ArgumentException("Cannot rename: no tag found to rename.");
        }
        tags.Remove(oldName);
        tags.Add(newName, tag);
    }


    /// <summary> Gets a collection containing all tag names in this NbtCompound. </summary>
    public IEnumerable<string> Names
    {
        get { return tags.Keys; }
    }

    /// <summary> Gets a collection containing all tags in this NbtCompound. </summary>
    public IEnumerable<Tag> Tags
    {
        get { return tags.Values; }
    }


    #region Reading / Writing

    internal override bool ReadTag(TagReader readStream)
    {
        if (Parent != null && readStream.Selector != null && !readStream.Selector(this))
        {
            SkipTag(readStream);
            return false;
        }

        while (true)
        {
            TagType nextTag = readStream.ReadTagType();
            Tag newTag;
            switch (nextTag)
            {
                case TagType.End:
                    return true;

                case TagType.Byte:
                    newTag = new ByteTag();
                    break;

                case TagType.Short:
                    newTag = new ShortTag();
                    break;

                case TagType.Int:
                    newTag = new IntTag();
                    break;

                case TagType.Long:
                    newTag = new LongTag();
                    break;

                case TagType.Float:
                    newTag = new FloatTag();
                    break;

                case TagType.Double:
                    newTag = new DoubleTag();
                    break;

                case TagType.ByteArray:
                    newTag = new ByteArrayTag();
                    break;

                case TagType.String:
                    newTag = new StringTag();
                    break;

                case TagType.List:
                    newTag = new ListTag();
                    break;

                case TagType.Compound:
                    newTag = new CompoundTag();
                    break;

                case TagType.IntArray:
                    newTag = new IntArrayTag();
                    break;

                case TagType.LongArray:
                    newTag = new LongArrayTag();
                    break;

                default:
                    throw new NbtFormatException("Unsupported tag type found in NBT_Compound: " + nextTag);
            }
            newTag.Parent = this;
            var tagName = readStream.ReadString();
            if (newTag.ReadTag(readStream))
            {
                // ReSharper disable AssignNullToNotNullAttribute
                // newTag.Name is never null
                tags.Add(tagName, newTag);
                // ReSharper restore AssignNullToNotNullAttribute
            }
        }
    }


    internal override void SkipTag(TagReader readStream)
    {
        while (true)
        {
            TagType nextTag = readStream.ReadTagType();
            Tag newTag;
            switch (nextTag)
            {
                case TagType.End:
                    return;

                case TagType.Byte:
                    newTag = new ByteTag();
                    break;

                case TagType.Short:
                    newTag = new ShortTag();
                    break;

                case TagType.Int:
                    newTag = new IntTag();
                    break;

                case TagType.Long:
                    newTag = new LongTag();
                    break;

                case TagType.Float:
                    newTag = new FloatTag();
                    break;

                case TagType.Double:
                    newTag = new DoubleTag();
                    break;

                case TagType.ByteArray:
                    newTag = new ByteArrayTag();
                    break;

                case TagType.String:
                    newTag = new StringTag();
                    break;

                case TagType.List:
                    newTag = new ListTag();
                    break;

                case TagType.Compound:
                    newTag = new CompoundTag();
                    break;

                case TagType.IntArray:
                    newTag = new IntArrayTag();
                    break;

                case TagType.LongArray:
                    newTag = new LongArrayTag();
                    break;

                default:
                    throw new NbtFormatException("Unsupported tag type found in NBT_Compound: " + nextTag);
            }
            readStream.SkipString();
            newTag.SkipTag(readStream);
        }
    }


    internal override void WriteTag(TagWriter writeStream, string name)
    {
        writeStream.Write(TagType.Compound);
        writeStream.Write(name);
        WriteData(writeStream);
    }


    internal override void WriteData(TagWriter writeStream)
    {
        foreach (var (key,value) in tags)
        {
            value.WriteTag(writeStream, key);
        }
        writeStream.Write(TagType.End);
    }

    #endregion

    #region IDictionary
    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, Tag>> GetEnumerator()
    {
        return tags.GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(string tagName, Tag newTag)
    {
        ArgumentNullException.ThrowIfNull(newTag);
        NbtStructuralChecks.ThrowIfHasParent(newTag);
        NbtStructuralChecks.ThrowIfCircularDependency(newTag, this);
        tags.Add(tagName, newTag);
        newTag.Parent = this;
    }


    /// <inheritdoc />
    public void Clear()
    {
        foreach (Tag tag in tags.Values)
        {
            tag.Parent = null;
        }
        tags.Clear();
    }

    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
        return tags.ContainsKey(key);
    }

    /// <inheritdoc />
    public bool ContainsValue(Tag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        return tags.ContainsValue(tag);
    }

    /// <inheritdoc />
    public int Count => tags.Count;

    public ICollection<string> Keys => tags.Keys;

    public ICollection<Tag> Values => tags.Values;

    bool ICollection<KeyValuePair<string, Tag>>.IsReadOnly => false;

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Tag value)
    {
        return tags.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    void ICollection<KeyValuePair<string, Tag>>.Add(KeyValuePair<string, Tag> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc />
    bool ICollection<KeyValuePair<string, Tag>>.Contains(KeyValuePair<string, Tag> item)
    {
        return tags.Contains(item);
    }

    /// <inheritdoc />
    void ICollection<KeyValuePair<string, Tag>>.CopyTo(KeyValuePair<string, Tag>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, Tag>>)tags).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    bool ICollection<KeyValuePair<string, Tag>>.Remove(KeyValuePair<string, Tag> item)
    {
        return ((ICollection<KeyValuePair<string, Tag>>)tags).Remove(item);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    /// <inheritdoc />
    public override object Clone()
    {
        return new CompoundTag(this);
    }


    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            sb.Append(indentString);
        }
        sb.Append("TAG_Compound[");
        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", tags.Count);
        sb.Append(" entries {{");
        if (Count > 0)
        {
            sb.Append('\n');
            foreach (Tag tag in tags.Values)
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

    protected override bool EqualsInternal(CompoundTag other)
    {
        if (other == null) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            if (!this[i].Equals(other[i])) return false;
        }
        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return tags.Aggregate(0, (hash, kvp) => HashCode.Combine(hash, kvp.Key, kvp.Value.GetHashCode()));
    }

}
