using CompareNbt.Parsing.Tags;

namespace CompareNbt.Parsing;

/// <summary> Provides options for reading and writing NBT files and network streams in different versions of the NBT format. </summary>
public sealed class NbtFlavor
{
    /// <summary>
    /// Creates a new NbtFlavor with specified custom options.
    /// </summary>
    /// <param name="bigEndian">Whether numbers are encoded in BigEndian (Java), instead of LittleEndian (Bedrock) byte order. Default is true. </param>
    /// <param name="allowListRootTag">Whether lists are allowed to be root tags.</param>
    /// <param name="allowIntArray">Whether to allow <see cref="IntArrayTag"/> tags (1.2.1+). Default is true. </param>
    /// <param name="allowLongArray">Whether to allow <see cref="LongArrayTag"/> tags (1.12+). Default is true. </param>
    /// <param name="unnamedRootTag">Whether to use unnamed compound tags for root tags.</param>
    /// <param name="useVarInt">Whether to use VarInts with ZigZag encoding to store most numbers.</param>
    public NbtFlavor(bool bigEndian = true, bool allowListRootTag = false, bool allowIntArray = true, bool allowLongArray = true, bool unnamedRootTag = false, bool useVarInt = false)
    {
        BigEndian = bigEndian;
        AllowListRootTag = allowListRootTag;
        AllowIntArray = allowIntArray;
        AllowLongArray = allowLongArray;
        UnnamedRootTag = unnamedRootTag;
        UseVarInt = useVarInt;
    }

    /// <summary> Whether numbers are encoded in BigEndian (Java) or LittleEndian (Bedrock) byte order. Default is true. </summary>
    public bool BigEndian { get; private set; }

    /// <summary> Whether lists are allowed to be root tags. </summary>
    public bool AllowListRootTag { get; private set; }

    /// <summary> Whether to allow <see cref="IntArrayTag"/> tags (1.2.1+). Default is true. </summary>
    public bool AllowIntArray { get; private set; }

    /// <summary> Whether to allow <see cref="LongArrayTag"/> tags (1.12+). Default is true. </summary>
    public bool AllowLongArray { get; private set; }

    /// <summary> Whether to use unnamed compound tags for root tags. </summary>
    public bool UnnamedRootTag { get; private set; }

    /// <summary> Whether to use VarInts with ZigZag encoding to store most numbers. </summary>
    public bool UseVarInt { get; private set; }

    /// <summary>
    /// Appropriate options for reading and writing NBT files and network streams in the original format,
    /// used for Indev, Infdev, Alpha, Beta, and Java Editions up through version 1.1.
    /// </summary>
    public static NbtFlavor JavaLegacy { get; } = new()
    {
        AllowIntArray = false,
        AllowLongArray = false,
    };

    /// <summary>
    /// Appropriate options for reading and writing NBT files and network streams with IntArray tags,
    /// introduced as part of the Anvil format in Java Edition 1.2.1 and used until 1.12.
    /// </summary>
    public static NbtFlavor JavaAnvil { get; } = new()
    {
        AllowLongArray = false,
    };

    /// <summary>
    /// Appropriate options for reading and writing NBT files with IntArray and LongArray tags,
    /// introduced by Java Edition 1.12 (World of Color) and still used in files today.
    /// Also works for network streams as used by Java Edition 1.12 through 1.20.1, but not later.
    /// </summary>
    public static NbtFlavor JavaWorldOfColor { get; } = new();

    /// <summary>
    /// Appropriate options for reading and writing NBT network streams as used by Java Edition 1.20.2 and later.
    /// Almost identical to <see cref="JavaWorldOfColor"/> but usses unnamed compound tags for roots,
    /// which was not allowed other flavor of NBT. Should not be used for loading/saving files.
    /// </summary>
    public static NbtFlavor JavaNetwork { get; } = new()
    {
        UnnamedRootTag = true,
    };

    /// <summary>
    /// Appropriate options for reading and writing NBT files and network streams in the Bedrock Edition format.
    /// Supports IntArrays and LongArrays, allows lists in addition to compounds as root tags, and encodes
    /// numbers using VarInts.
    /// </summary>
    public static NbtFlavor Bedrock { get; } = new()
    {
        BigEndian = false,
        AllowListRootTag = true,
        UseVarInt = true
    };

    /// <summary>
    /// The default options for reading and writing NBT files and network streams, set to <see cref="JavaWorldOfColor"/>.
    /// </summary>
    public static NbtFlavor Default { get; set; } = new();
}
