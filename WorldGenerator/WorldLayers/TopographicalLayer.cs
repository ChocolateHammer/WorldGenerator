using System.Drawing;

namespace WorldGenerator.WorldLayers
{
    /// <summary>
    /// 
    /// </summary>
    public class TopographicalLayer : WorldLayer<decimal>
    {

        public TopographicalLayer( int size, decimal initialValue =0) : base("Topographical", size, initialValue)
        {
        }

        public void ApplyOffset(Point p, decimal offset)
        {
            var realCord = ConvertWrappedIndexToActualIndex(p);
            var value = GetValueAt(p);
            Matrix[realCord.X, realCord.Y] = value + offset;
        }

        public void SetValueToAverage(Point p, decimal newValue)
        {
            var realCord = ConvertWrappedIndexToActualIndex(p);
            var value = GetValueAt(p);
            if (value == InitialValue)
                SetValueAt(p, newValue);
            else
                Matrix[realCord.X, realCord.Y] = (value + newValue) /2;
        }
    }
}
