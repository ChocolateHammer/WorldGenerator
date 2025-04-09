using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using WorldGenerator.Generators;
using WorldGenerator.WorldLayers;

namespace WorldGenerator
{
    /// <summary>
    /// Before I started this project I had a loose idea of how this would 
    /// work sketched out.  But things didn't work the way I wanted them to
    /// so I'm just proto-typing now to try to figure out what will actually
    /// work I'll start over and do thing properly. 
    /// 
    /// This class will ultimately have all the world layers and be able to do things like apply weather
    /// techitonic movements, ocean currents, rain fall, and ultimately life layers, possibly even creating
    /// cities, and histories.
    /// 
    /// </summary>
    public class WorldManager
    {
        Dictionary<string, dynamic> _layers = new Dictionary<string, dynamic>();

        public int Seed { get; }
        public int Size { get; }

        public WorldManager( int seed, int size, int maxHeight, int deviation )
        {
            Seed = seed;
            Size = size;

            //these should be done with an IOC 
            CreateLayer(new TectonicGenerator(seed, size, deviation));
            CreateLayer(new TopoGeneratorSin(seed, size, maxHeight, deviation));

        }

        private void CreateLayer(GeneratorBase gen)
        {
            var l = gen.Generate();
            _layers.Add(l.Name, l);
        }

        public void ApplyFirstTechCyles()
        {
            var tectonicLayer = (TectonicLayer)_layers[TectonicLayer.TectonicName];
            var topoLayer = (TopographicalLayer)_layers[TopographicalLayer.TopoName];

            for (int steps = 3; steps < Size / 8; steps += 5)
            {
                MovePlatesByStep(tectonicLayer, topoLayer, steps);
            }
        }

        //Todo : need to write test for this haven't done it yet because there's a high likely hood that this is going to be thrown out.
        private void MovePlatesByStep(TectonicLayer tectonicLayer, TopographicalLayer topoLayer, float rupturePoint)
        {
            foreach( var y in tectonicLayer.GetBinaryEnumerator())
            {
                double f = 0;
                for( int x =0; x<Size; x++)
                {        
                    var v1 = tectonicLayer.GetValueAt(x, (int)y);
                    if (v1.HasBeenProcessed == false)
                    {
                        bool ruptured = false;
                        do
                        {
                            f += v1.Force;
                            v1.HasBeenProcessed = true;
                            var next_loc = v1.VectorIsFacing(new Point(x, (int)y));
                            var v2 = tectonicLayer.GetValueAt(next_loc);
                            if (v1.DirectionInRange(30, v2.Direction))
                            {
                                //break into subfunction
                                f += v1.Force;
                                v2.HasBeenProcessed = true;
                                v1.AverageOut(v2);
                                v2.SetFrom(v1);
                                if (f >= rupturePoint)
                                {
                                    //DoInLineRupter(f, next_loc);
                                    topoLayer.ApplyOffset(next_loc, (decimal)(1 + (rupturePoint - f)));
                                    ruptured = true;
                                }
                            }
                            else
                            {
                                //need to walk the opposing vector to make sure we don't just overrun it[if it's a one off]
                                var v2_nextPos = v2.VectorIsFacing(next_loc, v2.Direction - 180);
                                if (tectonicLayer.GetValueAt(v2_nextPos).VectorIsFacing(v2_nextPos) == next_loc)
                                {
                                    if (v1.DirectionInRange(90, v2.Direction))
                                    {
                                        //convergent pushing together< 90
                                        ruptured = true;
                                    }
                                    else if (v1.DirectionInRange(180, v2.Direction))
                                    {
                                        //divergent pulling apart > 180
                                        ruptured = true;
                                    }
                                    else
                                    {
                                        //transform moving sideways
                                        ruptured = true;
                                    }
                                }
                                else
                                {
                                    //do the same stuff as above
                                }
                            }
                          
                        }
                        while (!ruptured);
                    }

                }
            }
        }
    }
}
