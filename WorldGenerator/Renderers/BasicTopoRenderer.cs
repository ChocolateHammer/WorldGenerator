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

            var returnValue = new MagickImage(MagickColor.FromRgb(0, 100, 200), (uint)Layer.Size, (uint)Layer.Size);
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
            var returnValue = new MagickImage(MagickColor.FromRgb(0, 100, 200), (uint)Layer.Size, (uint)Layer.Size);

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
            //if something went wonky in the world gen and we have points too high or too low reset to max for render
            if (value > MaxHeight) value = MaxHeight;
            else if (value < -MaxHeight) value = -MaxHeight;
            //translate max height to a piece that is 1/255 the size so it maps to an RBG value
            var hinc = MaxHeight / 255;
            //normalized value will have a range of (255,-255) 
            var normalizeValue = value / hinc;


            //red should have a domain of (255, 500) but I don't want to have it bleed green out so give it a fractional value
            byte r = 0;
            if (normalizeValue > 230) r = (byte)normalizeValue;
            else if (normalizeValue > 205) r = (byte)(normalizeValue * 4/5);
            else if (normalizeValue > 180) r = (byte)(normalizeValue * 3/5);
            else if (normalizeValue > 155) r = (byte)(normalizeValue *2/5);
            else if (normalizeValue > 130) r = (byte)(normalizeValue/ 5);
            else if( normalizeValue > 100) r = (byte)(normalizeValue / 8);
            else if (normalizeValue > 50) r = (byte)(normalizeValue / 20);
            //green should have a range of (255, -255 ) but i want it heavy above 0 and low below it
            byte g = (normalizeValue >= 0) ? (byte)Math.Min(255, normalizeValue * 5 / 2) : (byte)((normalizeValue + 255) / 3);
            //blues domain is 255 to -255 -> (255 to 50) so v = (normalizeValue + 255)/2
            byte b = (normalizeValue >= 0) ? (byte)((normalizeValue + 255) *2/5):(byte)((normalizeValue + 400)/2); 
            //if( normalizeValue == 0)
            //    return new ReadOnlySpan<byte>(new byte[] { 0, 0, 0 });
            return new ReadOnlySpan<byte>(new byte[] { r,g,b});
        }

        private ReadOnlySpan<byte> GetColorHist(int x, int y)
        {
            var value = Layer.GetValueAt(x, y);
            return new ReadOnlySpan<byte>(new byte[] { (byte)(value%255), (byte)(value % 255), (byte)(value % 255) });

        }

    }
}