﻿using System;
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
using System.Security.RightsManagement;

namespace ProjektASM
{
    internal static class MyBitmap
    {
        public static Bitmap ColorBalanceParallel(this Bitmap sourceBitmap, byte blueLevel,
                                      byte greenLevel, byte redLevel, uint numberOfThreads)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                            sourceBitmap.Width, sourceBitmap.Height),
                                            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            float blueLevelFloat = blueLevel;
            float greenLevelFloat = greenLevel;
            float redLevelFloat = redLevel;

            Parallel.For(0, pixelBuffer.Length, new ParallelOptions { MaxDegreeOfParallelism = (int)numberOfThreads }, k =>
            {
                if (k * 4 < pixelBuffer.Length)
                {
                    int index = k * 4;
                    float blue = blueLevelFloat / 255.0f * (float)pixelBuffer[index];
                    float green = greenLevelFloat / 255.0f * (float)pixelBuffer[index + 1];
                    float red = redLevelFloat / 255.0f * (float)pixelBuffer[index + 2];

                    if (blue > 255) { blue = 255; }
                    else if (blue < 0) { blue = 0; }

                    if (green > 255) { green = 255; }
                    else if (green < 0) { green = 0; }

                    if (red > 255) { red = 255; }
                    else if (red < 0) { red = 0; }

                    pixelBuffer[index] = (byte)blue;
                    pixelBuffer[index + 1] = (byte)green;
                    pixelBuffer[index + 2] = (byte)red;
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
