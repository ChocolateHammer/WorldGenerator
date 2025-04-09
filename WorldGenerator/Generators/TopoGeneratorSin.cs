
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

        public TopoGeneratorSin(int seed, int worldSize, int maxHeight, int deviation, decimal defaultValue = -1, Random rand = null) : base(seed, worldSize, maxHeight, deviation)
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

        internal decimal GetRandomDeviation() => _rand.Next(Deviation * 2) - Deviation;

        decimal GetIncrementalHeightRandomizedOffset(decimal currentHeight)
        {
            var nextOffset = GetRandomDeviation();
            if (Math.Abs(nextOffset + currentHeight) > MaxHeight)
            {
                nextOffset = -nextOffset;
            }
            return nextOffset;
        }

        public void InitalRandomize()
        {
            decimal height = GetRandomHeight();
            for (int y = 0; y < (WorldSize+1) / 2; y++)
            {
                for (int x = 0; x < (WorldSize + 1) / 2; x++)
                {
                    if( _rand.Next(11) == 1)
                    {
                        height += GetIncrementalHeightRandomizedOffset(height);
                    }

                    _layer.SetValueAt(x, y, height);
                    _layer.SetValueAt(-x, y, height);
                    _layer.SetValueAt(x, -y, height);
                    _layer.SetValueAt(-x, -y, height);
                }
            }
        }

        private void GenerateWalkingRandomMap()
        {
            for ( int x = 0; x< WorldSize; x++)
            {
                decimal height = GetRandomHeight();
                Point p = new Point(_rand.Next(WorldSize), _rand.Next(WorldSize));
                int count = WorldSize*6;
                //adding a list of points we already hit makes this generate a much cleaner world but it's order n expensive so just doing this.
                //it's fine for an initial world the tectonic and weather erode patterns will make it work the way it should move this into something
                // that is more what you expect.

                while(count -- > 0)
                {
                    if( _rand.Next(11) == 1)
                    {
                        height += GetIncrementalHeightRandomizedOffset(height);
                    }
                    p = MoveRandomDirection(p);
          
                    _layer.SetValueToAverage(p, height);
                }

            }
          
        }

        private Point MoveRandomDirection(Point p)
        {
            switch (_rand.Next(4))
            {
                case 0: //move right
                    p.Offset(1, 0);
                    break;
                case 1: //move left
                    p.Offset(-1, 0);
                    break;
                case 2: //move up
                    p.Offset(0, -1);
                    break;
                default:
                    p = new Point(_rand.Next(WorldSize), _rand.Next(WorldSize));
                    break;
                case 3: //move down
                    p.Offset(0, 1);
                    break;
            }

            return p;
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

        public TopographicalLayer GenerateRandomInit()
        {
            InitalRandomize();

            return _layer;
        }

        internal void GenerateSignWave( int startingIteration, int ampitude)
        {
            int iterations = _rand.Next(startingIteration);
            int offsetCelling = Deviation / startingIteration;
            for (int y = 0; y < (WorldSize+1) / 2; y++)
            {
                if (_rand.Next(5) == 1)
                {
                    iterations += _rand.Next(5) - 2;
                    if (iterations > 9) iterations--;
                    if (iterations <= 1) iterations++;
                }

                int offset = _rand.Next(offsetCelling / 2) - offsetCelling;
                //need to do something better with the offset but for now just doing this
                var calc = new SinWaveCalculator(ampitude, iterations, offset, (int)WorldSize);
                for (int x = 0; x < WorldSize; x++)
                {
                    _layer.SetValueToAverage(new Point(x, y), (decimal)calc.CalcValue(x));
                    _layer.SetValueToAverage(new Point(x, -y), (decimal)calc.CalcValue(x));

                }
            }
        }

        public TopographicalLayer GenerateSin()
        {
            //InitalRandomize();


            GenerateSignWave(10, MaxHeight);
            //Add_Volcanos_And_Meteors();


            return _layer;
        }

        private void Add_Volcanos_And_Meteors()
        {
            int passes = _rand.Next(Deviation)+Deviation/2;
            while (passes-- > 0)
            {
                int radius = _rand.Next((int)WorldSize / 10)+1;
                int x = _rand.Next((int)WorldSize);
                int y = _rand.Next((int)WorldSize);
                decimal h = GetRandomHeight();
                h /= radius;

                for (double angle = 0; angle < 90; angle++)
                {
                    var yOffset = Math.Sin(angle) * radius;
                    var xOffset = Math.Cos(angle) * radius;
                    for (int xPos = 0; xPos <= xOffset; xPos++)
                    {
                        for (int yPos = 0; yPos <= yOffset; yPos++)
                        {
                            //at the center we get the whole height differential h
                            //as we get further out the difference get's less and less until it's h/radius at the end    
                            var useHeight = (radius - Math.Max(xPos, yPos))*h;
                            _layer.SetValueToAverage(new Point(x + xPos, y + yPos), useHeight);
                            _layer.SetValueToAverage(new Point(x - xPos, y + yPos), useHeight);
                            _layer.SetValueToAverage(new Point(x + xPos, y - yPos), useHeight);
                            _layer.SetValueToAverage(new Point(x - xPos, y - yPos), useHeight);
                        }
                    }
                }
            }
        }

        public override dynamic Generate()
        {
            GenerateWalkingRandomMap();
            for (int y = 0; y < WorldSize; y++)
            {
                for (int x = 0; x < WorldSize; x++)
                {
                    var v = _layer.GetValueAt(x, y);
                    if (v == _layer.InitialValue)
                    {
                        _layer.SetValueAt(x, y, -(MaxHeight));
                    }           
                }
            }
            return _layer;
        }
    }
}
