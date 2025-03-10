using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldGenerator.WorldLayers
{
    public class TectonicLayer : WorldLayer<TetonicVector>
    {
        public TectonicLayer(string name, int size, TetonicVector initialValue) : base("Tectonic", size, initialValue)
        {

        }
    }

    public class TetonicVector: IComparable
    {

        int YDrift { get; set; }
        int XDrift { get; set; }

        //private Double _direction = null;
        //Double Direction { get; set; }

        //Decimal CalcDirection()
        //{
        //    TetonicVector
        //}

        public int CompareTo(object? obj)
        {
            //write this for real later need to convert to radians and then add a lenght
            return 0;
        }
    }
}
