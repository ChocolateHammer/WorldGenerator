using WorldGenerator.WorldLayers;

namespace WorldGenerator.Generators
{
    public abstract class GeneratorBase
    {
        /// <summary>
        /// I think I'm going to build a little ui later on that will let the user tweak the generate params 
        /// so even thought the constructor takes all the input bars the associated properties have gets and sets 
        /// and are not read only.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="worldSize"></param>
        /// <param name="maxHeight"></param>
        /// <param name="deviation"></param>
        public GeneratorBase(int seed, uint worldSize, int maxHeight, int deviation)
        {
            Seed = seed;
            WorldSize = worldSize;
            MaxHeight = maxHeight;
            Deviation = deviation;

            //Todo: set min and mx limits
        }

        /// <summary>
        /// seed used to randomize the world.   THe seed will be reset for each layer
        /// so that if we want to be able to zoom in the algorithm will be consistant.
        /// </summary>
        public int Seed { get; set; }
        /// <summary>
        /// The size of the world being generated
        /// </summary>
        public uint WorldSize { get; set; }
        /// <summary>
        /// Max hieght/depth of the world
        /// </summary>
        public int MaxHeight { get; set; }
        /// <summary>
        /// Determines how 'bumpy' the world is allowed to be.
        /// </summary>
        public int Deviation { get; set; }


        ///// <summary>
        ///// Generates a new worldlayer object using the generator settings.
        ///// </summary>
        ///// <returns>generated world</returns>
        //public abstract WorldLayer<T> Generate<T>() where T : IComparable;

    }
}
