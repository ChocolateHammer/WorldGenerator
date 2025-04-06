using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using WorldGenerator.WorldLayers;

namespace WorldGenerator.Generators
{
    //The first version of the generator did random lines then tried to get mid values between lines
    //the devition was actually pretty good with it but the algorythim kind of made a wood grain texture
    //world.  I think I need to try to do a kind of marching squares method.  This should accomplish that
    public class TopoGenerator2 : GeneratorBase
    {
        private Random _rand;
        private TopographicalLayer _layer;

        public TopoGenerator2(int seed, int worldSize, int maxHeight, int deviation, int defaultValue = -1) : base(seed, worldSize, maxHeight, deviation)
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
            var mid = (v1+ GetRandomDeviation() + v2 + GetRandomDeviation()) / 2;
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


        public TopographicalLayer Generate()
        {
            Randomize();
            DoMarchingSquaresPass();
            //ApplyLinerDev();
            return _layer;
        }

        private void ApplyDeviations()
        {
            for(int x = 0; x < Layer.Size; x++) 
            {
                for(int y =0; y< Layer.Size; y++)
                {
                    var h = Layer.GetValueAt(x, y);
                    h += GetNextRand(Deviation * 2) - Deviation;
                    Layer.SetValueAt(new System.Drawing.Point(x, y), h);
                }
            }
        }

        private void Randomize()
        {
            for (int x = 0; x < Layer.Size; x++)
            {
                for (int y = 0; y < Layer.Size; y++)
                {
                    
                    Layer.SetValueAt(new System.Drawing.Point(x, y), GetRandomHeight());
                }
            }
        }

        List<int> _rowsDone = new List<int>();
        private void ApplyLinerDev()
        {
            foreach (var yTuple in Layer.GetBinaryTupleEnumerator())
            {
                GenerateMidPointLineIfNeeded( yTuple );
            }
        }

        private void GenerateMidPointLineIfNeeded(Tuple<uint, uint, uint> yTuple)
        {
            if (!_rowsDone.Contains((int)yTuple.Item2))
            {
                for (int xpos = 0; xpos < Layer.Size; xpos++)
                {
                    var v1 = Layer.GetValueAt(new Point(xpos, (int)yTuple.Item1));
                    var v2 = Layer.GetValueAt(new Point(xpos, (int)yTuple.Item3));
                    var value = CalcValue(v1, v2, Layer.GetValueAt(xpos, (int)yTuple.Item2));
                    Layer.SetValueAt(new Point(xpos, (int)yTuple.Item2), value);
                }
                _rowsDone.Add((int)yTuple.Item2);
            }
        }

        private void DoMarchingSquaresPass()
        {
            int passes = _rand.Next((int)Layer.Size/5);
            while(passes-->0)
            {
                int radius =_rand.Next((int)Layer.Size/10);
                int x = _rand.Next((int)Layer.Size);
                int y = _rand.Next((int)Layer.Size);
                decimal h = GetRandomHeight();

                for (double angle = 0; angle < 90; angle++)
                {
                    var yOffset = Math.Sin(angle) * radius;
                    var xOffset = Math.Cos(angle) * radius;
                    for (int xPos = 0; xPos <= xOffset; xPos++)
                    {
                        for (int yPos = 0; yPos <= yOffset; yPos++)
                        {
                            Layer.ApplyOffset(new Point(x + xPos, y + yPos), h + GetRandomDeviation());
                            Layer.ApplyOffset(new Point(x - xPos, y + yPos), h + GetRandomDeviation());
                            Layer.ApplyOffset(new Point(x + xPos, y - yPos), h + GetRandomDeviation());
                            Layer.ApplyOffset(new Point(x - xPos, y - yPos), h + GetRandomDeviation());
                            //Layer.SetValueAt(new System.Drawing.Point(x + xPos, y + yPos), h+ GetNextRand(Deviation * 2) - Deviation);
                            //Layer.SetValueAt(new System.Drawing.Point(x - xPos, y + yPos), h+ GetNextRand(Deviation * 2) - Deviation);
                            //Layer.SetValueAt(new System.Drawing.Point(x + xPos, y - yPos), h+ GetNextRand(Deviation * 2) - Deviation);
                            //Layer.SetValueAt(new System.Drawing.Point(x - xPos, y - yPos), h+ GetNextRand(Deviation * 2) - Deviation);
                        }
                    }
                }
            }
        }
    }
}
