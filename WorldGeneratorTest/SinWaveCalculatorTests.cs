using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldGenerator.Generators;


namespace WorldGeneratorTest
{
    [TestFixture]
    public class SinWaveCalculatorTests
    {

        [Test]
        public void Test()
        {
            int size = 50;
            var items = new List<double>();
            var calc = new SinWaveCalculator( 10, 3, 0, size);
            for( int x = 1; x < size; x ++)
            {
                items.Add(calc.CalcValue(x));
            }
            Assert.That(items.Count == 0);
        }
    }
}
