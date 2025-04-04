using System.Drawing;
using WorldGenerator.Generators;

namespace WorldGeneratorTest.Generators
{
    [TestFixture]
    public class TopographicalGeneratorTest
    {
        const int KNOWN_SEED = 999_983;
        const int INIT_VALUE = -100;
        const int VARIANCE = 10;
        const int MAX_HEIGHT = 50;
        const int SMALL_TEST_SIZE = 50;


        private TopographicalGenerator _generator;

        [SetUp]
        public void Setup()
        {
            _generator = new TopographicalGenerator(KNOWN_SEED,SMALL_TEST_SIZE, MAX_HEIGHT, VARIANCE, INIT_VALUE);
        }


        [TestCase(10, 20, 5 , 12.5),
         TestCase(10,-10, 5, -2.5),
         TestCase(20,22,10, 21),
         TestCase(-22,-24, 10, -23),
         TestCase(50,50,20, 50),
         TestCase(-50, -50, 0, -50)]
        public void CalcNormalizedPos_Tests(int v1, int v2, decimal randomNumber, decimal expected)
        {
            var gen = new TopoGeneratorMoq(KNOWN_SEED, 1, 50, 10);
            gen.GivenNextRandNumberIs(randomNumber);
            var value = gen.CalcNormalizedPos(v1, v2);

            Assert.That(value == expected, $"with rand value ={randomNumber} expected {expected} but is {value}");
        }

        [TestCase(10, 4, -6),
         TestCase(50, 50, 0),
         TestCase(10, 18, 8)]
        public void GetRandomHeightTest(int height, int randomNumber, decimal expected)
        {
            var gen = new TopoGeneratorMoq(KNOWN_SEED, 1, height, 1);
            gen.GivenNextRandNumberIs(randomNumber);
            var value = gen.GetRandomHeight();

            Assert.That(value == expected, $"{randomNumber} should be changed to {expected} but is {value}");
        }


        [TestCase(100, 49, 51, 50),
        TestCase(100, 20, 30, 50)]
        public void CalcValueTest(decimal random, int v1, int v2, decimal expected)
        {
            //these tests are questionable it is making sure that the coverage is there
            //but we wind up using the same random number in both cases so it's brittle.
            //might need to rework a little later.
            var gen = new TopoGeneratorMoq(KNOWN_SEED, 1, 50, 1);
            gen.GivenNextRandNumberIs(random);

            var value = gen.CalcValue(v1, v2);
            Assert.That(value == expected, $"Expected {expected} but is {value}");
        }

        [Test]
        public void GenerateIndependentLineTest()
        {
            int yPosLine = 10;

            _generator.GenerateIndependentLineIfNeeded(yPosLine); // just generate a line at 10

            for (int xPos = 0; xPos < _generator.WorldSize; xPos++)
            {
                for (int yPos = 0; yPos < _generator.WorldSize; yPos++)
                {
                    var value = _generator.Layer.GetValueAt(new Point(xPos, yPos));
                    if (yPos == yPosLine)
                    {
                        Assert.That(value != INIT_VALUE, "A value wasn't initialized in the line");
                    }
                    else
                    {
                        Assert.That(value == INIT_VALUE, "A value was initialized outside of the line");
                    }
                }
            }
        }

        [Test]
        public void TopoGenerator_values_InRange()
        {
            var layer = _generator.Generate();

            var largestValue = layer.FindLargest();
            var smallestValue = layer.FindSmallest();

            Assert.That(largestValue <= MAX_HEIGHT + VARIANCE, $"value {largestValue} out of legal range");
            Assert.That(smallestValue >= -(MAX_HEIGHT + VARIANCE), $"value {largestValue} out of legal range");
        }

    }
}
