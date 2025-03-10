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
    }
}
