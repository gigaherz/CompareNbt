using System.Collections;
using System.Collections.Generic;

namespace CompareNbt.Parsing.Tags
{
    public abstract class ArrayTag<TTag, TElement> : Tag<TTag>, IReadOnlyList<TElement>
        where TTag : Tag<TTag>
    {
        /// <summary> Value/payload of this tag (an array of T). Value is stored as-is and is NOT cloned. May not be <c>null</c>. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        public abstract TElement[] Value { get; set; }

        TElement IReadOnlyList<TElement>.this[int index] => Value[index];

        public int Count => Value.Length;

        /// <inheritdoc/>
        public IEnumerator<TElement> GetEnumerator() =>
            ((IEnumerable<TElement>)Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Value.GetEnumerator();
    }
}