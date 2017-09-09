using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace AltarSteganography {
	public class Steganograph : Stream {
		private readonly Bitmap Image;
		private readonly int ImageWidth;
		private readonly int ImageHeight;
		private readonly int ImageDepth;
		private readonly bool DemoReveal;
		private readonly byte AlphaThresold;
		private readonly byte[] PixelsMap;

		private BitmapData ImageMap;
		private IntPtr ImagePtr;
		private int IndexX;
		private int IndexY;
		private int IndexRGB;
		private byte[] Pixel;
		private long length;

		public bool IsMoreAvailable { get; private set; }

		public Steganograph(Bitmap img, byte alphaThresold = 0, bool demoRevealUsedPixels = false) : base() {
			Image = img;
			ImageDepth = Bitmap.GetPixelFormatSize(Image.PixelFormat);
			if (ImageDepth != 32)
				throw new BadImageFormatException("Only 32bpp images are supported.");
			AlphaThresold = alphaThresold;
			DemoReveal = demoRevealUsedPixels;
			ImageWidth = Image.Width;
			ImageHeight = Image.Height;
			ImageMap = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.ReadWrite, Image.PixelFormat);
			int step = ImageDepth / 8;
			PixelsMap = new byte[(ImageWidth * ImageHeight) * step];
			ImagePtr = ImageMap.Scan0;
			Marshal.Copy(ImagePtr, PixelsMap, 0, PixelsMap.Length);
			Reset();
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing && ImageMap != null) {
				Marshal.Copy(PixelsMap, 0, ImagePtr, PixelsMap.Length);
				Image.UnlockBits(ImageMap);
				ImageMap = null;
			}
		}

		public Color GetPixel(int x, int y) {
			int cCount = ImageDepth / 8;
			int i = y * ImageMap.Stride + x * cCount;
			if (i > PixelsMap.Length - cCount)
				throw new IndexOutOfRangeException("You are trying to read or write outside of the image's bounds.");
			switch (ImageDepth) {
				case 32: { // For 32 bpp get Red, Green, Blue and Alpha
					byte b = PixelsMap[i];
					byte g = PixelsMap[i + 1];
					byte r = PixelsMap[i + 2];
					byte a = PixelsMap[i + 3];
					return Color.FromArgb(a, r, g, b);
				}
				case 24: { // For 24 bpp get Red, Green and Blue
					byte b = PixelsMap[i];
					byte g = PixelsMap[i + 1];
					byte r = PixelsMap[i + 2];
					return Color.FromArgb(r, g, b);
				}
				case 8: { // For 8 bpp get color value (Red, Green and Blue values are the same)
					byte c = PixelsMap[i];
					return Color.FromArgb(c, c, c);
				}
				default:
					return Color.Empty;
			}
		}

		public void SetPixel(int x, int y, Color color) {
			int cCount = ImageDepth / 8;
			int i = y * ImageMap.Stride + x * cCount;
			switch (ImageDepth) {
				case 32: // For 32 bpp set Red, Green, Blue and Alpha
					PixelsMap[i] = color.B;
					PixelsMap[i + 1] = color.G;
					PixelsMap[i + 2] = color.R;
					PixelsMap[i + 3] = DemoReveal ? (byte)255 : color.A;
					break;
				case 24: // For 24 bpp set Red, Green and Blue
					PixelsMap[i] = color.B;
					PixelsMap[i + 1] = color.G;
					PixelsMap[i + 2] = color.R;
					break;
				case 8: // For 8 bpp set color value (Red, Green and Blue values are the same)
					PixelsMap[i] = color.B;
					break;
			}
		}

		private byte? ReadNextByte() {
			if (IsMoreAvailable) {
				var o = Peek();
				IndexRGB = (IndexRGB + 1) % 3;
				if (IndexRGB == 0)
					IsMoreAvailable = SetToNextAlphaPixel();
				return o;
			}
			return null;
		}

		private bool WriteNextByte(byte o) {
			if (IsMoreAvailable) {
				Pixel[IndexRGB + 1] = o;
				SetPixel(IndexX, IndexY, Color.FromArgb(BigEndianToInteger(Pixel)));
				IndexRGB = (IndexRGB + 1) % 3;
				if (IndexRGB == 0)
					IsMoreAvailable = SetToNextAlphaPixel();
				return true;
			}
			return false;
		}

		public byte Peek() {
			return Pixel[IndexRGB + 1];
		}

		public void Reset() {
			IndexX = -1;
			IndexY = 0;
			IndexRGB = 0;
			Pixel = null;
			IsMoreAvailable = true;
			if (SetToNextAlphaPixel() == false)
				throw new BadImageFormatException("There is no transparent pixel in the image.");
		}

		private bool SetToNextAlphaPixel() {
			while (true) {
				IndexX++;
				if (IndexX == ImageWidth) {
					IndexX = 0;
					IndexY++;
					if (IndexY == ImageHeight)
						return false;
				}
				var c = GetPixel(IndexX, IndexY);
				if (c.A <= AlphaThresold) {
					Pixel = IntegerToBigEndian(c.ToArgb());
					return true;
				}
			}
		}

		byte[] IntegerToBigEndian(int data) {
			var b = new byte[4];
			b[0] = (byte)(((uint)data >> 24) & 0xFF);
			b[1] = (byte)(((uint)data >> 16) & 0xFF);
			b[2] = (byte)(((uint)data >> 8) & 0xFF);
			b[3] = (byte)data;
			return b;
		}

		int BigEndianToInteger(byte[] data) {
			return (data[3])
				 | (data[2] << 8)
				 | (data[1] << 16)
				 | data[0] << 24;
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return true; }
		}

		public override void Flush() {

		}

		public override long Length {
			get {
				if (length == 0) {
					var backup = new {
						X = IndexX,
						Y = IndexY,
						RGB = IndexRGB,
						Pixel = Pixel,
						More = IsMoreAvailable
					};
					Reset();
					length = 3;

					while (SetToNextAlphaPixel())
						length += 3;

					IndexX = backup.X;
					IndexY = backup.Y;
					IndexRGB = backup.RGB;
					Pixel = backup.Pixel;
					IsMoreAvailable = backup.More;
				}
				return length;
			}
		}

		public override long Position {
			get {
				return IndexX * IndexY;
			}
			set {
				throw new IOException("Cannot seek.");
			}
		}

		public override int Read(byte[] buffer, int offset, int count) {
			for (int i = offset; i < offset + count; i++) {
				var o = ReadNextByte();
				if (o == null)
					return i - offset;
				buffer[i] = o.Value;
			}
			return count;
		}

		public override long Seek(long offset, SeekOrigin origin) {
			throw new IOException("Cannot seek.");
		}

		public override void SetLength(long value) {
			throw new IOException("Cannot set length due to the nature of the stream.");
		}

		public override void Write(byte[] buffer, int offset, int count) {
			for (int i = offset; i < offset + count; i++)
				if (WriteNextByte(buffer[i]) == false)
					throw new IndexOutOfRangeException("No more pixel to write to.");
		}
	}
}
