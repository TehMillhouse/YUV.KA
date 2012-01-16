using YuvKA.Pipeline;
using System;
using System.Windows;

namespace YuvKA.Test.Pipeline
{
	using System.Drawing;
	using Xunit;
	using YuvKA.Pipeline.Implementation;
	using VideoModel;
	using System.Collections.Generic;

	public class OutputNodeTests
	{
		[Fact]
		public void TestDiagramNode()
		{
			// Add Input Node for DiagramNode with 3 Outputs.
			AnonymousNode SourceNode = new AnonymousNode(sourceNode, 3);
			Frame[] inputs = SourceNode.Process(null, 0);

			// Generate DiagramNode and add referencevideo.
			Node.Input reference = new Node.Input();
			reference.Source = SourceNode.Outputs[0];
			DiagramNode diaNode = new DiagramNode();
			diaNode.ReferenceVideo = reference;
			diaNode.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			Node.Input video = new Node.Input();
			video.Source = SourceNode.Outputs[1];
			diaNode.Inputs.Add(video);
			Node.Input annVid = new Node.Input();
			annVid.Source = SourceNode.Outputs[2];
			diaNode.Inputs.Add(annVid);

			// Generate and add all GraphTypes to DiagramGraph once.
			DiagramGraph pixDiff = new DiagramGraph();
			pixDiff.Video = video;
			pixDiff.Type = new PixelDiff();
			DiagramGraph pSNR = new DiagramGraph();
			pSNR.Video = video;
			pSNR.Type = new PeakSignalNoiseRatio();
			DiagramGraph inBlFreq = new DiagramGraph();
			inBlFreq.Video = annVid;
			inBlFreq.Type = new IntraBlockFrequency();
			diaNode.Graphs.Add(pixDiff);
			diaNode.Graphs.Add(pSNR);
			diaNode.Graphs.Add(inBlFreq);

			diaNode.Process(inputs, 0);

			// Calculate expected results independently from DiagramGraph methods.
			double MSE = 0.0;
			double difference = 0.0;
			for (int x = 0; x < inputs[0].Size.Width; x++) {
				for (int y = 0; y < inputs[0].Size.Height; y++) {
					difference += Math.Abs(inputs[0][x, y].R - inputs[1][x, y].R) + Math.Abs(inputs[0][x, y].G - inputs[1][x, y].G) +
						Math.Abs(inputs[0][x, y].B - inputs[1][x, y].B);
					MSE += Math.Pow(((inputs[0][x, y].R + inputs[0][x, y].G + inputs[0][x, y].B) - (inputs[1][x, y].R +
						inputs[1][x, y].G + inputs[1][x, y].B)), 2);
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
			Assert.Equal(diaNode.Graphs[2].Data[0], 2);
		}

		/* A Process method to be used by AnonymousNodes.
		 * Generates an array of 2 Frames and 1 AnnotatedFrame with randomly filled Data */
		private Frame[] sourceNode(Frame[] inputs, int tick)
		{
			VideoModel.Size testSize = new VideoModel.Size(5, 5);
			Frame[] outputs = { new Frame(testSize), new Frame(testSize), generateAnnFrame() };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					outputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
					outputs[1][x, y] = new Rgb((byte)(x * y), (byte)(x * y), (byte)(x * y));
				}
			}
			return outputs;
		}

		/* Generates an AnnotatedFrame with randomly filled Data */
		private AnnotatedFrame generateAnnFrame()
		{
			VideoModel.Size testSize = new VideoModel.Size(8, 8);
			MacroblockDecision[,] decisions = {{
				new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Intra4x4},
				new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision = MacroblockPartitioning.Intra4x4},
				new MacroblockDecision { Movement = new Vector(0.0, 0.0), PartitioningDecision =  MacroblockPartitioning.Inter8x4}
			}};
			AnnotatedFrame output = new AnnotatedFrame(testSize, decisions);
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					output[x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			return output;
		}

		[Fact]
		public void TestHistogramNodeRGB()
		{
			// Generates an array of 1 Frame with randomly filled Data
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}

			// Generate all RGB HistogramNode once
			HistogramNode histNodeR = new HistogramNode();
			histNodeR.Type = HistogramType.R;
			HistogramNode histNodeG = new HistogramNode();
			histNodeG.Type = HistogramType.G;
			HistogramNode histNodeB = new HistogramNode();
			histNodeB.Type = HistogramType.B;

			histNodeR.Process(inputs, 0);
			histNodeG.Process(inputs, 0);
			histNodeB.Process(inputs, 0);

			// Calculate expected results independently from Histogram methods.
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
			// Generates an array of 1 Frame with randomly filled Data
			YuvKA.VideoModel.Size testSize = new YuvKA.VideoModel.Size(5, 5);
			Frame[] inputs = { new Frame(testSize) };
			for (int x = 0; x < testSize.Width; x++) {
				for (int y = 0; y < testSize.Height; y++) {
					inputs[0][x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}

			// Generate Value HistogramNode once
			HistogramNode histNodeValue = new HistogramNode();
			histNodeValue.Type = HistogramType.Value;

			histNodeValue.Process(inputs, 0);

			// Calculate expected results independently from Histogram method.
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

		/// <summary>
		/// Construct an AnnotatedFrame and use the BlockOverlay on top of it
		/// The Frame is completely gray and its MacroblockDecsions are ordered like this:
		///
		/// |-------------------------------|
		/// | Skip  | 16x16 | 16x8  |  8x16 |
		/// | Inter | Inter | Inter | Inter |
		/// |-------------------------------|
		/// |  8x8  |  4x8  |  8x4  |  4x4  |
		/// | Inter | Inter | Inter | Inter |
		/// |-------------------------------|
		/// | 16x16 |  8x8  |  4x4  |  Un-  |
		/// | Intra | Intra | Intra | known |
		/// |-------------------------------|
		///
		/// The Result has to be inspected manually
		/// </summary>
		[Fact]
		public void TestMacroBlockOverlay()
		{
			Frame testFrame = new Frame(new YuvKA.VideoModel.Size(64, 48));
			for (int x = 0; x < testFrame.Size.Width; x++) {
				for (int y = 0; y < testFrame.Size.Height; y++) {
					testFrame[x, y] = new Rgb(111, 111, 111);
				}
			}
			MacroblockDecision[] decisions = new MacroblockDecision[12];
			decisions[0] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.InterSkip };
			decisions[1] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter16x16 };
			decisions[2] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter16x8 };
			decisions[3] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter8x16 };
			decisions[4] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter8x8 };
			decisions[5] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter4x8 };
			decisions[6] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter8x4 };
			decisions[7] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter4x4 };
			decisions[8] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Intra16x16 };
			decisions[9] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Intra8x8 };
			decisions[10] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Intra4x4 };
			decisions[11] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Unknown };
			Frame[] input = { new AnnotatedFrame(testFrame, decisions) };
			OverlayNode Node = new OverlayNode { Type = new BlocksOverlay() };
			Node.ProcessCore(input, 0);
			List<Frame> output = new List<Frame>();
			output.Add(Node.Data);
			YuvEncoder.Encode(@"..\..\..\..\output\BlockOverlayTest_64x48.yuv", output);
		}

		/// <summary>
		/// Construct an AnnotatedFrame and use VectorOverlay on it
		/// The Frame is completely gray and the Vectors
		/// should look somewhatlike this
		/// |---------------|
		/// | | |  /|   |\  |
		/// | | | / |---| \ |
		/// | | |/  |   |  \|
		/// |---------------|
		/// | | |  /|   |\  |
		/// | | | / |---| \ |
		/// | | |/  |   |  \|
		/// |---------------|
		/// |   |   |   |   |
		/// | - | | | / | - |
		/// |   |   |   |   |
		/// |---------------|
		/// The result has to be inspected manually
		/// </summary>
		[Fact]
		public void TestVecorOverlay()
		{
			Frame testFrame = new Frame(new YuvKA.VideoModel.Size(64, 48));
			for (int x = 0; x < testFrame.Size.Width; x++) {
				for (int y = 0; y < testFrame.Size.Height; y++) {
					testFrame[x, y] = new Rgb(111, 111, 111);
				}
			}
			MacroblockDecision[] decisions = new MacroblockDecision[12];
			decisions[0] = new MacroblockDecision { Movement = new Vector(0.0, 12.0) };
			decisions[1] = new MacroblockDecision { Movement = new Vector(12.0, 12.0) };
			decisions[2] = new MacroblockDecision { Movement = new Vector(12.0, 0.0) };
			decisions[3] = new MacroblockDecision { Movement = new Vector(12.0, -12.0) };
			decisions[4] = new MacroblockDecision { Movement = new Vector(3.0, -12.0) };
			decisions[5] = new MacroblockDecision { Movement = new Vector(-38.0, -15.0) };
			decisions[6] = new MacroblockDecision { Movement = new Vector(-120.0, 0.0) };
			decisions[7] = new MacroblockDecision { Movement = new Vector(-20.0, 20.0) };
			decisions[8] = new MacroblockDecision { Movement = new Vector(4.0, 0.0) };
			decisions[9] = new MacroblockDecision { Movement = new Vector(0.0, 4.0) };
			decisions[10] = new MacroblockDecision { Movement = new Vector(4.0, 4.0) };
			decisions[11] = new MacroblockDecision { Movement = new Vector(-4.0, 0.0) };
			Frame[] input = { new AnnotatedFrame(testFrame, decisions) };
			OverlayNode Node = new OverlayNode { Type = new MoveVectorsOverlay() };
			Node.ProcessCore(input, 0);
			List<Frame> output = new List<Frame>();
			output.Add(Node.Data);
			YuvEncoder.Encode(@"..\..\..\..\output\VectorOverlayTest_64x48.yuv", output);
		}

		/// <summary>
		/// Create one monocolored gray Frame and make an altered Frame.
		/// Put both Frames into the ArtifactOverlay and choose the monocolored
		/// Frame as reference.
		/// The result has to be inspected manually.
		/// </summary>
		[Fact]
		public void ArtifactOverlay()
		{
			Frame testFrame = new Frame(new YuvKA.VideoModel.Size(80, 80));
			for (int x = 0; x < testFrame.Size.Width; x++) {
				for (int y = 0; y < testFrame.Size.Height; y++) {
					testFrame[x, y] = new Rgb(111, 111, 111);
				}
			}
			Frame alteredTestFrame = new Frame(testFrame.Size);
			for (int x = 0; x < testFrame.Size.Width; x++) {
				for (int y = 0; y < testFrame.Size.Height; y++) {
					alteredTestFrame[x, y] = new Rgb((byte)(x + y), (byte)(x + y), (byte)(x + y));
				}
			}
			Frame[] input = { alteredTestFrame, testFrame };
			OverlayNode Node = new OverlayNode { Type = new ArtifactsOverlay() };
			Node.ProcessCore(input, 0);
			List<Frame> output = new List<Frame>();
			output.Add(Node.Data);
			YuvEncoder.Encode(@"..\..\..\..\output\ArtifactOverlayTest_80x80.yuv", output);
		}
	}
}
