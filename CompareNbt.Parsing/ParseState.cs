namespace CompareNbt.Parsing;

internal enum ParseState
{
    AtStreamBeginning,
    AtCompoundBeginning,
    InCompound,
    AtCompoundEnd,
    AtListBeginning,
    InList,
    AtStreamEnd,
    Error
}
