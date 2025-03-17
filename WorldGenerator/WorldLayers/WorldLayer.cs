using ImageMagick;
using System.Drawing;

namespace WorldGenerator.WorldLayers
{
    /// <summary>
    /// The virtual world is actually going to consist of several layers 
    /// this class is a generic version that can communalize matrix stuff
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WorldLayer<T> where T: IComparable
    {

        /// It makes sense to have a function to enforce the layers are 
        /// of a minimum viable size but it's made the TDD harder than I think
        /// justifies it's existence when a small one will just generate a garbage world 
        /// and the UI will restrict it in any case the function public virtual int GetMinViableSize() => 500 has been removed for that reason

        public WorldLayer(string name, uint size, T initialValue)
        {
            ValidateParams(name, size);
            Name = name;
            Size = size;
            Matrix = new T[size, size];
            ClearTo(initialValue);
        }

        public WorldLayer(string name, uint size)
        {
            ValidateParams(name, size);
            Name = name;
            Size = size;
            Matrix = new T[size, size];
        }

        public void ValidateParams(string name, uint size)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidLayerSetupException("Invalid world layer Name");
        }

        /// <summary>
        /// Sets all the values in the matrix to a specific value
        /// </summary>
        /// <param name="val"></param>
        public void ClearTo(T val)
        {
            //could use Matrix.GetLowerBound(0_ and Matrix.GetUpperBound(0) but right now the matrix is fixed to size
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Matrix[i, j] = val;
                }
            }
        }

        /// <summary>
        /// commonialize the walk/
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        protected T Find(Func<T,T,bool> comparer)
        {
            var currentChoice = Matrix[0, 0];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (comparer(currentChoice, Matrix[i, j]))
                        currentChoice = Matrix[i, j];
                }
            }
            return currentChoice;
        }

        public T FindLargest()
        {
            Func<T, T, bool> largest = (n1, n2) => n1.CompareTo(n2) < 0;
            return Find(largest);
        }

        public T FindSmallest()
        {
            Func<T, T, bool> largest = (n1, n2) => n2.CompareTo(n1) < 0;
            return Find(largest);
        }


        /// <summary>
        /// a method that lets you just do math go get a value 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public T GetValueAt(int x, int y)
        {
            var realCord = ConvertWrappedIndexToActualIndex(new Point(x, y));
            return Matrix[realCord.X,realCord.Y];
        }

        public T GetValueAt(Point p)
        {
            var realCord = ConvertWrappedIndexToActualIndex(p);
            return Matrix[realCord.X, realCord.Y];
        }

        public void SetValueAt(Point p, T value)
        {
            var realCord = ConvertWrappedIndexToActualIndex(p);
            Matrix[realCord.X, realCord.Y] = value;
        }

        [Obsolete]
        public void SetValueAt(int x, int y, T value)
        {
            SetValueAt(new Point(x, y), value);
        }

        public Point ConvertWrappedIndexToActualIndex(Point p)
        {
            //if the index is grater than size get the offset that is left
            int realx = (int)ConvertToActualIndex(p.X);
            int realy = (int)ConvertToActualIndex(p.Y);

            return new Point(realx, realy);
        }

        public uint ConvertToActualIndex(int v)
        { 
            //if it's in legal ranges just return it
            if (v < Size && v >= 0) return (uint)v;
            //I'd tried to do this in a cleaner way but it got funky with negative vars
            //come back and simply this later right now I just want it to work.
            if( v >= Size)
            {
                return (uint)(v%Size);
            }
            else
            {
                uint value = (uint)(Math.Abs(v) % Size);
                if (value == 0)
                    return 0;
                return Size-value;
            }
        }

        public void NormalizePoint( ref Point p)
        {
            p.X =(int) ConvertToActualIndex(p.X);
            p.Y = (int)ConvertToActualIndex(p.Y);
        }

        public Point CalcMidPoint( Point startingPoint, Point endingPoint)
        {
            NormalizePoint(ref startingPoint);
            NormalizePoint(ref endingPoint);
            int midX = (startingPoint.X == endingPoint.X) ? startingPoint.X : (startingPoint.X + endingPoint.X) / 2;
            int midY = (startingPoint.Y == endingPoint.Y) ? startingPoint.Y : (startingPoint.Y + endingPoint.Y) / 2;

            return new Point(midX, midY);
        }


        public IEnumerable<uint> GetBinaryEnumerator()
        {
            var values = new List<uint>();

            RecurseBinaryWalk(0, Size , values);

            foreach (var v in values)
                yield return v;
        }

        /// <summary>
        /// This uses recursion to generate a binary walk of the nodes in a the world
        /// It's a little off right now because it does the lower half then the upper half
        /// ideally the distribution would be more even, but its good enough to go right now.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="items"></param>
        private void RecurseBinaryWalk( uint start, uint end, List<uint> items )
        {
            if (start != end)
            {
                uint next = (start + end) / 2;
                if (next != start 
                    || (next == 0 && !items.Contains(0)) )
                {
                    items.Add(next);
                    RecurseBinaryWalk(start, next, items);
                    RecurseBinaryWalk(next, end, items);
                }
            } 
        }

        /// <summary>
        /// The matrix that holds data about the world
        /// </summary>
        public T[,] Matrix { get; }

        /// <summary>
        /// The name of this layer
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The size of the matrix
        /// </summary>
        public uint Size { get; }
  
    }
}
