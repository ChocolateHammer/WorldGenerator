using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGenerator.WorldLayers
{
    public class InvalidLayerSetupException : Exception
    {
        public InvalidLayerSetupException(string reason) : base(reason) { }

    }
}
