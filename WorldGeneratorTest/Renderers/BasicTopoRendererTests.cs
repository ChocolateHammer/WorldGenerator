using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldGenerator.Renderers;
using WorldGenerator.WorldLayers;

namespace WorldGeneratorTest.Renderers
{
    [TestFixture]
    public class BasicTopoRendererTests
    {
        BasicTopoRenderer _render;
        [SetUp]
        public void Setup()
        {
            _render = new BasicTopoRenderer();
        }

        [TestCase(-40),
            TestCase(0),
            TestCase(1),
            TestCase(40)]
        public void TestSolidColor(int setTo)
        {
            //given a matrix with a given value 
            var matrix = new IngoreSizeMatrix<decimal>("fake", 10);
            var topoLayer = new Mock<TopographicalLayer>();
            topoLayer.Setup(m => m.GetMinViableSize()).Returns(10);

            matrix.ClearTo(setTo);
            //renders and image
             
            //that is wholely that value


        }


    }
}
