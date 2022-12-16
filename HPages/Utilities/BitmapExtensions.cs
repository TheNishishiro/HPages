using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HPages.Utilities
{
    public static class BitmapExtensions
    {
        public static string Hash(this byte[] data)
        {
            try
            {
                Bitmap bmp;
                using (var ms = new MemoryStream(data))
                {
                    bmp = new Bitmap(ms);
                    bmp = bmp.MakeGrayscale3();
                    bmp = bmp.ResizeBitmap(100, 100);
                    return bmp.Hash();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public static string Hash(this Bitmap bmp)
        {
            var hash = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    hash.Append(bmp.GetPixel(i,j).R.ToString("000"));
                }
            }
            return hash.ToString();
        }
        
        public static Bitmap ResizeBitmap(this Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }
 
            return result;
        }
        
        public static Bitmap MakeGrayscale3(this Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using(Graphics g = Graphics.FromImage(newBitmap)){

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][] 
                    {
                        new float[] {.3f, .3f, .3f, 0, 0},
                        new float[] {.59f, .59f, .59f, 0, 0},
                        new float[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });

                //create some image attributes
                using(ImageAttributes attributes = new ImageAttributes()){

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }
    }
}