using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ProjektASM
{
    internal static class MyBitmap
    {
        public static Bitmap ProcessUsingLockbitsAndUnsafeAndParallel(this Bitmap processedBitmap, byte blueLevel,
                                      byte greenLevel, byte redLevel, int numberOfThreads)
        {
            unsafe
            {
                BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                float blueLevelFloat = 255.0f - blueLevel;
                float greenLevelFloat = 255.0f - greenLevel;
                float redLevelFloat = 255.0f - redLevel;

                Parallel.ForEach(Partitioner.Create(0, heightInPixels), new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }, range =>
                {
                    for (int y = range.Item1; y < range.Item2; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                        for (int x = 0; x + bytesPerPixel < widthInBytes; x += bytesPerPixel)
                        {
                            float blue = 255.0f / blueLevelFloat * (float)currentLine[x];
                            float green = 255.0f / greenLevelFloat * (float)currentLine[x + 1];
                            float red = 255.0f / redLevelFloat * (float)currentLine[x + 2];

                            if (blue > 255) { blue = 255; }
                            else if (blue < 0) { blue = 0; }

                            if (green > 255) { green = 255; }
                            else if (green < 0) { green = 0; }

                            if (red > 255) { red = 255; }
                            else if (red < 0) { red = 0; }

                            currentLine[x] = (byte)blue;
                            currentLine[x + 1] = (byte)green;
                            currentLine[x + 2] = (byte)red;
                        }
                    }
                });

                processedBitmap.UnlockBits(bitmapData);
                return processedBitmap;
            }
        }
        public static Bitmap ColorBalance(this Bitmap sourceBitmap, byte blueLevel,
                                     byte greenLevel, byte redLevel, int numberOfThreads)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                            sourceBitmap.Width, sourceBitmap.Height),
                                            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);

            float blue = 0;
            float green = 0;
            float red = 0;

            float blueLevelFloat = 255.0f - blueLevel;
            float greenLevelFloat = 255.0f - greenLevel;
            float redLevelFloat = 255.0f - redLevel;

            Parallel.For(0, sourceBitmap.Height, new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }, j =>
            {
                for (int k = 0; k + 4 < sourceBitmap.Width; k += 4)
                {
                    blue = 255.0f / blueLevelFloat * (float)pixelBuffer[k];
                    green = 255.0f / greenLevelFloat * (float)pixelBuffer[k + 1];
                    red = 255.0f / redLevelFloat * (float)pixelBuffer[k + 2];

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
