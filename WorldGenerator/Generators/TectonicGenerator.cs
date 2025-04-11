using System.Drawing;
using WorldGenerator.WorldLayers;

namespace WorldGenerator.Generators
{
    public class TectonicGenerator : GeneratorBase
    {
        private readonly TectonicLayer _layer;
        private readonly Random _rand;

        public TectonicGenerator(int seed, int worldSize, int deviation) : base(seed, worldSize, 100, deviation)
        {
            _layer = new TectonicLayer(worldSize);
            _rand = new Random(seed);
        }

        /// <summary>
        /// Generates the map.
        /// </summary>
        /// <returns></returns>
        public override dynamic Generate()
        {
            //TODO:  I think that if I get rid of the random initialize and redo the walking map to just skip values that are already set
            //this will be alot closer to what I want it to be and faster.   But want to save this off first in any case.
            RandomInitialize();
            GenerateWalkingRandomMap();
            return _layer;

        }

        private void GenerateWalkingRandomMap()
        {
            for (int x = 0; x < WorldSize; x++)
            {
                double direction = _rand.Next(360);
                double force = (double)((_rand.Next(Deviation)) * 0.002);
                Point p = new Point(_rand.Next(WorldSize), _rand.Next(WorldSize));
                int count = WorldSize * 6;

                while (count-- > 0)
                {
                    if (_rand.Next(20) == 1)
                    {
                       direction = _rand.Next(360);
                       force = (double)((_rand.Next(Deviation)) * 0.002);
                    }
                    p = MoveRandomDirection(p);

                    _layer.SetValueAt(p, new TectonicVector( direction, force ));
                }
            }
        }

        //TODO another copy paste that needs to be cleaned up.  move this to the base layer class.[and write tests].
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

        /// <summary>
        /// randomize the layer
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void RandomInitialize()
        {
            for(int x = 0; x<WorldSize; x ++)
            {
                for(int y =0; y<WorldSize; y++)
                {
                    var vector = new TectonicVector(_rand.Next(360), (double)_rand.Next(Deviation) / Deviation+0.000001);
                    _layer.SetValueAt(new Point(x, y), vector);
                }
            }
        }

    }
}
