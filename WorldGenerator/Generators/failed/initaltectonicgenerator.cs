using System.Drawing;
using WorldGenerator.WorldLayers;

namespace WorldGenerator.Generators.Failed
{

    /// <summary>
    /// first stab at a tectonic generator....   Didn't works as I thought it would and it was really slow to generate
    /// </summary>
    public class TectonicGenerator : GeneratorBase
    {
        private readonly TectonicLayer _layer;
        private readonly Random _rand;

        public TectonicGenerator(int seed, int worldSize, int deviation) : base(seed, worldSize, 100, deviation)
        {
            _layer = new TectonicLayer(worldSize);
            _rand = new Random(seed);
        }

        public override dynamic Generate()
        {
            RandomInitialize();
            RemoveAnomalies(90);
            for (double d = 20; d > 130; d += 20)
            {

                DoLinearSmooth(d, true);
                DoLinearSmooth(d, false);
                break;
            }

            Smooth(90);
            return _layer;

        }

        /// <summary>
        /// randomize the layer
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void RandomInitialize()
        {
            for (int x = 0; x < WorldSize; x++)
            {
                for (int y = 0; y < WorldSize; y++)
                {
                    var vector = new TectonicVector(_rand.Next(360), (double)_rand.Next(Deviation) / Deviation + 0.000001);
                    _layer.SetValueAt(new Point(x, y), vector);
                }
            }
        }


        /// <summary>
        /// if everything surrounding a point is moving in generally the same direct but the center node is
        /// going into a different way. Or the vector has no force then re-calc the value to be the average of
        /// the surrounding areas.
        /// </summary>
        /// <param name="degree"></param>
        private void RemoveAnomalies(double degree)
        {
            for (int y = 0; y < WorldSize; y++)
            {
                for (int x = 0; x < WorldSize; x++)
                {
                    //from the matrix below
                    ///r1x1,r1x2,r1x3
                    ///r2x1,curr,r2x3
                    ///r3x1,r3x2,r3x3
                    var curr = _layer.GetValueAt(x, y);
                    var surroundingVectors = _layer.GetSurroundingItems(x, y);

                    if (curr.VectorsWithinDegreeVariance(degree, surroundingVectors))
                    {
                        curr.AverageOut(surroundingVectors);
                        _layer.SetValueAt(new Point(x, y), curr);
                    }
                }
            }
        }

        /// <summary>
        /// if the 
        /// </summary>
        /// <param name="smoothingAngle"></param>
        private void Smooth(double smoothingAngle)
        {
            for (int y = 0; y < WorldSize; y++)
            {
                for (int x = 0; x < WorldSize; x++)
                {
                    var curr = _layer.GetValueAt(x, y);
                    var surroundingVectors = _layer.GetSurroundingItems(x, y);
                    curr.AverageOut(surroundingVectors);
                    _layer.SetValueAt(new Point(x, y), curr);
                }
            }
        }

        public void DoLinearSmooth(double angle, bool doHorizontal)
        {
            //walk the array looking for a break.
            for (int y = 0; y < WorldSize; y++)
            {
                for (int x = 0; x < WorldSize; x++)
                {
                    Point p = (doHorizontal) ? new Point(x, y) : new Point(y, x);
                    var start = _layer.GetValueAt(p);
                    int run = 1;
                    double sumForce = start.Force;

                    var items = new List<TectonicVector>();
                    do
                    {
                        Point p1 = (doHorizontal) ? new Point(x + run++, y) : new Point(y, x + run);
                        var nextElement = _layer.GetValueAt(p1);
                        if (!start.DirectionInRange(angle, nextElement.Direction))
                        {
                            sumForce -= nextElement.Force;
                        }
                        else
                        {
                            sumForce += nextElement.Force;
                            items.Add(nextElement);
                        }
                        if (items.Count < 5)
                            continue;
                    }
                    while (sumForce > 0 &&
                        items.Count < WorldSize &&

                        run < 200);


                    start.AverageOut([.. items]);
                    for (int pos = 1; pos < items.Count; pos++)
                    {
                        if (doHorizontal)
                        {
                            _layer.SetValueAt(x + pos, y, new TectonicVector(start.Direction, start.Force));
                            x++;
                        }
                        else
                        {
                            _layer.SetValueAt(x, y + pos, new TectonicVector(start.Direction, start.Force));
                            y++;
                        }
                    }
                }
            }
        }
    }
}
