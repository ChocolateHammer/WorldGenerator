﻿using System.Drawing;

namespace WorldGenerator.WorldLayers
{
    /// <summary>
    /// 
    /// </summary>
    public class TopographicalLayer : WorldLayer<decimal>
    {

        public TopographicalLayer( uint size, decimal initialValue =0) : base("Topographical", size, initialValue)
        {
        }

        public void SetValueOffsetAt(Point p, decimal offset)
        {
            var realCord = ConvertWrappedIndexToActualIndex(p);
            var value = GetValueAt(p);
            Matrix[realCord.X, realCord.Y] = value + offset;
        }

        public void SetValueAveAt(Point p, decimal newValue)
        {
            var realCord = ConvertWrappedIndexToActualIndex(p);
            var value = GetValueAt(p);
            Matrix[realCord.X, realCord.Y] = (value + newValue) /2;
        }
    }
}
