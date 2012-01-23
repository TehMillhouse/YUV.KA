using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class HistogramNode : OutputNode
	{
		public HistogramNode()
			: base(inputCount: 1)
		{
			Name = "Histogram";
			Data = new double[256];
		}

		[DataMember]
		[Browsable(true)]
		public HistogramType Type { get; set; }

		[DataMember]
		public double[] Data { get; private set; }

		[Browsable(true)]
		public HistogramViewModel Window { get { return new HistogramViewModel(this); } }

		public override void ProcessCore(Frame[] inputs, int tick)
		{
			if (Type == HistogramType.R) {
				CalculateR(inputs[0], tick);
			}
			else if (Type == HistogramType.G) {
				CalculateG(inputs[0], tick);
			}
			else if (Type == HistogramType.B) {
				CalculateB(inputs[0], tick);
			}
			else if (Type == HistogramType.Value) {
				CalculateValue(inputs[0], tick);
			}
		}

		private void CalculateR(Frame input, int tick)
		{
			int value;
			int[] intData = new int[256];
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					value = input[x, y].R;
					intData[value]++;
				}
			}
			int numberOfPixels = input.Size.Height * input.Size.Width;
			for (int i = 0; i < 256; i++) {
				Data[i] = (double)intData[i] / numberOfPixels;
			}
		}

		private void CalculateG(Frame input, int tick)
		{
			int value;
			int[] intData = new int[256];
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					value = input[x, y].G;
					intData[value]++;
				}
			}
			int numberOfPixels = input.Size.Height * input.Size.Width;
			for (int i = 0; i < 256; i++) {
				Data[i] = (double)intData[i] / numberOfPixels;
			}
		}

		private void CalculateB(Frame input, int tick)
		{
			int value;
			int[] intData = new int[256];
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					value = input[x, y].B;
					intData[value]++;
				}
			}
			int numberOfPixels = input.Size.Height * input.Size.Width;
			for (int i = 0; i < 256; i++) {
				Data[i] = (double)intData[i] / numberOfPixels;
			}
		}

		private void CalculateValue(Frame input, int tick)
		{
			Color rgbValue;
			int value;
			int[] intData = new int[256];
			for (int x = 0; x < input.Size.Width; x++) {
				for (int y = 0; y < input.Size.Height; y++) {
					/* Convert Frame Rgb struct to Color struct. */
					rgbValue = Color.FromArgb(input[x, y].R, input[x, y].G, input[x, y].B);
					/* Brightness (=Value) is stored as a float from 0.0 to 1.0, hence we have to convert to an int from 0 to 255. */
					value = (int)(rgbValue.GetBrightness() * 255);
					intData[value]++;
				}
			}
			int numberOfPixels = input.Size.Height * input.Size.Width;
			for (int i = 0; i < 256; i++) {
				Data[i] = (double)intData[i] / numberOfPixels;
			}
		}
	}
}
