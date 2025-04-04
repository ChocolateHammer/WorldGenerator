namespace WorldGenerator.Generators
{
    /// <summary>
    /// used to get calc a value using a sin Wave
    /// 
    /// Does the calculation is y(x) = Asin(2πfx + φ) = Asin(ωx + φ)
    ///
    /// Where:
    ///     A, amplitude, the peak deviation of the function from zero.
    ///     f, ordinary frequency, the number of oscillations (cycles) that occur each cycle of x
    ///     ω = 2πf, angular frequency, the rate of change of the function argument in units of radians per second
    ///     φ, phase, specifies (in radians) where in its cycle the oscillation is at x = 0.

    /// </summary>
    internal class SinWaveCalculator
    {
        private readonly int spread;

        /// <summary>
        /// initialize the wave calc
        /// </summary>
        /// <param name="frequency">number of x positions in a wave cycle</param>
        /// <param name="amplitude">max height of the wave</param>
        /// <param name="offset"> the offset [a fraction of the frequency</param>
        public SinWaveCalculator(double amplitude, int frequency, int offset, int spread)
        {
            Amplitude = amplitude;
            Frequency = frequency;
            Offset = offset;
            this.spread = spread;
            W = Math.PI * 2 * frequency;
        }

        private double Amplitude { get; }
        private int Frequency { get; }
        private int Offset { get; }
        private double W { get;  }

        public double CalcValue( int x )
        {
            double d = (double)x / spread;
            return Amplitude * Math.Sin(W * d + Offset);
        }

        public double CalcValueD(double x)
        {
            return Amplitude * Math.Sin(W * x + Offset);
        }
    }
}
