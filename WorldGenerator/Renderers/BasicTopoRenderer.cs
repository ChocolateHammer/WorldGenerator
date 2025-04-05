using ImageMagick;
using ImageMagick.Drawing;
using WorldGenerator.Generators;
using WorldGenerator.WorldLayers;

namespace WorldGenerator.Renderers
{
    public class BasicTopoRenderer
    {
        public BasicTopoRenderer(TopographicalLayer layer)
        {
            Layer = layer;
        }

        public TopographicalLayer Layer { get; }

        public MagickImage Render()
        {

            var returnValue = new MagickImage(MagickColor.FromRgb(0, 100, 200), Layer.Size, Layer.Size);
            var maxHeight = Layer.FindLargest();

            var pixels = returnValue.GetPixels();
            for (int x = 0; x < Layer.Size; x++)
            {
                for (int y = 0; y < Layer.Size; y++)
                {
                    pixels.SetPixel(x, y, GetColor(x, y, maxHeight));
                }
            }
            return returnValue;
        }


        public MagickImage RenderHist()
        {
            var returnValue = new MagickImage(MagickColor.FromRgb(0, 100, 200), Layer.Size, Layer.Size);

            var pixels = returnValue.GetPixels();
            for (int x = 0; x < Layer.Size; x++)
            {
                for (int y = 0; y < Layer.Size; y++)
                {
                    pixels.SetPixel(x, y, GetColorHist(x, y));
                }
            }
            return returnValue;

        }

        #region First stab according to documentation but it's very slow.  Keepiing it for  bit as a refernce but will delete it later\
#if false
        public MagickImage RenderOrg()
        {

            var returnValue = new MagickImage(MagickColor.FromRgb(0, 100, 200), Layer.Size, Layer.Size);
            var drawer = new Drawables();
            for (int x = 0; x < Layer.Size; x++)
            {
                for (int y = 0; y < Layer.Size; y++)
                {
                    drawer.FillColor(GetColorX(x, y));
                    drawer.Color((double)x, (double)y, PaintMethod.Point);
                }
            }

            returnValue.Draw(drawer);

            return returnValue;
        }

        private IMagickColor<byte> GetColorOrg(int x, int y)
        {

            var value = Layer.GetValueAt(x, y);
            var hinc = value / 20;
            byte r = (byte)(100 + hinc * 2),
                g = (byte)(100 + hinc * 2),
                b = (byte)(100 - hinc * 2);

            return MagickColor.FromRgb(r, g, b);
        } 
#endif
        #endregion

        private ReadOnlySpan<byte> GetColor(int x, int y, decimal MaxHeight)
        {
            var value = Layer.GetValueAt(x, y);
            var hinc = MaxHeight / 255;
            var normalizeValue = value / hinc;

            //byte r = (byte)Math.Max(0, Math.Min(255, normalizeValue / 2)),
            //    g = (byte)Math.Max(0, Math.Min(255, normalizeValue+50)),
            //    b = (byte)Math.Max(0, Math.Min(255, normalizeValue ));
            byte r = 0;
            if (normalizeValue > MaxHeight *2/3)
            {
                r = (byte)(normalizeValue);
            }
            else if( normalizeValue > MaxHeight/3)
            {
                r = (byte)(normalizeValue / 2);
            }
            else if( normalizeValue > 0 )
            {
                r = (byte)(normalizeValue / 3);
            }
            //byte r = (normalizeValue > MaxHeight / 5) ? (byte)(normalizeValue *3/5) : (byte)0; //only turn on the red when we are half way to max height
            byte g = (normalizeValue <= 0) ? (byte)0:(byte)Math.Min(255,normalizeValue);
            byte b = (normalizeValue <= 0) ? (byte)255 : (byte)((g) *3/4); 
            return new ReadOnlySpan<byte>(new byte[] { r,g,b});
        }

        private ReadOnlySpan<byte> GetColorHist(int x, int y)
        {
            var value = Layer.GetValueAt(x, y);
            return new ReadOnlySpan<byte>(new byte[] { (byte)(value%255), (byte)(value % 255), (byte)(value % 255) });

        }

    }
}