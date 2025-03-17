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

            var pixels = returnValue.GetPixels();
            for (int x = 0; x < Layer.Size; x++)
            {
                for (int y = 0; y < Layer.Size; y++)
                {
                    pixels.SetPixel(x, y, GetColor(x, y));
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

        private ReadOnlySpan<byte> GetColor(int x, int y)
        {
            var value = Layer.GetValueAt(x, y);
            var hinc = value / 10;
            byte r = (byte)Math.Max(0,(50 + hinc * 3)),
                g = (byte)Math.Max(0, (100 + hinc * 2)),
                b = (byte)Math.Max(0, (100 - hinc * 5));
            return new ReadOnlySpan<byte>(new byte[] { r,g,b});
        }

    }
}