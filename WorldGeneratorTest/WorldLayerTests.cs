using WorldGenerator.WorldLayers;

namespace WorldGeneratorTest
{
    class WorldLayerTests
    {
        const int SMALL_TEST_SIZE = 50;

        WorldLayer<int> CreateLayer(string name = "test", int size = SMALL_TEST_SIZE)
        {
            return new WorldLayer<int>(name, size);
        }

        [Test]
        public void ArrayCorrectlySized()
        {
            WorldLayer<int> layer = CreateLayer();
            Assert.That(layer.Matrix.GetUpperBound(0) == SMALL_TEST_SIZE - 1);
        }

        [TestCase(""),
         TestCase("   ")]
        public void InvalidNameThrowException(string name)
        {
            Assert.Throws<InvalidLayerSetupException>(() => CreateLayer(name));
        }

        [Test]
        public void ClearToWorks()
        {
            int clearToValue = -666;
            int matrixSize = SMALL_TEST_SIZE;
            WorldLayer<int> layer = new WorldLayer<int>("test", matrixSize);
            Assert.That(layer.Matrix[0, 0] == 0);
            Assert.That(layer.Matrix[matrixSize - 1, matrixSize - 1] == 0);
            layer.ClearTo(clearToValue);
            Assert.That(layer.Matrix[0, 0] == clearToValue);
            Assert.That(layer.Matrix[matrixSize - 1, matrixSize - 1] == clearToValue);
        }

        [Test]
        public void FindLargestWorks()
        {
            int matrixSize = SMALL_TEST_SIZE;
            WorldLayer<int> layer = new WorldLayer<int>("test", matrixSize, -1);
            int testValue = 10;
            Assert.That(layer.FindLargest() == -1, "check initialized correctly");

            layer.Matrix[0, 0] = testValue;
            Assert.That(layer.FindLargest() == testValue, "Check the first element in the matrix");
            testValue += 20;
            layer.Matrix[10, 10] = testValue;
            Assert.That(layer.FindLargest() == testValue, "Check a center point");
            testValue += 100;
            layer.Matrix[matrixSize - 1, matrixSize - 1] = testValue;
            Assert.That(layer.FindLargest() == testValue, "Check the last element in the matrix");
        }

        [Test]
        public void FindSmallestWorks()
        {
            int matrixSize = SMALL_TEST_SIZE;
            WorldLayer<int> layer = new WorldLayer<int>("test", matrixSize, 500);
            Assert.That(layer.FindSmallest() == 500, "check initialized correctly");

            int testValue = 100;
            layer.Matrix[0, 0] = testValue;
            Assert.That(layer.FindSmallest() == testValue, "Check the fist element in the matrix");
            testValue -= 20;
            layer.Matrix[10, 10] = testValue;
            Assert.That(layer.FindSmallest() == testValue, "Check a center point");
            testValue -= 500;
            layer.Matrix[matrixSize - 1, matrixSize - 1] = testValue;
            Assert.That(layer.FindSmallest() == testValue, "Check the last element in the matrix");
        }

        [TestCase(0, 0),
        TestCase(1, 1),
        TestCase(9, 9),
        TestCase(10, 0),
        TestCase(100, 0),
        TestCase(101, 1),
        TestCase(11, 1),
        TestCase(22, 2),
        TestCase(33, 3),
        TestCase(-1, 9),
        TestCase(-10, 0),
        TestCase(-100, 0),
        TestCase(-101, 9),
        TestCase(-201, 9)]
        public void TestMatrixWrap(int x, int expected)
        {
            var layer = new WorldLayer<int>("test", 10);
            var res = layer.ConvertToActualIndex(x);
            Assert.That(res == expected, $"{res} == {expected}");
        }
        //0123456789012345678901234567890123456789--just some number to be able to validate the testcases are setup correctly.

        [TestCase(10),
         TestCase(23),
         TestCase(50)]
        public void TestBinaryIterator(int arraySize)
        {
            var layer = new WorldLayer<int>("test", arraySize);
            List<int> items = [.. layer.GetBinaryEnumerator()];
            Assert.That(items.Count == layer.Size, "Wrong number of items in iterator");
            Assert.That(items[0] == arraySize / 2, "Doesn't start at the right point");
            for( int i = 0; i < arraySize; i ++)
            {
                Assert.That(items.Contains(i), $"Missing expected value {i}");
            }
        }
    }

}
