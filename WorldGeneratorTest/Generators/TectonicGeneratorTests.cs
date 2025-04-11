using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldGenerator;
using WorldGenerator.Generators;
using WorldGenerator.Renderers;
using WorldGenerator.WorldLayers;

namespace WorldGeneratorTest.Generators
{
    [TestFixture]
    public class TectonicGeneratorTests
    {
        //[Test]
        //public void Test()
        //{
        //    var gen = new TectonicGenerator(12321, 1000, 50);
        //    var layer = gen.Generate();

        //    var s = layer.ToString();
        //    var  histo = gen.HistoGraph(180);
        //    var s2 = histo.ToString();
        //    BasicTopoRenderer rend = new BasicTopoRenderer(histo);
        //    var image = rend.RenderHist();

        //    image.Write("C:\\temp\\histo.jpg");
        //    Assert.That(false);
        //}

        [Test]
        public void Test()
        {
            var man = new WorldManager(1231, 2000,  100, 10);
            var topo = man.Layers[TopographicalLayer.TopoName];
            var tectonic = man.Layers[TectonicLayer.TectonicName];//ToDo change to name of....maybe just type....
            PrintTopo(topo,"Init.Topo.jpg", false);
            PrintTopo(man.CreateTechHistoGraph(), "Init.Tectonic.jpg", true);
            man.ApplyFirstTechCyles();
            PrintTopo(topo, "After.Topo.jpg", false);
            PrintTopo(man.CreateTechHistoGraph(), "After.Tectonic.jpg", true);
            Assert.That(false);
        }
        

        private void PrintTopo( TopographicalLayer topo, string fileName, bool isHist)
        {
            BasicTopoRenderer rend = new BasicTopoRenderer(topo);
            var image = (isHist) ? rend.RenderHist(): rend.Render();

            image.Write($"C:\\temp\\{fileName}");

        }
    }
}
