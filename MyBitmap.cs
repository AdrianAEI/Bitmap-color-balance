using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Shapes;
using Rectangle = System.Drawing.Rectangle;
/*
* //i try
byte[] pixelBuffer = new byte[bitmapData.Stride * bitmapData.Height];
Marshal.Copy(bitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

Bitmap resultBitmap = new Bitmap(processedBitmap.Width, processedBitmap.Height);


BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
     resultBitmap.Width, resultBitmap.Height),
    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);



Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
resultBitmap.UnlockBits(resultData);


return resultBitmap;
*/
namespace ProjektASM
{
    internal static class MyBitmap
    {
public static Bitmap ProcessUsingLockbitsAndParallel(this Bitmap processedBitmap, byte blueLevel,
                                  byte greenLevel, byte redLevel, int numberOfThreads)
        {
            BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
            int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
            int heightInPixels = bitmapData.Height;
            int widthInBytes = bitmapData.Width * bytesPerPixel;
            byte[] pixelBuffer = new byte[widthInBytes * heightInPixels];


            Marshal.Copy(bitmapData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
           // processedBitmap.UnlockBits(bitmapData);

            float blue = 0;
            float green = 0;
            float red = 0;


            float blueLevelFloat = 255.0f - blueLevel;
            float greenLevelFloat = 255.0f - greenLevel;
            float redLevelFloat = 255.0f - redLevel;
            //Parallel.For(0, heightInPixels, new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }, y =>
            for(int y=0; y<heightInPixels; y++)
            {
                int lineStartIndex = y * widthInBytes;
                for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                {
                    int pixelIndex = lineStartIndex + x;
                    blue = 255.0f / blueLevelFloat * (float)pixelBuffer[pixelIndex];
                    green = 255.0f / greenLevelFloat * (float)pixelBuffer[pixelIndex + 1];
                    red = 255.0f / redLevelFloat * (float)pixelBuffer[pixelIndex + 2];

                    if (blue > 255) { blue = 255; }
                    else if (blue < 0) { blue = 0; }

                    if (green > 255) { green = 255; }
                    else if (green < 0) { green = 0; }

                    if (red > 255) { red = 255; }
                    else if (red < 0) { red = 0; }

                    pixelBuffer[pixelIndex] = (byte)blue;
                    pixelBuffer[pixelIndex + 1] = (byte)green;
                    pixelBuffer[pixelIndex + 2] = (byte)red;
                }
            }
            Marshal.Copy(pixelBuffer, 0, bitmapData.Scan0, pixelBuffer.Length);
            processedBitmap.UnlockBits(bitmapData);
            return processedBitmap;
            //Bitmap resultBitmap = new Bitmap(processedBitmap.Width, processedBitmap.Height);
            //BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
            //                              resultBitmap.Width, resultBitmap.Height),
            //                              ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
           // Marshal.Copy(pixelBuffer, 0, bitmapData.Scan0, pixelBuffer.Length);
           //resultBitmap.UnlockBits(resultData);

            //return resultBitmap;
        }
        public static Bitmap ColorBalanceBasic(this Bitmap sourceBitmap, byte blueLevel,
                                   byte greenLevel, byte redLevel)
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


            float blueLevelFloat = blueLevel;
            float greenLevelFloat = greenLevel;
            float redLevelFloat = redLevel;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = blueLevelFloat / 255.0f * (float)pixelBuffer[k];
                green = greenLevelFloat / 255.0f * (float)pixelBuffer[k + 1];
                red = redLevelFloat / 255.0f * (float)pixelBuffer[k + 2];

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


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                        resultBitmap.Width, resultBitmap.Height),
                                       ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
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

            float blueLevelFloat = blueLevel;
            float greenLevelFloat = greenLevel;
            float redLevelFloat = redLevel;

            Parallel.For(0, sourceData.Height, new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }, j =>
            {
                for (int i = 0;i < sourceData.Width; i += 1)
                {
                    int k = j * sourceData.Stride+ i*4;
                    blue = blueLevelFloat / 255.0f * (float)pixelBuffer[k];
                    green = greenLevelFloat / 255.0f * (float)pixelBuffer[k + 1];
                    red = redLevelFloat / 255.0f * (float)pixelBuffer[k + 2];

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

            Bitmap resultBitmap=new Bitmap(sourceBitmap.Width, sourceBitmap.Height); 
            //tablica tablic bajtow

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                            resultBitmap.Width, resultBitmap.Height),
                                           ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);

            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }

    }
   
}
