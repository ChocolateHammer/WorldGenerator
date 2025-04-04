
using System.Drawing;
using System.Reflection.Emit;
using WorldGenerator.WorldLayers;

namespace WorldGenerator.Generators
{

    public class TopoGeneratorSin : GeneratorBase
    {
        private Random _rand;
        private TopographicalLayer _layer;

        public decimal DefaultValue { get; }

        public TopoGeneratorSin(int seed, uint worldSize, int maxHeight, int deviation, decimal defaultValue = -1, Random rand = null) : base(seed, worldSize, maxHeight, deviation)
        {
            //The map is meant to be seamless meaning if you wrapped it then the edges line
            //up with the far edge.  This algorithm should ensure that by starting at the edges
            //and working toward the center
            _rand = new Random(seed);
            _layer = new TopographicalLayer(WorldSize, defaultValue);
            DefaultValue = defaultValue;
       
        }

        /// <summary>
        /// Gets a random height.
        /// </summary>
        /// <returns></returns>
        internal decimal GetRandomHeight() => _rand.Next(MaxHeight * 2) - MaxHeight;

        public void InitalRandomize()
        {
            for (int y = 0; y < WorldSize/2+1; y++)
            {
                for (int x = 0; x < WorldSize/2+1; x++)
                {
                    decimal height = GetRandomHeight();
                    _layer.SetValueAt(x, y, height );
                    _layer.SetValueAt(-x, y, height);
                    _layer.SetValueAt(x, -y, height);
                    _layer.SetValueAt(-x, -y, height);
                }
            }

        }

        public TopographicalLayer GenerateSimpleSign()
        {
            InitalRandomize();

            var calc = new SinWaveCalculator(MaxHeight, 10, 0, (int)WorldSize);
            for (int y = 0; y < WorldSize; y++)
            {
                for (int x = 0; x < WorldSize; x++)
                {
                    _layer.SetValueAt(new Point(x, y), (decimal)calc.CalcValue(x));
                }
            }

            return _layer;
        }

        public TopographicalLayer Generate()
        {
            InitalRandomize();
      
            int iterations = _rand.Next(10);
            int offsetCelling = Deviation / 10;
            for (int y = 0; y < WorldSize; y++)
            {
                iterations += _rand.Next(5) - 2;
                if (iterations > 9 ) iterations--;
                if (iterations <= 1) iterations++;

                int offset = _rand.Next(offsetCelling / 2) - offsetCelling;
                //need to do something better with the offset but for now just doing this
                var calc = new SinWaveCalculator(MaxHeight, iterations, offset, (int)WorldSize);
                for (int x = 0; x < WorldSize; x++)
                {
                    _layer.SetValueAveAt(new Point(x, y), (decimal)calc.CalcValue(x));
                }
            }
            //for (int y = 0; y < WorldSize*3; y++)
            //{
            //    iterations += _rand.Next(5) - 2;
            //    if (iterations > 9) iterations--;
            //    if (iterations <= 1) iterations++;

            //    int offset = _rand.Next(offsetCelling / 2) - offsetCelling;
            //    //need to do something better with the offset but for now just doing this
            //    var calc = new SinWaveCalculator(MaxHeight, iterations, offset, (int)WorldSize);
            //    for (int x = 0; x < WorldSize; x++)
            //    {
            //        _layer.SetValueAveAt(new System.Drawing.Point(x, y), (decimal)calc.CalcValue(x));
            //    }
            //}
            //for (int x = 0; x < WorldSize; x++)
            //{
            //    iterations += _rand.Next(5) - 2;
            //    if (iterations > 9) iterations--;
            //    if (iterations <= 1) iterations++;

            //    int offset = _rand.Next(offsetCelling / 2) - offsetCelling;
            //    //need to do something better with the offset but for now just doing this
            //    var calc = new SinWaveCalculator(MaxHeight, iterations, offset, (int)WorldSize);
            //    for (int y = 0; y < WorldSize; y++)
            //    { 
            //        _layer.SetValueAveAt(new System.Drawing.Point(x, y), (decimal)calc.CalcValue(y)/2 + dev/2 );
            //    }
            //}
            return _layer;
        }




    }
}
