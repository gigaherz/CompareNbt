using CompareNbt.Parsing.Tags;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareNbt.Parsing
{
    internal static class NbtStructuralChecks
    {
        public static void ThrowIfHasParent(Tag element)
        {
            if (element.Parent != null)
            {
                throw new ArgumentException("A tag may only be added to one collection at a time.");
            }
        }

        public static void ThrowIfCircularDependency(Tag element, Tag? container)
        {
            while (container != null)
            {
                if (element == container)
                {
                    throw new ArgumentException("A tag may not be added to itself, or to a child of itself.");
                }
                container = container.Parent;
            }
        }
    }
}
