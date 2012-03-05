using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;


namespace YuvKA.Test.Pipeline
{
	public class OutputNodeTests
	{
		/// <summary>
		/// Tests the general function of the DiagramNode by creating a DiagramNode
		///  and adding three Outputs and  recalculating the expected results 
		/// to test each current Graphtype.
		/// </summary>
		[Fact]
		public void TestDiagramNode()
		{
			// Add Input Node for DiagramNode with 3 Outputs.
			AnonymousNode sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			Frame[] inputs = sourceNode.Process(null, 0);

			// Generate DiagramNode and add referencevideo.
			Node.Input reference = new Node.Input();
			reference.Source = sourceNode.Outputs[0];
			DiagramNode diaNode = new DiagramNode();
			diaNode.ReferenceVideo = reference;
			diaNode.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			Node.Input video = new Node.Input();
			video.Source = sourceNode.Outputs[1];
			diaNode.Inputs.Add(video);
			Node.Input annVid = new Node.Input();
			annVid.Source = sourceNode.Outputs[2];
			diaNode.Inputs.Add(annVid);

			// Generate and add all GraphTypes to DiagramGraph once.
			DiagramGraph pixDiff = new DiagramGraph();
			pixDiff.Video = video;
			pixDiff.Type = new PixelDiff();
			DiagramGraph pseudoNoiseRatio = new DiagramGraph();
			pseudoNoiseRatio.Video = video;
			pseudoNoiseRatio.Type = new PeakSignalNoiseRatio();
			DiagramGraph inBlFreq = new DiagramGraph();
			inBlFreq.Video = annVid;
			inBlFreq.Type = new IntraBlockFrequency();
			DiagramGraph decDiff = new DiagramGraph();
			decDiff.Video = annVid;
			decDiff.Type = new DecisionDiff();
			diaNode.Graphs.Add(pixDiff);
			diaNode.Graphs.Add(pseudoNoiseRatio);
			diaNode.Graphs.Add(inBlFreq);
			diaNode.Graphs.Add(decDiff);

			diaNode.Process(inputs, 0);

			// Calculate expected results independently from DiagramGraph methods.
			double mse = 0.0;
			double pixDifference = 0.0;
			for (int x = 0; x < inputs[0].Size.Width; x++) {
				for (int y = 0; y < inputs[0].Size.Height; y++) {
					pixDifference += Math.Abs(inputs[0][x, y].R - inputs[1][x, y].R) + Math.Abs(inputs[0][x, y].G - inputs[1][x, y].G) +
						Math.Abs(inputs[0][x, y].B - inputs[1][x, y].B);
					mse += Math.Pow(((inputs[0][x, y].R + inputs[0][x, y].G + inputs[0][x, y].B) - (inputs[1][x, y].R +
						inputs[1][x, y].G + inputs[1][x, y].B)), 2);
				}
			}

			double decDifference = 0.0;
			for (int i = 0; i < ((AnnotatedFrame)inputs[2]).Decisions.GetLength(0); i++) {
				for (int j = 0; j < ((AnnotatedFrame)inputs[2]).Decisions.GetLength(1); j++) {
					if (((AnnotatedFrame)inputs[2]).Decisions[i, j].Equals(((AnnotatedFrame)inputs[0]).Decisions[i, j]))
						decDifference++;
				}
			}
			decDifference /= ((AnnotatedFrame)inputs[2]).Decisions.Length;

			pixDifference = pixDifference / (3 * inputs[0].Size.Height * inputs[0].Size.Width);
			mse *= (double)1 / (3 * inputs[1].Size.Height * inputs[1].Size.Width);
			double psnr;
			if (mse == 0.0)
				psnr = 0.0;
			psnr = 10 * Math.Log10((Math.Pow((Math.Pow(2, 24) - 1), 2)) / mse);

			Assert.Equal(diaNode.Graphs[0].Data[0].Value, pixDifference);
			Assert.Equal(diaNode.Graphs[1].Data[0].Value, psnr);
			Assert.Equal(diaNode.Graphs[2].Data[0].Value, 2);
			Assert.Equal(diaNode.Graphs[3].Data[0].Value, decDifference);
		}

		/// <summary>
		/// Tests the RGB-functions of the HistogramNode. Creates a sample pipeline with a HistogramNode as
		/// output and recalculates the expected results.
		/// </summary>
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
				Assert.Equal(histNodeR.Data[i], (double)intData[i, 0] / numberOfPixels);
				Assert.Equal(histNodeG.Data[i], (double)intData[i, 1] / numberOfPixels);
				Assert.Equal(histNodeB.Data[i], (double)intData[i, 2] / numberOfPixels);
			}
		}

		/// <summary>
		/// Test the Value function of the HistogramNode. Creates a sample pipeline with a HistogramNode as
		/// output and recalculates the expected results.
		/// </summary>
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
				Assert.Equal(histNodeValue.Data[i], (double)intData[i] / numberOfPixels);
			}
		}

		/// <summary>
		/// Tests whether the DiagramNode works if it is not enabled.
		/// </summary>
		[Fact]
		public void TestNoDrawingIfDiagramNodeNotEnabled()
		{
			// Add Input Node for DiagramNode with 3 Outputs.
			AnonymousNode sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			Frame[] inputs = sourceNode.Process(null, 0);

			// Generate DiagramNode and add referencevideo.
			Node.Input reference = new Node.Input();
			reference.Source = sourceNode.Outputs[0];
			DiagramNode diaNode = new DiagramNode();
			diaNode.ReferenceVideo = reference;
			diaNode.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			Node.Input video = new Node.Input();
			video.Source = sourceNode.Outputs[1];
			diaNode.Inputs.Add(video);

			// Generate sample GraphType to DiagramGraph.
			DiagramGraph pixDiff = new DiagramGraph();
			pixDiff.Video = video;
			pixDiff.Type = new PixelDiff();
			diaNode.Graphs.Add(pixDiff);

			diaNode.IsEnabled = false;

			diaNode.Process(inputs, 0);

			Assert.Empty(diaNode.Graphs[0].Data);
		}

		/// <summary>
		/// Tests whether the DiagramNode empties the current values of its graphs 
		/// if the tick is set before the current point in time.
		/// </summary>
		[Fact]
		public void RedrawOnTickSetBack()
		{
			// Add Input Node for DiagramNode with 3 Outputs.
			AnonymousNode sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			Frame[] inputs = sourceNode.Process(null, 0);

			// Generate DiagramNode and add referencevideo.
			Node.Input reference = new Node.Input();
			reference.Source = sourceNode.Outputs[0];
			DiagramNode diaNode = new DiagramNode();
			diaNode.ReferenceVideo = reference;
			diaNode.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			Node.Input video = new Node.Input();
			video.Source = sourceNode.Outputs[1];
			diaNode.Inputs.Add(video);

			// Generate sample GraphType to DiagramGraph.
			DiagramGraph pixDiff = new DiagramGraph();
			pixDiff.Video = video;
			pixDiff.Type = new PixelDiff();
			diaNode.Graphs.Add(pixDiff);

			diaNode.Process(inputs, 0);
			diaNode.Process(inputs, 1);
			Assert.Equal(diaNode.Graphs[0].Data.Count, 2);
			diaNode.Process(inputs, 0);
			Assert.Equal(diaNode.Graphs[0].Data.Count, 1);
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
		/// | 8x8or |  PCM  | null  |  Un-  |
		/// | below | Intra |       | known |
		/// |-------------------------------|
		///
		/// The Result has to be inspected manually
		/// </summary>
		[Fact]
		public void TestMacroBlockOverlay()
		{
			Frame testFrame = new Frame(new YuvKA.VideoModel.Size(64, 64));
			for (int x = 0; x < testFrame.Size.Width; x++) {
				for (int y = 0; y < testFrame.Size.Height; y++) {
					testFrame[x, y] = new Rgb(111, 111, 111);
				}
			}
			MacroblockDecision[] decisions = new MacroblockDecision[16];
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
			decisions[12] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Inter8x8OrBelow };
			decisions[13] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.IntraPCM };
			decisions[14] = new MacroblockDecision { PartitioningDecision = null };
			decisions[15] = new MacroblockDecision { PartitioningDecision = MacroblockPartitioning.Unknown };
			Frame[] input = { new AnnotatedFrame(testFrame, decisions) };
			OverlayNode node = new OverlayNode { Type = new BlocksOverlay() };
			node.ProcessCore(input, 0);
			List<Frame> output = new List<Frame>();
			output.Add(node.Data);
			YuvEncoder.Encode(@"..\..\..\..\output\BlockOverlayTest_64x64.yuv", output);
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
			OverlayNode node = new OverlayNode { Type = new MoveVectorsOverlay() };
			node.ProcessCore(input, 0);
			List<Frame> output = new List<Frame>();
			output.Add(node.Data);
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
			OverlayNode node = new OverlayNode { Type = new ArtifactsOverlay() };
			node.ProcessCore(input, 0);
			List<Frame> output = new List<Frame>();
			output.Add(node.Data);
			YuvEncoder.Encode(@"..\..\..\..\output\ArtifactOverlayTest_80x80.yuv", output);
		}

		/// <summary>
		/// Tests if noOverlay really has no effect
		/// </summary>
		[Fact]
		public void TestNoOverlay()
		{
			Frame testFrame = new Frame(new YuvKA.VideoModel.Size(80, 80));
			for (int x = 0; x < testFrame.Size.Width; x++) {
				for (int y = 0; y < testFrame.Size.Height; y++) {
					testFrame[x, y] = new Rgb(111, 111, 111);
				}
			}
			Frame[] input = { testFrame };
			OverlayNode node = new OverlayNode { Type = new NoOverlay() };
			node.ProcessCore(input, 0);
			List<Frame> output = new List<Frame>();
			for (int x = 0; x < testFrame.Size.Width; x++) {
				for (int y = 0; y < testFrame.Size.Height; y++) {
					Assert.Equal(testFrame[x, y], node.Data[x, y]);
				}
			}
		}

		/// <summary>
		/// Test miscellaneous functions of OverlayNode
		/// </summary>
		[Fact]
		public void TestOverlayNode()
		{
			OverlayNode node = new OverlayNode();
			Assert.Equal(false, node.InputIsValid);
			NoiseInputNode noise = new NoiseInputNode();
			node.Inputs[0].Source = noise.Outputs[0];
			Assert.Equal(true, node.InputIsValid);
			Frame[] input = { new Frame(new YuvKA.VideoModel.Size(5, 5)) };
			node.Type = new ArtifactsOverlay();
			node.ProcessCore(input, 0);
			node.Type = new BlocksOverlay();
			node.ProcessCore(input, 0);
			node.Type = new MoveVectorsOverlay();
			node.ProcessCore(input, 0);
		}
	}
}
