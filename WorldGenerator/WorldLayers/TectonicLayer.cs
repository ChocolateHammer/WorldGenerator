using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace WorldGenerator.WorldLayers
{


    /// <summary>
    /// This class generates a Tectonic layer that can be used to advance the age of the world...
    /// </summary
    /// <remarks>Originally I had planned on making random lines of incidence but thining about it in a more general
    /// sense the crust floats on magma and the mangma currents are kind like boiling water[only much much slower.
    /// My thought is that by following that logic we don't have to care about what's actually going on only whats happening
    /// at the surface.  As such I'm randomizing the layer then running it through and algorythimn to average them out.  
    /// It's my though that by doing this I'll wind up generating fault lines that do a reasonable job of approximating it.</remarks>
    public class TectonicLayer : WorldLayer<TectonicVector>
    {
        public TectonicLayer(int size) : base("Tectonic", size)
        {

        }


        /// <summary>
        /// Gets the items surround the point at [x,y]
        /// </summary>
        /// <remarks>really should move this to the base class but right now this is the only thing that needs it</remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TectonicVector[] GetSurroundingItems(int x, int y)
        {
            var r1x1 = GetValueAt(x - 1, y - 1);
            var r1x2 = GetValueAt(x, y - 1);
            var r1x3 = GetValueAt(x + 1, y - 1);
            var r2x1 = GetValueAt(x - 1, y);
            var r2x3 = GetValueAt(x + 1, y);
            var r3x1 = GetValueAt(x - 1, y + 1);
            var r3x2 = GetValueAt(x, y + 1);
            var r3x3 = GetValueAt(x + 1, y + 1);
            return [r1x1, r1x2, r1x3, r2x1, r2x3, r3x1, r3x2, r3x3];
        }

    }
}
