using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Provides an input stream containing noise. The noise can be Perlin or Coherent.
	/// </summary>
	[DataContract]
	[Description("This Node provides an input stream containing noise")]
	public class NoiseInputNode : InputNode
	{
		static int[] p = { 151, 160, 137, 91, 90, 15,
			131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
			190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
			88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
			77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
			102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
			135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
			5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
			223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
			129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
			251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
			49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
			138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
			151, 160, 137, 91, 90, 15,
			131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
			190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
			88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
			77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
			102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
			135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
			5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
			223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
			129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
			251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
			49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
			138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };

		[DataMember]
		int seed;

		double scale;
		double speed;
		NoiseType type;
		Frame outputFrame;

		/// <summary>
		/// Creates a new NoiseInputNode with default values. 
		/// </summary>
		public NoiseInputNode()
			: base(outputCount: 1)
		{
			Name = "Noise";
			Speed = 0.05;
			Granularity = 0.05;
			seed = (new Random()).Next();
			outputFrame = new Frame(Size);
		}

		/// <summary>
		/// Gets or sets the type of noise. It can be Perlin or Coherent.
		/// </summary>
		[DataMember]
		[Browsable(true)]
		public NoiseType Type
		{
			get { return type; }
			set
			{
				type = value;
				NotifyOfPropertyChange(() => Type);
				NotifyOfPropertyChange(() => Granularity);
				NotifyOfPropertyChange(() => Speed);
			}
		}

		/// <summary>
		/// Gets or sets the rendering speed of Perlin noise.
		/// </summary>
		[DataMember]
		[Browsable(true)]
		[Range(0.0, 1.0)]
		public double? Speed
		{
			get
			{
				if (Type == NoiseType.Coherent || Type == NoiseType.ColoredCoherent) {
					return null;
				}
				return speed;
			}
			set { speed = value ?? speed; }
		}

		/// <summary>
		/// Gets or sets the rendering size of Perlin noise.
		/// </summary>
		[DataMember]
		[Browsable(true)]
		[Range(0.0, 1.0)]
		public double? Granularity
		{
			get
			{
				if (Type == NoiseType.Coherent || Type == NoiseType.ColoredCoherent) {
					return null;
				}
				return scale;
			}
			set { scale = value ?? scale; }
		}

		/// <summary>
		/// Returns a Frame with noise corresponding to the current tick in the stream.
		/// </summary>
		/// <param name="tick">The index of the current frame in the video stream</param>
		/// <returns>A tick-dependent noise Frame.</returns>
		public override Frame OutputFrame(int tick)
		{
			if (Type == NoiseType.Coherent) {
				outputFrame = ProcessCoherentNoise(tick);
			}
			else if (Type == NoiseType.ColoredCoherent) {
				outputFrame = ProcessColoredCoherentNoise(tick);
			}
			else if (Type == NoiseType.Perlin && Granularity != null && Speed != null) {
				outputFrame = ProcessPerlinNoise(tick);
			}
			else if (Granularity != null && Speed != null) {
				outputFrame = ProcessColoredPerlinNoise(tick);
			}

			return outputFrame;
		}

		private Frame ProcessCoherentNoise(int tick)
		{
			Random rnd = new Random(seed % (tick == 0 ? 1 : tick));
			for (int y = 0; y < Size.Height; ++y) {
				for (int x = 0; x < Size.Width; ++x) {
					byte color = (byte)rnd.Next(256);
					outputFrame[x, y] = new Rgb(color, color, color);
				}
			}
			return outputFrame;
		}

		private Frame ProcessColoredCoherentNoise(int tick)
		{
			Random rnd = new Random(seed % (tick == 0 ? 1 : tick));
			for (int y = 0; y < Size.Height; ++y) {
				for (int x = 0; x < Size.Width; ++x) {
					byte colorR = (byte)rnd.Next(256);
					byte colorG = (byte)rnd.Next(256);
					byte colorB = (byte)rnd.Next(256);
					outputFrame[x, y] = new Rgb(colorR, colorG, colorB);
				}
			}
			return outputFrame;
		}

		private Frame ProcessPerlinNoise(int tick)
		{
			if (Granularity == null || Speed == null) {
				return outputFrame;
			}
			double gran = (double)Granularity;
			double speed = (double)Speed;
			for (int y = 0; y < Size.Height; ++y) {
				for (int x = 0; x < Size.Width; ++x) {
					// Generate a noise function value, which is also tick-dependent
					double randomNumber = (Noise(x * (double)gran, y * (double)gran, (double)speed * tick) + 1) / 2;
					byte randomColor = (byte)(randomNumber * 256);
					outputFrame[x, y] = new Rgb(randomColor, randomColor, randomColor);
				}
			}
			return outputFrame;
		}

		private Frame ProcessColoredPerlinNoise(int tick)
		{
			if (Granularity == null || Speed == null) {
				return outputFrame;
			}
			double gran = (double)Granularity;
			double speed = (double)Speed;
			for (int y = 0; y < Size.Height; ++y) {
				for (int x = 0; x < Size.Width; ++x) {
					// Generate noise function values, which is also tick-dependent
					double randomNumberR = (Noise(x * (double)gran,
						y * (double)gran,
						(double)speed * tick) + 1) / 2;
					double randomNumberG = (Noise((x + 42) * (double)gran,
											(y - 42) * (double)gran,
											(double)speed * (tick + 42)) + 1) / 2;
					double randomNumberB = (Noise((x - 42) * (double)gran,
											(y + 42) * (double)gran,
											(double)speed * (tick - 42)) + 1) / 2;
					byte randomRed = (byte)(randomNumberR * 255);
					byte randomGreen = (byte)(randomNumberG * 255);
					byte randomBlue = (byte)(randomNumberB * 255);
					outputFrame[x, y] = new Rgb(randomRed, randomGreen, randomBlue);
				}
			}
			return outputFrame;
		}

		private double Grad(int hash, double x, double y, double z)
		{
			int h = hash & 15;                        // Convert lo 4 bits of hash code
			double u = h < 8 ? x : y,                 // into 12 gradient directions.
				   v = h < 4 ? y : h == 12 || h == 14 ? x : z;
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}

		private double Lerp(double t, double a, double b) { return a + t * (b - a); }

		private double Fade(double t) { return t * t * t * (t * (t * 6 - 15) + 10); }

		private double Noise(double x, double y, double z)
		{
			int xx = (int)Math.Floor(x) & 255;                 // Find unit cube that
			int yy = (int)Math.Floor(y) & 255;                 // contains point.
			int zz = (int)Math.Floor(z) & 255;

			x -= Math.Floor(x);                                // Find relative x, y, z
			y -= Math.Floor(y);                                // of point in cube.
			z -= Math.Floor(z);

			double u = Fade(x);                                // Compute fade curves
			double v = Fade(y);                                // for each of x, y, z.
			double w = Fade(z);

			int a = p[xx] + yy, aa = p[a] + zz, ab = p[a + 1] + zz,          // Hash coordinates of
				b = p[xx + 1] + yy, ba = p[b] + zz, bb = p[b + 1] + zz;      // the 8 cube corners,

			double g1 = Grad(p[aa], x, y, z);
			double g2 = Grad(p[ba], x - 1, y, z);
			double g3 = Grad(p[ab], x, y - 1, z);
			double g4 = Grad(p[bb], x - 1, y - 1, z);
			double g5 = Grad(p[aa + 1], x, y, z - 1);
			double g6 = Grad(p[ba + 1], x - 1, y, z - 1);
			double g7 = Grad(p[ab + 1], x, y - 1, z - 1);
			double g8 = Grad(p[bb + 1], x - 1, y - 1, z - 1);

			return Lerp(w, Lerp(v, Lerp(u, g1, g2), Lerp(u, g3, g4)), Lerp(v, Lerp(u, g5, g6), Lerp(u, g7, g8)));
		}

		public override void OnSizeChanged()
		{
			outputFrame = new Frame(Size);
		}
	}
}
