using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Q3MinimapGenerator
{

    public class ByteImage
    {
        public byte[] imageData;
        public int stride;
        public int width, height;
        public PixelFormat pixelFormat;

        public ByteImage(byte[] imageDataA, int strideA, int widthA, int heightA, PixelFormat pixelFormatA)
        {
            imageData = imageDataA;
            stride = strideA;
            width = widthA;
            height = heightA;
            pixelFormat = pixelFormatA;
        }

        public int Length
        {
            get { return imageData.Length; }
        }

        public byte this[int index]
        {
            get
            {
                return imageData[index];
            }

            set
            {
                imageData[index] = value;
            }
        }
    }

    public static class Helpers
    {

        public static ByteImage BitmapToByteArray(Bitmap bmp)
        {

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int stride = Math.Abs(bmpData.Stride);
            int bytes = stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            bmp.UnlockBits(bmpData);

            return new ByteImage(rgbValues, stride, bmp.Width, bmp.Height, bmp.PixelFormat);
        }
        public static Bitmap ByteArrayToBitmap(ByteImage byteImage)
        {
            Bitmap myBitmap = new Bitmap(byteImage.width, byteImage.height, byteImage.pixelFormat);
            Rectangle rect = new Rectangle(0, 0, myBitmap.Width, myBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                myBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                myBitmap.PixelFormat);

            bmpData.Stride = byteImage.stride;

            IntPtr ptr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(byteImage.imageData, 0, ptr, byteImage.imageData.Length);

            myBitmap.UnlockBits(bmpData);
            return myBitmap;

        }
        public static T ReadBytesAsType<T>(this BinaryReader br, long byteOffset = -1, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            if (!(byteOffset == -1 && seekOrigin == SeekOrigin.Begin))
            {
                br.BaseStream.Seek(byteOffset, seekOrigin);
            }
            byte[] bytes = br.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T retVal = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return retVal;
        }
        public static void WriteBytesFromType<T>(this BinaryWriter bw, T value, long byteOffset = -1, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            if (!(byteOffset == -1 && seekOrigin == SeekOrigin.Begin))
            {
                bw.BaseStream.Seek(byteOffset, seekOrigin);
            }
            byte[] byteData = new byte[Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(byteData, GCHandleType.Pinned);
            // TODO Not sure if this is safe? Am I expected to do some fancy AllocHGlobal and then Marshal.Copy?! Why? This seems to work so whats the problem?
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            bw.Write(byteData);
        }
        public static byte[] BytesFromType<T>(T value)
        {
            byte[] byteData = new byte[Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(byteData, GCHandleType.Pinned);
            // TODO Not sure if this is safe? Am I expected to do some fancy AllocHGlobal and then Marshal.Copy?! Why? This seems to work so whats the problem?
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            return byteData;
        }

        public static T ArrayBytesAsType<T, B>(B data, int byteOffset = 0)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T retVal = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject() + byteOffset, typeof(T));
            handle.Free();
            return retVal;
        }
        public static float zCross2d(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
        {
            return ((p2.X - p1.X) * (p3.Y - p2.Y)) - ((p2.Y - p1.Y) * (p3.X - p2.X));
        }

        public static bool pointInTriangle2D(ref Vector3 point, ref Vector3 t1, ref Vector3 t2, ref Vector3 t3)
        {
            float a = zCross2d(ref t1, ref t2, ref point);
            float b = zCross2d(ref t2, ref t3, ref point);
            float c = zCross2d(ref t3, ref t1, ref point);

            return a > 0 && b > 0 && c > 0 || a < 0 && b < 0 && c < 0;
        }
    }
}
