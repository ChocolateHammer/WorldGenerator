using System.Diagnostics;
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

            //say we have direction of 5 or 0 since we count 1,2,3,4,....358,359,0,1,...
            //calcaltion of the degree gets messy lets see if the simple rule works
            if (Direction >= variance &&
                Direction+variance <= 360)
            {
                var lowRange = NormalizeDegree(direction - variance);
                var highRange = NormalizeDegree(direction + variance);

                return (Direction >= lowRange
                    && Direction <= highRange);
            }
            else if( Direction + variance > 360 )
            {
                //too close to 360 spin it backward then do the test

                var lowRange = NormalizeDegree(direction -allowedDegrees - variance);
                var highRange = NormalizeDegree(direction - allowedDegrees + variance);

                return (Direction - allowedDegrees >= lowRange
                    && Direction - allowedDegrees <= highRange);
            }
            else
            {
                //direction too close to zero apin it foward then do the test
                var lowRange = NormalizeDegree(direction + allowedDegrees - variance);
                var highRange = NormalizeDegree(direction + allowedDegrees + variance);

                return (Direction + allowedDegrees >= lowRange
                    && Direction + allowedDegrees <= highRange);
            }
        }

        public double DirectionWithSign => (Direction > 180) ? Direction -360 : Direction;

        public void AverageOut(TectonicVector prev, TectonicVector next)
        {
            AverageOut(new TectonicVector[] { prev, next });
        }


        /// <summary>
        /// Used to calculate the vector average out so that the force weights out 
        /// the sum of vectors
        /// </summary>
        /// <returns></returns>
        private double DirectionWithForceApplied() => DirectionWithSign * Force;

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

        public override string ToString()
        {
            return $"(D:{Direction:F2},F:{Force:F2})";
        }
    }
}
