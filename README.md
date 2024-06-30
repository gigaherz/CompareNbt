# CompareNbt
A small crossplatform tool to visualize differences in Minecraft NBT files.

NBT file parsing based on code from [fNbt](https://github.com/mstefarov/fNbt) with heavy modifications. See [CompareNbt.Parsing/LICENSE.md](CompareNbt.Parsing/LICENSE.md) for license details.

Supports any NBT flavour type supported by fNbt, unless I broke it in my refactors (please tell me if I did!).

## TODO

- Detect partial matches in lists and arrays, and show the differences as removals/additions.
- Allow editing and "copy from left" / "copy from right" actions.