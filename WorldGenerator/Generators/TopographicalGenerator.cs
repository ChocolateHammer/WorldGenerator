using System.Drawing;
using System.Runtime.CompilerServices;
using WorldGenerator.WorldLayers;

[assembly: InternalsVisibleTo("WorldGeneratorTest")]
namespace WorldGenerator.Generators
{
    public class TopographicalGenerator : GeneratorBase
    {
        private readonly Random _rand;
        private readonly TopographicalLayer _layer;
        private readonly List<int> _rowsDone = new List<int>();

        public TopographicalGenerator(int seed, int worldSize, int maxHeight, int deviation, decimal defaultValue = -1) : base(seed, worldSize, maxHeight, deviation)
        {
            //The map is meant to be seamless meaning if you wrapped it then the edges line
            //up with the far edge.  This algorithm should ensure that by starting at the edges
            //and working toward the center
            _rand = new Random(seed);
            _layer = new TopographicalLayer(WorldSize, defaultValue);
        }

        /// <summary>
        /// Get's the layer 
        /// </summary>
        public TopographicalLayer Layer => _layer;

        #region Calculation functions.
        /// <summary>
        /// Calc a value
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        internal decimal CalcValue(decimal v1, decimal v2)
        {
            decimal baseValue = GetRandomHeight();
            //bend it back in range somewhat
            if ((baseValue >= v1 && baseValue <= v2) ||
                (baseValue >= v2 && baseValue <= v1))
            {
                //if base value is in the domain [v1,.... baseValue, .... v2] no need to tweak it.
                return baseValue;
            }
            else
            {
                return CalcNormalizedPos(v1, v2);
            }
        }

        internal decimal GetRandomDevitaion() => GetNextRand(Deviation * 2) - Deviation;

        /// <summary>
        /// Get a randomized point somewhere +|- a deviation from the mid point of [v1,v2]
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        internal decimal CalcNormalizedPos(decimal v1, decimal v2)
        {
            var mid = (v1 + GetRandomDevitaion() + v2) / 2;
            if (mid > MaxHeight)
            {
                return MaxHeight;
            }
            else if (mid < -MaxHeight)
            {
                return -MaxHeight;
            }
            return mid;
        }

        /// <summary>
        /// Need to move the rand to a function that can be overloaded for tests.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        virtual protected decimal GetNextRand(int max) => _rand.Next(max);


        /// <summary>
        /// Gets a random height.
        /// </summary>
        /// <returns></returns>
        internal decimal GetRandomHeight() => GetNextRand(MaxHeight * 2) - MaxHeight;
        #endregion

        #region Independent line generation functions.
        /// <summary>
        /// Generates a habitational line with randomized items
        /// </summary>
        /// <param name="yPos"></param>
        internal void GenerateIndependentLineIfNeeded(int yPos)
        {
            if (!_rowsDone.Contains(yPos))
            {
                List<uint> usedPos = new List<uint>();
                foreach (var xSet in Layer.GetBinaryTupleEnumerator())
                {
                    if (!usedPos.Contains(xSet.Item1))
                    {
                        Layer.SetValueAt(new Point((int)xSet.Item1, yPos), GetRandomHeight());
                        usedPos.Add(xSet.Item1);
                    }
                    if (!usedPos.Contains(xSet.Item3))
                    {
                        Layer.SetValueAt(new Point((int)xSet.Item2, yPos), GetRandomHeight());
                        usedPos.Add(xSet.Item1);
                    }
                    if (!usedPos.Contains(xSet.Item1))
                    {
                        SetMidValueOnHorizontalLine((int)xSet.Item1, (int)xSet.Item2, (int)xSet.Item3, yPos);
                        usedPos.Add(xSet.Item2);
                    }
                }

                _rowsDone.Add(yPos);
            }
        }

        /// <summary>
        /// used to builds a horizontal line that pays attention to the end points.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="midX"></param>
        /// <param name="endX"></param>
        /// <param name="yPos"></param>
        private void SetMidValueOnHorizontalLine(int startX, int midX, int endX, int yPos)
        {
            var v1 = Layer.GetValueAt(new Point(startX, yPos));
            var v2 = Layer.GetValueAt(new Point(endX, yPos));
            var value = CalcValue(v1, v2);
            Layer.SetValueAt(new Point(midX, yPos), value);
        }
        #endregion

        public void GenerateLines()
        {
            GenerateBorderLines();
            foreach (var yTuple in Layer.GetBinaryTupleEnumerator())
            {
                GenerateIndependentLineIfNeeded((int)yTuple.Item1);
                GenerateIndependentLineIfNeeded((int)yTuple.Item3);
                GenerateMidPointLineIfNeeded(yTuple);
            }
        }

        private void GenerateBorderLines()
        {
            GenerateIndependentLineIfNeeded(0);
            for (int x = 0; x < WorldSize; x++)
            {
                Layer.SetValueAt(new Point(x, (int)Layer.Size - 1), Layer.GetValueAt(new Point(x, 0)));
                _rowsDone.Add((int)Layer.Size - 1);
            }
        }

        private void GenerateMidPointLineIfNeeded(Tuple<uint, uint, uint> yTuple)
        {
            if( !_rowsDone.Contains((int)yTuple.Item2))
            {
                for( int xpos = 0; xpos<Layer.Size; xpos ++)
                {
                    var v1 = Layer.GetValueAt(new Point(xpos, (int)yTuple.Item1));
                    var v2 = Layer.GetValueAt(new Point(xpos, (int)yTuple.Item3));
                    var value = CalcValue(v1, v2);
                    Layer.SetValueAt(new Point(xpos, (int)yTuple.Item2), value);
                }
                _rowsDone.Add((int)yTuple.Item2);
            }
        } 

        public TopographicalLayer Generate()
        {
            GenerateLines();
            return Layer;
        }
    }
}
