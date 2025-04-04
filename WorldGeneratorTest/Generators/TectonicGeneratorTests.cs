using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldGenerator.Generators;
using WorldGenerator.Renderers;

namespace WorldGeneratorTest.Generators
{
    public class TectonicGeneratorTests
    {
        [Test]
        public void Test()
        {
            var gen = new TectonicGenerator(12321, 1000, 50);
            var layer = gen.Generate();

            var s = layer.ToString();
            var  histo = gen.HistoGraph(180);
            var s2 = histo.ToString();
            BasicTopoRenderer rend = new BasicTopoRenderer(histo);
            var image = rend.RenderHist();

            image.Write("C:\\temp\\histo.jpg");
            Assert.That(false);
        }
    }
}
