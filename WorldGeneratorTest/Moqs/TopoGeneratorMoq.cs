using WorldGenerator.Generators;

internal class TopoGeneratorMoq : TopographicalGenerator
{
    decimal _nextRandomNumber = 0;

    public TopoGeneratorMoq(int seed, int worldSize, int maxHeight, int deviation, decimal defaultValue = -1) : base(seed, worldSize, maxHeight, deviation, defaultValue)
    {
    }

    public void GivenNextRandNumberIs(decimal value) => _nextRandomNumber = value;

    protected override decimal CalcValue()
    {
        return _nextRandomNumber;
    }
}
