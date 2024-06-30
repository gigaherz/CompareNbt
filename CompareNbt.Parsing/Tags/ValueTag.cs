namespace CompareNbt.Parsing.Tags;

public interface ValueTag
{
    object? RawValue { get; }
}

public abstract class ValueTag<TTag, TValue> : Tag<TTag>, ValueTag
    where TTag : Tag<TTag>
{
    public abstract TValue Value { get; set; }
    
    public object? RawValue => Value;
}