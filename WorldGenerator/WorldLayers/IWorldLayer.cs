using System.Drawing;

namespace WorldGenerator.WorldLayers
{
    public interface IWorldLayer<T> where T : IComparable
    {
        T InitialValue { get; }
        T[,] Matrix { get; }
        string Name { get; }
        int Size { get; }

        Point CalcMidPoint(Point startingPoint, Point endingPoint);
        void ClearTo(T val);
        uint ConvertToActualIndex(int v);
        Point ConvertWrappedIndexToActualIndex(Point p);
        T FindLargest();
        T FindSmallest();
        IEnumerable<int> GetBinaryEnumerator();
        IEnumerable<Tuple<uint, uint, uint>> GetBinaryTupleEnumerator();
        T GetValueAt(int x, int y);
        T GetValueAt(Point p);
        void NormalizePoint(ref Point p);
        void SetValueAt(int x, int y, T value);
        void SetValueAt(Point p, T value);
        string ToString();
        void ValidateParams(string name);
    }
}