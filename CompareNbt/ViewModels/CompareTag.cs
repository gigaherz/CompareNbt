using CommunityToolkit.Mvvm.ComponentModel;
using CompareNbt.Parsing;
using CompareNbt.Parsing.Tags;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CompareNbt.ViewModels;

public class CompareTag(Tag? tag, string? name = null) : ObservableObject, IEquatable<CompareTag>
{
    private readonly Tag? tag = tag;

    public bool HasKeyName { get; } = name != null;
    public string KeyName { get; } = name ?? string.Empty;

    public string DisplayName { get; } = tag?.Type switch
    {
        TagType.Byte => "Byte " + ((ByteTag)tag).Value,
        TagType.Short => "Short " + ((ShortTag)tag).Value,
        TagType.Int => "Int " + ((IntTag)tag).Value,
        TagType.Long => "Long " + ((LongTag)tag).Value,
        TagType.Float => "Float " + ((FloatTag)tag).Value,
        TagType.Double => "Double " + ((DoubleTag)tag).Value,
        TagType.String => "String \"" + ((StringTag)tag).Value + "\"",
        TagType.ByteArray => "ByteArray[" + ((ByteArrayTag)tag).Count + "]",
        TagType.IntArray => "IntArray[" + ((IntArrayTag)tag).Count + "]",
        TagType.LongArray => "LongArray[" + ((LongArrayTag)tag).Count + "]",
        TagType.List => "List(" + ((ListTag)tag).Count + ")",
        TagType.Compound => "Compound(" + ((CompoundTag)tag).Count + ")",
        TagType.End => "End.",
        _ => "?",
    };

    public bool InputEditable { get; } = tag?.Type switch
    {
        TagType.Byte => true,
        TagType.Short => true,
        TagType.Int => true,
        TagType.Long => true,
        TagType.Float => true,
        TagType.Double => true,
        TagType.String => true,
        _ => false,
    };

    private ObservableCollection<CompareTag>? _childTags = null;
    public ObservableCollection<CompareTag> ChildTags
    {
        get
        {
            _childTags ??= tag?.Type switch
                {
                    TagType.ByteArray => new ObservableCollection<CompareTag>(((ByteArrayTag)tag).Select(t => new CompareTag(new ByteTag(t)))),
                    TagType.IntArray => new ObservableCollection<CompareTag>(((IntArrayTag)tag).Select(t => new CompareTag(new IntTag(t)))),
                    TagType.LongArray => new ObservableCollection<CompareTag>(((LongArrayTag)tag).Select(t => new CompareTag(new LongTag(t)))),
                    TagType.List => new ObservableCollection<CompareTag>(((ListTag)tag).Select(t => new CompareTag(t))),
                    TagType.Compound => FromLookup(),
                    _ => []
                };
            return _childTags;
        }
    }

    private Dictionary<string, CompareTag>? _byNameLookup = null;
    public Dictionary<string, CompareTag> ByNameLookup
    {
        get
        {
            return EnsureLookupCreated();
        }
    }

    private ObservableCollection<CompareTag> FromLookup()
    {
        var lookup = EnsureLookupCreated();
        return new ObservableCollection<CompareTag>(lookup.Values);
    }

    private Dictionary<string, CompareTag> EnsureLookupCreated()
    {
        if (_byNameLookup == null)
        {
            _byNameLookup = [];
            if (tag?.Type == TagType.Compound)
            {
                foreach (var (key, value) in (CompoundTag)tag)
                {
                    _byNameLookup.Add(key, new CompareTag(value, key));
                }
            }
        }
        return _byNameLookup;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        return Equals(obj as CompareTag);
    }

    public bool Equals(CompareTag? other)
    {
        if (this == other) return true;
        if (other == null) return false;
        // implementation of the equality comparison

        if (tag == null) return other.tag == null;
        return tag.Equals(other.tag);
    }


    public override int GetHashCode()
    {
        if (tag == null) return 0;
        return tag.GetHashCode();
    }

    public string Change { get; set; } = "";
    internal bool ProcessDifferences(CompareTag leftSide, HashSet<string> seenChanges)
    {
        var leftTag = leftSide.tag;

        if (tag == null || leftTag == null) return true; // shouldn't happen

        if (tag.GetType() != leftTag.GetType())
        {
            this.Change = "+";
            leftSide.Change = "-";
        }

        bool changeDetected = false;
        switch (tag)
        {
            case ValueTag rightValue:
            {
                var leftValue = (ValueTag)leftTag;
                if (!Equals(leftValue.RawValue, rightValue.RawValue))
                {
                    this.Change = "*";
                    leftSide.Change = "*";
                    seenChanges.Add("*");
                    changeDetected = true;
                }
                break;
            }
            case ListTag rightList:
            {
                var leftList = (ListTag)leftTag;
                // TODO: Compare lists accounting for removals and additions
                var minCount = Math.Min(leftList.Count, rightList.Count);
                var rightCollection = this.ChildTags;
                var leftCollection = leftSide.ChildTags;
                for (int i = 0; i < minCount; i++)
                {
                    var rightValue = rightCollection[i];
                    var leftValue = leftCollection[i];
                    if (rightValue.ProcessDifferences(leftValue, seenChanges))
                        changeDetected = true;
                }
                if (rightList.Count > minCount)
                {
                    for (int i = minCount; i < rightList.Count; i++)
                    {
                        var rightValue = rightCollection[i];
                        rightValue.Change = "+";
                    }
                    seenChanges.Add("+");
                    changeDetected = true;
                }
                if (leftList.Count > minCount)
                {
                    for (int i = minCount; i < leftList.Count; i++)
                    {
                        var leftValue = leftCollection[i];
                        leftValue.Change = "-";
                    }
                    seenChanges.Add("-");
                    changeDetected = true;
                }
                if (changeDetected || leftList.Count != rightList.Count)
                {
                    this.Change = "*";
                    leftSide.Change = "*";
                    changeDetected = true;
                }
                break;
            }
            case CompoundTag rightCompound:
            {
                var leftCompound = (CompoundTag)leftTag;
                var sameKeys = leftCompound.Keys.Intersect(rightCompound.Keys).ToList();
                var addedKeys = rightCompound.Keys.Except(sameKeys).ToList();
                var removedKeys = leftCompound.Keys.Except(sameKeys).ToList();
                if (addedKeys.Count > 0)
                {
                    foreach (var added in addedKeys)
                    {
                        var value = this.ByNameLookup[added];
                        value.Change = "+";
                    }
                    seenChanges.Add("+");
                    changeDetected = true;
                }
                if (removedKeys.Count > 0)
                {
                    foreach (var removed in removedKeys)
                    {
                        var value = leftSide.ByNameLookup[removed];
                        value.Change = "-";
                        changeDetected = true;
                    }
                    seenChanges.Add("-");
                    changeDetected = true;
                }
                foreach (var same in sameKeys)
                {
                    var rightValue = this.ByNameLookup[same];
                    var leftValue = leftSide.ByNameLookup[same];
                    if (rightValue.ProcessDifferences(leftValue, seenChanges))
                        changeDetected = true;
                }
                if (changeDetected || leftCompound.Count != rightCompound.Count)
                {
                    this.Change = "*";
                    leftSide.Change = "*";
                    changeDetected = true;
                }
                break;
            }
        }

        return changeDetected;
    }
}
