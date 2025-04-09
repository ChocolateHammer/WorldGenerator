using System.Drawing;
using System.Reflection.Emit;
using WorldGenerator.WorldLayers;

namespace WorldGenerator.Generators
{
    public class TopoGenerator3 : GeneratorBase
    {
        private Random _rand;
        private TopographicalLayer _layer;

        public decimal DefaultValue { get; }

        public TopoGenerator3(int seed, int worldSize, int maxHeight, int deviation, decimal defaultValue) : base(seed, worldSize, maxHeight, deviation)
        {
            //The map is meant to be seamless meaning if you wrapped it then the edges line
            //up with the far edge.  This algorithm should ensure that by starting at the edges
            //and working toward the center
            _rand = new Random(seed);
            _layer = new TopographicalLayer(WorldSize, defaultValue);
            DefaultValue = defaultValue;
        }

        #region Calculation functions.
        /// <summary>
        /// Calc a value
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        internal decimal CalcValue(decimal v1, decimal v2, decimal baseValue)
        {
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


        internal decimal GetRandomDeviation() => GetNextRand(Deviation * 2) - Deviation;
        /// <summary>
        /// Get a randomized point somewhere +|- a deviation from the mid point of [v1,v2]
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        internal decimal CalcNormalizedPos(decimal v1, decimal v2)
        {
            var mid = (v1 + GetRandomDeviation() + v2 + GetRandomDeviation()) / 2;
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


        /// <summary>
        /// Generates a habitational line with randomized items
        /// </summary>
        /// <param name="yPos"></param>
        internal void GenerateIndependentLine(int yPos)
        {

            List<uint> usedPos = new List<uint>();
            foreach (var xSet in _layer.GetBinaryTupleEnumerator())
            {
                if (!usedPos.Contains(xSet.Item1))
                {
                    _layer.SetValueAt(new Point((int)xSet.Item1, yPos), GetRandomHeight());
                    usedPos.Add(xSet.Item1);
                }
                if (!usedPos.Contains(xSet.Item3))
                {
                    _layer.SetValueAt(new Point((int)xSet.Item2, yPos), GetRandomHeight());
                    usedPos.Add(xSet.Item1);
                }
                if (!usedPos.Contains(xSet.Item1))
                {
                    SetMidValueOnHorizontalLine((int)xSet.Item1, (int)xSet.Item2, (int)xSet.Item3, yPos);
                    usedPos.Add(xSet.Item2);
                }
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
            var v1 = _layer.GetValueAt(new Point(startX, yPos));
            var v2 = _layer.GetValueAt(new Point(endX, yPos));
            var value = CalcValue(v1, v2, GetRandomHeight());
            _layer.SetValueAt(new Point(midX, yPos), value);
        }

        private void GenerateBorderLines()
        {
            GenerateIndependentLine(0);
            for (int x = 0; x < WorldSize; x++)
            {
                _layer.SetValueAt(new Point(x, (int)_layer.Size - 1), _layer.GetValueAt(new Point(x, 0)));
            }
            GenerateIndependentLine((int)(WorldSize / 2));
        }

        public override dynamic Generate()
        {
            var setMatrix = new byte[WorldSize, WorldSize];

            //GenerateBorderLines();
            foreach ( var y in _layer.GetBinaryTupleEnumerator())
            {
                foreach( var x in _layer.GetBinaryTupleEnumerator())
                {
                    var x1 = _layer.GetValueAt((int)x.Item1, (int)y.Item1);
                    var x2 = _layer.GetValueAt(new Point((int)x.Item3, (int)y.Item3));
                    if ( x1 == DefaultValue )
                    {
                        x1 = GetRandomHeight();
                        _layer.SetValueAt(new Point((int)x.Item1, (int)y.Item1), x1);
                    }
                    if (x2 == DefaultValue)
                    {
                        x2 = GetRandomHeight();
                        _layer.SetValueAt(new Point((int)x.Item3, (int)y.Item3), x2);
                    }
                    var curHeight = CalcValue(x1, x2, GetRandomHeight());
                    _layer.SetValueAt((int)x.Item2, (int)y.Item2, curHeight);
                }
            }
            return _layer;
        }

    }
}
