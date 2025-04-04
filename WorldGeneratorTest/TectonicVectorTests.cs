using Newtonsoft.Json.Linq;
using WorldGenerator.WorldLayers;

namespace WorldGeneratorTest
{
    public class TectonicVectorTests
    {
        [TestCase( 90, 90, 20, true),
        TestCase( 90, 100, 20, true),
        TestCase( 90, 101, 20, false),
        TestCase( 90+360, 101, 20, false),
        TestCase(90, 100+360, 40, true),
        TestCase( 0, 15, 40, true),
        TestCase( 0, 345, 40, true ),
        TestCase( 0, 335, 40, false),
        TestCase( 360, 1, 10, true),
        TestCase( 360, 8, 10, false)]
        public  void  DirectionInRangeTest(double direction1, double direction2, double degree, bool expectedValue)
        {
            var vector = new TectonicVector(direction1, 10);
            Assert.That(vector.DirectionInRange(degree, direction2) == expectedValue, $"{direction1} : {direction2} : {degree} doesn't yield expected result");
        }

        [TestCase( 10, 10),
         TestCase( 360, 0),
         TestCase( 0,0),
         TestCase(361, 1),
         TestCase(-10, 350),
         TestCase(770, 50),
         TestCase(-370,350 )]
        public void NormalizeDegree_Test(double value, double normalizedValue)
        {
            var vector = new TectonicVector(value, 10);
            Assert.That(vector.Direction == normalizedValue, $"A direction of {value} yields invalid {vector.Direction}");
        }

        [TestCase(0,0, "(D:0.00,F:0.00)"),
         TestCase(10, 20, "(D:10.00,F:20.00)"),
         TestCase(10.12345123, 20.1231212312, "(D:10.12,F:20.12)")]
        public void ToString_Test(double direction, double force, string expectedResult )
        {
            var vector = new TectonicVector(direction, force);

            Assert.That(vector.ToString().CompareTo(expectedResult) == 0, $"Got {vector.ToString()} rather than {expectedResult}");
        }

        [TestCase(0, 1, 2, 1),
         TestCase(-10, 10, 30, 10),
         TestCase(350, 10, 9, 3)]
        public void AverageOut_DirectionOnly_Test( double d1, double d2, double d3,  double expectedD )
        {
            var v1 = new TectonicVector(d1, 1);
            var v2 = new TectonicVector(d2, 1);
            var v3 = new TectonicVector(d3, 1);

            v2.AverageOut(v1, v3);
            Assert.That(v2.Force == 1);
            Assert.That(v2.Direction == expectedD);

        }

        [TestCase(1, 5, 15, 3),
        TestCase(5, 1, 75, 3),
        TestCase(10, 10, 45, 10),
        TestCase(3, 2, 54, 2.5) ]
        public void AverageOut_ForceOnly_Test(double f1, double f2, double expectedD, double expectedF)
        {
            var v1 = new TectonicVector(90,f1);
            var v2 = new TectonicVector(0, f2);

            v2.AverageOut(new TectonicVector[] { v1});
            Assert.That(v2.Force.Equals(expectedF), $"Ave of {f1}, {f2} == {v2.Force}");
            Assert.That(v2.Direction == expectedD, $"Ave of {f1}, {f2} == {v2.Direction} rather than{expectedD}");

        }

        [TestCase(1, 5, 345, 3),
        TestCase(5, 1, 360-75, 3),
        TestCase(10, 10, 315, 10),
        TestCase(3, 2, 360-54, 2.5)]
        public void AverageOut_ForceOnly_270_Test(double f1, double f2, double expectedD, double expectedF)
        {
            var v1 = new TectonicVector(270, f1);
            var v2 = new TectonicVector(0, f2);

            v2.AverageOut(new TectonicVector[] { v1 });
            Assert.That(v2.Force.Equals(expectedF), $"Ave of {f1}, {f2} == {v2.Force}");
            Assert.That(v2.Direction == expectedD, $"Ave of {f1}, {f2} == {v2.Direction} rather than{expectedD}");

        }
    }
}
