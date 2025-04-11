using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace WorldGenerator.WorldLayers
{
    /// <summary>
    /// just a real simple vector
    /// </summary>
    public class TectonicVector: IComparable
    {

        public TectonicVector(double direction, double force)
        {
            Direction = direction;
            Force = force;
        }

        double _direction;
        private double _force;

        public bool HasBeenProcessed
        {
            get;set;
        }

        /// <summary>
        /// the angle 0-360 of the movement
        /// </summary>
        /// <remarks>
        /// should probably be double, thinking 
        /// </remarks>
        public double Direction
        {
            get => _direction;
            set
            {
                _direction = NormalizeDegree(value);
            }
        }

        /// <summary>
        /// handles wrapping the degree to a single rotation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double NormalizeDegree(double value)
        {
            value = value % 360;
            if (value < 0)
            {
                value = 360 + value; //value is negative so + is actually minus
            }
            return value;
        }

        /// <summary>
        /// the amount of force being directed
        /// Note: A vector without a force component cannot have a direction
        /// rather than try to deal with Nan as direction I'm simply defaulting force.
        /// </summary>
        public double Force
        {
            get => _force;
            set
            {
                _force = (value > 0) ? value: 0.0001; 
                _force = value;
            }
        }

        /// <summary>
        /// compares one vector to another.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object? obj)
        {
            if (obj is TectonicVector)
            {
                if (Direction == (obj as TectonicVector).Direction)
                {
                    return Force.CompareTo((obj as TectonicVector).Force);
                }
                return Direction.CompareTo((obj as TectonicVector).Direction);
            }
            return -1;

        }


        internal bool VectorsWithinDegreeVariance(double degree, params TectonicVector[] vectors)
        {
            //TODO write test for this
            foreach( var vector in vectors )
            {
                if( !DirectionInRange( degree, vector.Direction))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// checks to see if the direction of this vector is within allowed degrees for the passed direction
        /// </summary>
        /// <param name="allowedDegres">total allowed degrees.
        /// Note: this means 90 is +- 45 degrees.
        /// </param>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal bool DirectionInRange(double allowedDegrees, double direction)
        {
            var variance = Math.Abs(allowedDegrees) / 2;

            return (DifferenceInDirection(direction) <= variance);
        }

        public double DirectionWithSign() => GetDirectionWithSign(Direction);

        public double GetDirectionWithSign(Double d)
        {
            d = d % 360;
            return (d  > 180) ? d - 360 : d;
        }

        public void AverageOut(TectonicVector prev, TectonicVector next)
        {
            AverageOut(new TectonicVector[] { prev, next });
        }


        /// <summary>
        /// Used to calculate the vector average out so that the force weights out 
        /// the sum of vectors
        /// </summary>
        /// <returns></returns>
        private double DirectionWithForceApplied() => DirectionWithSign() * Force;

        /// <summary>
        /// Sums out the array of vectors[including this one] and sets the vector to the
        /// force weighted average of all of them.
        /// </summary>
        /// <param name="vectors"></param>
        public void AverageOut(params TectonicVector[] vectors)
        {
            double sumForce = 0;
            double sumDirection = 0;
            foreach( var vector in vectors)
            {
                sumForce += vector.Force;
                sumDirection += vector.DirectionWithForceApplied();
            }
            sumForce += Force;
            sumDirection +=DirectionWithForceApplied();
            var averageForce = sumForce / (vectors.Length + 1);
            //--if we have 90:10 0:10 at this point the values should be sumF:20 sumD:900 aveF:10
            //then the New force should be 10, and new direction should be 45=[((90*10)+(0*10))/(10+10)]
            //--if we have 90:3 0:2 at this point values should be sumF:5, sumD:270, aveF: 2.5
            //so Force should become 2.5 and direction should be 60=[((90*3)+(0*2))/(5)]=(270+0)/5
            //--if we have 90:0,0:0 at this point the values should be sumF:20 sumD:900 aveF

            var dWithForce = (sumForce != 0 ) ? sumDirection/sumForce: sumDirection;
            Force = averageForce;
            Direction = NormalizeDegree(dWithForce);
        }

        const double DEGREES_INCREMENT = 22.5D;
    
        public Point VectorIsFacing(Point p, double direction)
        {
            var directionDif = DifferenceInDirection(0);
            directionDif = Math.Abs(GetDirectionWithSign(directionDif));
            var singedD = DirectionWithSign();
            var y = (singedD > 0) ? -1 : 1;

            if (directionDif <= DEGREES_INCREMENT)
            {
                p.Offset(1, 0);
            }
            else if (directionDif <= DEGREES_INCREMENT*3)
            {
                p.Offset(1, y);
            }
            else if (directionDif <= DEGREES_INCREMENT * 5)
            {
                p.Offset(0, y);
            }
            else if (directionDif <= DEGREES_INCREMENT * 7)
            {
                p.Offset(-1, y);
            }
            else 
            {
                p.Offset(-1, 0);
            }

            return p;
        }

        public Point VectorIsFacing(Point p )
        {
            return VectorIsFacing(p, Direction);
        }

        public override string ToString()
        {
            return $"(D:{Direction:F2},F:{Force:F2})";
        }

        /// <summary>
        /// basically an assignment operator.  I
        /// could have overridden the equal operator
        /// but I think this is cleaner.
        /// </summary>
        /// <param name="v1"></param>
        internal void SetFrom(TectonicVector v1)
        {
            Direction = v1.Direction;
            Force = v1.Force;
            HasBeenProcessed = v1.HasBeenProcessed;
        }

        internal double DifferenceInDirection(TectonicVector v)
        {
            return DifferenceInDirection(v.DirectionWithSign());
        }

        internal double DifferenceInDirection(double direction)
        {
            return Math.Abs((Direction - GetDirectionWithSign(direction))%360);
        }
    }
}
