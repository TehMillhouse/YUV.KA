using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Moq;
using Xunit;
using YuvKA;
using YuvKA.Implementation;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.Test.Pipeline;
using YuvKA.Test.ViewModel;
using YuvKA.ViewModel;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Test
{
	public class GlobalTestT50
	{
		[Fact]
		public void Test50()
		{
			// Some necessary initializations.
			MainViewModel mvm = MainViewModelTest.GetInstance();
			var windowManMock = new Mock<IWindowManagerEx>();
			var vm = MainViewModelTest.GetInstance(cont => cont.ComposeExportedValue<IWindowManagerEx>(windowManMock.Object));
			var conductorMock = new Mock<IConductor>();


			// Step 1: Create simple pipeline.
			//	
			//	[color]---[overlay]
			//		   \ /
			//			X	
			//		   / \
			//  [noise]---[diagram]
			NoiseInputNode noise = new NoiseInputNode();
			mvm.Model.Graph.AddNode(noise);
			ColorInputNode color = new ColorInputNode();
			mvm.Model.Graph.AddNode(color);
			DiagramNode diagram = new DiagramNode();
			diagram.Inputs.Add(new Node.Input());
			diagram.Inputs.Add(new Node.Input());
			mvm.Model.Graph.AddNode(diagram);
			OverlayNode overlay = new OverlayNode();
			mvm.Model.Graph.AddNode(overlay);
			Assert.Contains(noise, mvm.Model.Graph.Nodes);
			Assert.Contains(color, mvm.Model.Graph.Nodes);
			Assert.Contains(diagram, mvm.Model.Graph.Nodes);
			Assert.Contains(overlay, mvm.Model.Graph.Nodes);

			mvm.Model.Graph.AddEdge(noise.Outputs[0], overlay.Inputs[0]);
			mvm.Model.Graph.AddEdge(noise.Outputs[0], diagram.Inputs[0]);
			mvm.Model.Graph.AddEdge(color.Outputs[0], overlay.Inputs[1]);
			mvm.Model.Graph.AddEdge(color.Outputs[0], diagram.Inputs[1]);
			Assert.Equal(noise.Outputs[0], overlay.Inputs[0].Source);
			Assert.Equal(noise.Outputs[0], diagram.Inputs[0].Source);
			Assert.Equal(color.Outputs[0], overlay.Inputs[1].Source);
			Assert.Equal(color.Outputs[0], diagram.Inputs[1].Source);

			// Step 2: Disable diagram node and open overlay node.
			diagram.IsEnabled = false;
			var overlayWindow = new OverlayViewModel(overlay) { Parent = conductorMock.Object };
			var diagramWindow = new DiagramViewModel(diagram) { Parent = conductorMock.Object };
			((IActivate)overlayWindow).Activate();
			conductorMock.Setup(c => c.DeactivateItem(overlayWindow, true))
				.Callback(() => ((IDeactivate)overlayWindow).Deactivate(close: true))
				.Verifiable();

			mvm.OpenWindow(overlayWindow);
			Assert.Contains(overlayWindow, mvm.OpenWindows);

			// Step 3: Start pipeline and add an overlay in the overlay node.
			mvm.Model.Start(mvm.Model.Graph.Nodes);
			overlay.Type = new ArtifactsOverlay();

			// Step 4: Remove selected overlay.
			overlay.Type = new NoOverlay();

			// Step 5: Stop pipeline and close overlay node.
			mvm.Model.Stop();
			mvm.CloseWindows(overlay);
			Assert.DoesNotContain(overlayWindow, mvm.OpenWindows);

			// Step 6: Re-enable diagram node and open it.
			diagram.IsEnabled = true;
			mvm.OpenWindow(diagramWindow);
			Assert.Contains(diagramWindow, mvm.OpenWindows);

			// Step 7: Choose a reference video and add a diagram graph.
			diagram.ReferenceVideo = diagram.Inputs[0];
			DiagramGraph diagramGraph = new DiagramGraph();
			diagram.Graphs.Add(diagramGraph);
			diagramGraph.Video = diagram.Inputs[1];
			diagramGraph.Type = new PixelDiff();

			// Step 8: Start pipeline again and change diagram graph type.
			mvm.Model.Start(mvm.Model.Graph.Nodes);
			diagramGraph.Type = new PeakSignalNoiseRatio();

			// Step 9: Remove the diagram graph.
			diagram.Graphs.Remove(diagramGraph);
			Assert.DoesNotContain(diagramGraph, diagram.Graphs);
		}
	}
}