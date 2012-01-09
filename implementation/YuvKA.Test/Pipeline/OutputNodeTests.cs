namespace YuvKA.Test.Pipeline
{
	using System.Drawing;
	using Xunit;
	using YuvKA.Pipeline.Implementation;
	using YuvKA.VideoModel;

	public class OutputNodeTests
	{
		[Fact]
		public void TestDiagramNode()
		{
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize), new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
					inputs[1][x, y] = new Rgb((byte)(x * y), (byte)(x * y), (byte)(x * y));
				}
			}
			DiagramNode diaNode = new DiagramNode(0);
			diaNode.RefIndex = 0;
			DiagramGraph pixDiff = new DiagramGraph(1, new PixelDiff());
			DiagramGraph pSNR = new DiagramGraph(1, new PeakSignalNoiseRatio());
			diaNode.Graphs.Add(pixDiff);
			diaNode.Graphs.Add(pSNR);
			diaNode.ProcessCore(inputs, 0);
			Assert.Equal(diaNode.Graphs[0].Data[0], 93);
			Assert.Equal(diaNode.Graphs[1].Data[0], 87.5979796069714);
		}

		public void TestHistogramNodeRGB()
		{
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			HistogramNode histNodeR = new HistogramNode(HistogramType.R);
			HistogramNode histNodeG = new HistogramNode(HistogramType.G);
			HistogramNode histNodeB = new HistogramNode(HistogramType.B);
			histNodeR.Process(inputs, 0);
			histNodeG.Process(inputs, 0);
			histNodeB.Process(inputs, 0);
			int[] value = new int[3];
			int[,] intData = new int[256, 3];
			for (int x = 0; x < inputs[0].Size.Width; x++) {
				for (int y = 0; y < inputs[0].Size.Height; y++) {
					value[0] = inputs[0][x, y].R;
					value[1] = inputs[0][x, y].G;
					value[2] = inputs[0][x, y].B;
					intData[value[0], 0]++;
					intData[value[1], 1]++;
					intData[value[2], 2]++;
				}
			}
			int numberOfPixels = inputs[0].Size.Height * inputs[0].Size.Width;
			for (int i = 0; i < 256; i++) {
				Assert.Equal(histNodeR.Data[i], (double)(intData[i, 0] / numberOfPixels));
				Assert.Equal(histNodeG.Data[i], (double)(intData[i, 1] / numberOfPixels));
				Assert.Equal(histNodeB.Data[i], (double)(intData[i, 2] / numberOfPixels));
			}
		}

		public void TestHistogramNodeValue()
		{
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			HistogramNode histNodeValue = new HistogramNode(HistogramType.Value);
			histNodeValue.Process(inputs, 0);
			Color rgbValue;
			int value;
			int[] intData = new int[256];
			for (int x = 0; x < inputs[0].Size.Width; x++) {
				for (int y = 0; y < inputs[0].Size.Height; y++) {
					rgbValue = Color.FromArgb(inputs[0][x, y].R, inputs[0][x, y].G, inputs[0][x, y].B);
					value = (int)(rgbValue.GetBrightness() * 255);
					intData[value]++;
				}
			}
			int numberOfPixels = inputs[0].Size.Height * inputs[0].Size.Width;
			for (int i = 0; i < 256; i++) {
				Assert.Equal(histNodeValue.Data[i], (double)(intData[i] / numberOfPixels));
			}
		}
	}
}
