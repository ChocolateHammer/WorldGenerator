


namespace WorldGenerator
{
    public class World
    {

        public World(int seaLevel, int size)
        {
            SeaLevel = seaLevel;
            Size = size;
            Surface = new decimal[size,size];
            Tectonics = new byte[size,size];
        }

        /// <summary>
        /// Gets the size of the world.
        /// </summary>
        public int Size { get; }
        /// <summary>
        /// where is the sea level
        /// </summary>
        public int SeaLevel { get; set; }
        /// <summary>
        /// height map for thw world.
        /// <Note> Originally this was a int array but I decided to convert it to decimal early on
        /// even though this will have some performance issues it will allow for much better render 
        /// functions, and aging emulation.</Note>
        /// </summary>
        public decimal[,] Surface { get; set; }
        /// <summary>
        /// Tectonic layer used for aging
        /// Note: currently byte array probably need to translate it to vector array
        /// </summary>
        public byte[,] Tectonics { get; set; }
        
    }
}
