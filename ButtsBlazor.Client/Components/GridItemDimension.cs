namespace ButtsBlazor.Client.Components;

[Flags]
public enum GridItemSize
{
    OneByOne = OneRow | OneCol,
    TwoByOne = TwoRows | OneCol,
    OneByTwo = OneRow | TwoCols,
    TwoByTwo = TwoRows | TwoCols,
    OneRow = 1 << 4,
    TwoRows = 2 << 4,
    OneCol = 1,
    TwoCols = 2,
}

[Flags]
public enum GridItemDimension
{
    One=1,
    Two=2
}