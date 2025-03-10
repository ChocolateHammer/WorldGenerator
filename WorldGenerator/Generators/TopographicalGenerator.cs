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
            decimal baseValue = CalcValue();
            System.Diagnostics.Debug.Assert(baseValue <= MaxHeight + Deviation);
            //bend it back in range somewhat
            if ( (baseValue >= v1 && baseValue <= v2 ) ||
                  (baseValue >= v2 && baseValue <= v1) )
            {
                return baseValue;
            }
            else if( baseValue > v1 && baseValue >v2)
            {
                if (baseValue > 0)
                    return baseValue - ((v1 + v2) / 4);
                else
                    return baseValue + ((v1 + v2) / 4);
            }
            else
            {
                if (baseValue > 0)
                    return baseValue + ((v1 + v2) / 4);
                else
                    return baseValue - ((v1 + v2) / 4);
            }
        }


        virtual protected decimal CalcValue() => _rand.Next( MaxHeight * 2) - MaxHeight + (_rand.Next(Deviation*2)-Deviation);
        

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
                Layer.SetValueAt(new Point(xPos, yPos), CalcValue());
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
