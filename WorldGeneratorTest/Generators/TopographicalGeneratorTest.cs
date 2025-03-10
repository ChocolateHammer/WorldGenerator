using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WorldGenerator.Generators;
using WorldGenerator.WorldLayers;

namespace WorldGeneratorTest.Generators
{
    [TestFixture]
    public class TopographicalGeneratorTest
    {
        const int KNOWN_SEED = 999_983;
        const int INIT_VALUE = -100;
        const int VARIANCE = 10;
        const int MAX_HEIGHT = 50;


        private TopographicalGenerator _generator;

        [SetUp]
        public void Setup()
        {
            _generator = new TopographicalGenerator(KNOWN_SEED,
                IngoreSizeTopoMatrix.SMALL_TEST_SIZE, 
                MAX_HEIGHT, VARIANCE, INIT_VALUE);
        }


        [TestCase(20, 20, 20, 20),
        TestCase(30, 20, 49, 30),
        TestCase(50, 15, 25, 40),
        TestCase(15, 30, 9, 15),
        TestCase(-5, -1, -10, -5),
        TestCase(-15, -20, -9, -15),
        TestCase(10, 20, 30, 22.5),
        TestCase(10, 39, 20, 24.75),
        TestCase(-10, -20, -40, -25),
        TestCase(-50, -20, -20, -40)]
        public void CalcValueTest(int randomNumber, int v1, int v2, decimal expected)
        {
            var gen = new TopoGeneratorMoq(KNOWN_SEED, 200, 50, 10);
            gen.GivenNextRandNumberIs(randomNumber);
            var value = gen.CalcValue(v1, v2);

            Assert.That(value == expected, $"{randomNumber} should be changed to {expected} but is {value}");
        }

        [Test]
        public void GenerateIndependentLineTest()
        {
            int yPosLine = 10;

            _generator.GenerateIndependentLine(yPosLine); // just generate a line at 10

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
