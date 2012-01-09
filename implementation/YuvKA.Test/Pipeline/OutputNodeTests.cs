using YuvKA.Pipeline;
using System;

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
			
            AnonymousNode Source = new AnonymousNode(sourceNode, 2, null);
		    Frame[] inputs = Source.Process(null, 0);
		    YuvKA.Pipeline.Node.Input reference = new Node.Input();
		    reference.Source = Source.Outputs[0];
		    DiagramNode diaNode = new DiagramNode();
		    diaNode.ReferenceVideo = reference;
            diaNode.Inputs.Add(reference);
            YuvKA.Pipeline.Node.Input video = new Node.Input();
            video.Source = Source.Outputs[1];
            diaNode.Inputs.Add(video);
			DiagramGraph pixDiff = new DiagramGraph();
		    pixDiff.Video = video;
		    pixDiff.Type = new PixelDiff();
			DiagramGraph pSNR = new DiagramGraph();
		    pSNR.Video = video;
		    pSNR.Type = new PeakSignalNoiseRatio();
			diaNode.Graphs.Add(pixDiff);
			diaNode.Graphs.Add(pSNR);
			diaNode.ProcessCore(inputs, 0);
            double MSE = 0.0;
            double difference = 0.0;
            for (int x = 0; x < inputs[1].Size.Width; x++)
            {
                for (int y = 0; y < inputs[1].Size.Height; y++)
                {
                    difference += Math.Abs(inputs[1][x, y].R - inputs[0][x, y].R) + Math.Abs(inputs[0][x, y].G - inputs[0][x, y].G) +
                        Math.Abs(inputs[1][x, y].B - inputs[0][x, y].B);
                    MSE += Math.Pow(((inputs[1][x, y].R + inputs[1][x, y].G + inputs[1][x, y].B) - (inputs[0][x, y].R + inputs[0][x, y].G + inputs[0][x, y].B)), 2);
                }
            }
            difference = (double)difference / 3;
            MSE *= (double)1 / (3 * inputs[1].Size.Height * inputs[1].Size.Width);
		    double PSNR;
            if (MSE == 0.0)
                PSNR = 0.0;
            PSNR = 10 * Math.Log10((Math.Pow((Math.Pow(2, 24) - 1), 2)) / MSE);
			Assert.Equal(diaNode.Graphs[0].Data[0], difference);
			Assert.Equal(diaNode.Graphs[1].Data[0], PSNR);
		}

        private Frame[] sourceNode(Frame[] inputs, int tick)
        {
            YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
            Frame[] outputs = { new Frame(testSize), new Frame(testSize) };
            for (int x = 0; x < testSize.Width; x++)
            {
                for (int y = 0; y < testSize.Height; y++)
                {
                    outputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
                    outputs[1][x, y] = new Rgb((byte)(x * y), (byte)(x * y), (byte)(x * y));
                }
            }
            return outputs;
        }
        [Fact]
	    public void TestHistogramNodeRGB()
		{
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			HistogramNode histNodeR = new HistogramNode();
	        histNodeR.Type = HistogramType.R;
			HistogramNode histNodeG = new HistogramNode();
	        histNodeG.Type = HistogramType.G;
			HistogramNode histNodeB = new HistogramNode();
	        histNodeB.Type = HistogramType.B;
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
        [Fact]
		public void TestHistogramNodeValue()
		{
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			HistogramNode histNodeValue = new HistogramNode();
            histNodeValue.Type = HistogramType.Value;
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
