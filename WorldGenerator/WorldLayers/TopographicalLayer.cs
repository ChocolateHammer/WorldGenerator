using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
