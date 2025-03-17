using WorldGenerator.Generators;
using WorldGenerator.Renderers;
using WorldGenerator.WorldLayers;
using static System.Net.Mime.MediaTypeNames;

namespace WorldGeneratorTest.Renderers
{
    [TestFixture]
    public class BasicTopoRendererTests
    {

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

            var renderer = new BasicTopoRenderer(matrix);

            matrix.ClearTo(setTo);
            //renders and image
            var image = renderer.Render();
            var pixels = image.GetPixelsUnsafe();
            var baseColor = pixels.GetPixel(0,0);
            foreach (var color in pixels)
            {
                Assert.That(color.Equals(baseColor), "unexpected color found");
            }


            //image.Write(@"C:\temp\Test.jpg");
            ////that is wholely that value
            //Assert.That(false, "image[x,y]=expected color");


        }

        [Test]
        public void TestBigMatrix()
        {

            var gen = new TopographicalGenerator(12312, 2000, 200, 20);
            var matrix = gen.Generate();

            var renderer = new BasicTopoRenderer(matrix);
            var image = renderer.Render();
            image.Write(@"C:\temp\Test.jpg");

            //not actually a test[yet] wanted to be able to see what the topolayer looks like ... right now I need to tweak it it's not smooth yet.


        }


    }
}
