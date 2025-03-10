using WorldGenerator.WorldLayers;
/// <summary>
/// This is a class that allows me to make smaller than allowed layers so that tests can be more easily run.
/// </summary>
internal class IngoreSizeMatrix<T> : WorldLayer<T> where T : IComparable
{
    public IngoreSizeMatrix(string name, int size) : base(name, size) { }
    public override int GetMinViableSize() => 1;
}

internal class IngoreSizeTopoMatrix : TopographicalLayer
{
    public const int SMALL_TEST_SIZE = 50;

    public IngoreSizeTopoMatrix(int size, decimal initialValue = 0) : base(size, initialValue)
    {
    }

    public override int GetMinViableSize() => 1;
}