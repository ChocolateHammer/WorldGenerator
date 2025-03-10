using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using WorldGenerator.WorldLayers;

[assembly: InternalsVisibleToAttribute("WorldGeneratorTest")]
namespace WorldGenerator.Generators
{
    public class TopographicalGenerator : GeneratorBase
    {
        private readonly Random _rand;
        private readonly TopographicalLayer _layer;

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
            if ( (baseValue >= v1 && baseValue <= v2 ) ||
                (baseValue >= v2 && baseValue <= v1) )
            {
                //if base value is in the domain [v1,.... baseValue, .... v2] no need to tweak it.
                return baseValue;
            }
            else 
            {
                return CalcNormalizedPos(v1, v2);
            }
        }


        /// <summary>
        /// Get a randomized point somewhere +|- a deviation from the mid point of [v1,v2]
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        internal decimal CalcNormalizedPos( decimal v1, decimal v2)
        {
            var mid = (v1 + v2) / 2;
            mid += GetNextRand(Deviation * 2) - Deviation;
            if (mid > MaxHeight)
            {
                return MaxHeight;
            }
            else if( mid <-MaxHeight)
            {
                return -MaxHeight;
            }
            return mid;
        }

        internal decimal GetRandomHeight() => GetNextRand(MaxHeight * 2) - MaxHeight;

        /// <summary>
        /// Need to move the rand to a function that can be overloaded for tests.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        virtual protected  decimal GetNextRand(int max) => _rand.Next(max);
        
        private void GenerateLine( int startY, int genY, int endY)
        {
            foreach( int xPos in Layer.GetBinaryEnumerator())
            {
                var v1 = Layer.GetValueAt(new Point(xPos, startY));
                var v2 = Layer.GetValueAt(new Point(xPos, endY));
                var value = CalcValue(v1, v2);
                Layer.SetValueAt(new Point(xPos, genY), value);
            }
        }

        internal void GenerateIndependentLine(int yPos)
        {
            foreach (int xPos in Layer.GetBinaryEnumerator())
            {
                Layer.SetValueAt(new Point(xPos, yPos), GetRandomHeight());
            }
        }

        public void GenerateLines()
        {
            int? lastYPos = null;
            int? lastLastYPos = null;

            foreach (int yPos in Layer.GetBinaryEnumerator())
            {
                lastLastYPos = lastYPos;
                if( lastYPos == null || lastLastYPos == null)
                {
                    GenerateIndependentLine(yPos);
                    lastYPos = yPos;
                }
                else
                {
                    GenerateLine(lastYPos.Value, yPos, lastLastYPos.Value);
                    lastLastYPos = lastYPos;
                    lastYPos = yPos;
                }
            }
        }

        public TopographicalLayer Generate()
        {
            GenerateLines();
            return Layer;
        }
    }
}
