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
            //I thought that this would be a super easy way to get 
            // a visual image of the matrix but it turns out he system.drawing stuff 
            //has no counterpart in .core.     I'm going to have to use the MAGICK nugets
            //but haven't worked with them as of yet.  Will push on with that when I pick
            //this back up.
            //given a matrix with a given value 
            var matrix = new TopographicalLayer(10);

            matrix.ClearTo(setTo);
            //renders and image
            var image = _render.Render();
            //that is wholely that value
            Assert.That(false, "image[x,y]=expected color");


        }


    }
}
