using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjektASM
{
    internal static class MyBitmap
    {
        public static Bitmap ColorBalance(this Bitmap sourceBitmap, byte blueLevel,
                                     byte greenLevel, byte redLevel, int numberOfThreads)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                            sourceBitmap.Width, sourceBitmap.Height),
                                            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            float blueLevelFloat = 255.0f - blueLevel;
            float greenLevelFloat = 255.0f - greenLevel;
            float redLevelFloat = 255.0f - redLevel;

            Parallel.For(0, sourceBitmap.Height, row =>
            {
                int rowStartIndex = row * sourceData.Stride;
                for (int k = rowStartIndex; k + 4 < rowStartIndex + sourceData.Stride; k += 4)
                {
                    float blue = 255.0f / blueLevelFloat * (float)pixelBuffer[k];
                    float green = 255.0f / greenLevelFloat * (float)pixelBuffer[k + 1];
                    float red = 255.0f / redLevelFloat * (float)pixelBuffer[k + 2];

                    if (blue > 255) { blue = 255; }
                    else if (blue < 0) { blue = 0; }

                    if (green > 255) { green = 255; }
                    else if (green < 0) { green = 0; }

                    if (red > 255) { red = 255; }
                    else if (red < 0) { red = 0; }

                    pixelBuffer[k] = (byte)blue;
                    pixelBuffer[k + 1] = (byte)green;
                    pixelBuffer[k + 2] = (byte)red;
                }
            });

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                            resultBitmap.Width, resultBitmap.Height),
                                           ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }

    }
}
