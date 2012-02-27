using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.Test.Pipeline;
using YuvKA.VideoModel;
using YuvKA.ViewModel;
using YuvKA.ViewModel.Implementation;
using Caliburn.Micro;

namespace YuvKA.Test.ViewModel
{
	public class DiagramViewModelTest
	{
		/// <summary>
		/// Checks whether the DiagramVM can add a line to its diagram.
		/// </summary>
		[Fact]
		public void CanAddLine()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] {new PixelDiff(), new IntraBlockFrequency()};

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			//var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);

			DVM.Reference = DVM.Videos[0];
			DVM.ChosenVideo = DVM.Videos[1];
			DVM.AddGraphControl();
			var gc = DVM.GraphControls.Single();
			gc.ChosenType = gc.DisplayTypes[0];
			
			Assert.NotEmpty(DVM.LineGraphs);
			Assert.NotEmpty(DVM.NodeModel.Graphs);
		}

		[Fact]
		public void CanDeleteLine()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);
			var annVid = new Node.Input { Source = sourceNode.Outputs[2] };
			DVM.NodeModel.Inputs.Add(annVid);

			DVM.Reference = DVM.Videos[0];
			DVM.ChosenVideo = DVM.Videos[1];
			DVM.AddGraphControl();
			var gc = DVM.GraphControls.Single();
			gc.ChosenType = gc.DisplayTypes[0];

			//DVM.NodeModel.ProcessCore(inputs, 0);

			Assert.NotEmpty(DVM.LineGraphs);
			Assert.NotEmpty(DVM.NodeModel.Graphs);

			DVM.DeleteGraphControl(gc);

			Assert.Empty(DVM.LineGraphs);
			Assert.Empty(DVM.NodeModel.Graphs);
		}

		[Fact]
		public void GetsData()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);

			DVM.Reference = DVM.Videos[0];
			DVM.ChosenVideo = DVM.Videos[1];
			DVM.AddGraphControl();
			var gc = DVM.GraphControls.Single();
			gc.ChosenType = gc.DisplayTypes[0];

			DVM.NodeModel.ProcessCore(inputs, 0);

			var dataBefore = DVM.LineGraphs.Single().PointDataSource;

			DVM.Handle(null);

			Assert.NotEqual(DVM.LineGraphs.Single().PointDataSource, dataBefore);
		}

		[Fact]
		public void ShowOnlyAvailableTypes()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff(), new DecisionDiff(), new IntraBlockFrequency(), new PeakSignalNoiseRatio() };

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);
			var annVid = new Node.Input { Source = sourceNode.Outputs[2] };
			DVM.NodeModel.Inputs.Add(annVid);

			DVM.ChosenVideo = DVM.Videos[1];
			DVM.AddGraphControl();
			var gcNoRefNoLog = DVM.GraphControls.ToList().Last();
			Assert.False(gcNoRefNoLog.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnReference || type.Item2.DependsOnLogfile));

			sourceNode.SettableOutputHasLogfile = true;
			DVM.ChosenVideo = DVM.Videos[2];
			DVM.AddGraphControl();
			var gcLogNoRef = DVM.GraphControls.ToList().Last();
			gcLogNoRef.SetDisplayTypes();
			Assert.False(gcLogNoRef.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnReference));
			Assert.True(gcLogNoRef.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnLogfile));
			sourceNode.SettableOutputHasLogfile = false;

			DVM.Reference = DVM.Videos[0];
			DVM.ChosenVideo = DVM.Videos[1];
			DVM.AddGraphControl();
			var gcLoggedRefNoLog = DVM.GraphControls.ToList().Last();
			gcLoggedRefNoLog.ReferenceHasLogfile = true;
			gcLoggedRefNoLog.SetDisplayTypes();
			Assert.True(gcLoggedRefNoLog.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnReference));
			Assert.False(gcLoggedRefNoLog.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnLogfile));

			sourceNode.SettableOutputHasLogfile = true;
			DVM.ChosenVideo = DVM.Videos[2];
			DVM.AddGraphControl();
			var gcLoggedRefLog = DVM.GraphControls.ToList().Last();
			Assert.True(gcLoggedRefLog.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnReference));
			Assert.True(gcLoggedRefLog.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnLogfile));
			Assert.True(gcLoggedRefLog.DisplayTypes.ToList().Exists(type => type.Item2.DependsOnAnnotatedReference));
		}

		[Fact]
		public void ResetChosenType()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff(), new PeakSignalNoiseRatio() };

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);

			DVM.Reference = DVM.Videos[0];
			DVM.ChosenVideo = DVM.Videos[1];
			DVM.AddGraphControl();
			var gc = DVM.GraphControls.Single();
			gc.ChosenType = gc.DisplayTypes[0];

			DVM.NodeModel.ProcessCore(inputs, 0);

			DVM.Handle(null);

			gc.ChosenType = gc.DisplayTypes[1];

			DVM.NodeModel.ProcessCore(inputs, 1);

			DVM.Handle(null);

			Assert.Equal(gc.DisplayTypes[0].Item2.Process(inputs[1], inputs[0]), gc.Graph.Data[0].Value);
			Assert.Equal(gc.DisplayTypes[1].Item2.Process(inputs[1], inputs[0]), gc.Graph.Data[1].Value);
		}

		[Fact]
		public void ResetReferenceWithExistingGraphControls()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);
			var altRef = new Node.Input { Source = sourceNode.Outputs[2] };
			DVM.NodeModel.Inputs.Add(altRef);

			DVM.Reference = DVM.Videos[0];
			DVM.ChosenVideo = DVM.Videos[1];
			DVM.AddGraphControl();
			var gc = DVM.GraphControls.Single();
			gc.ChosenType = gc.DisplayTypes[0];

			DVM.NodeModel.ProcessCore(inputs, 0);

			DVM.Handle(null);

			DVM.Reference = DVM.Videos[2];

			DVM.NodeModel.ProcessCore(inputs, 1);

			DVM.Handle(null);

			Assert.Equal(gc.DisplayTypes[0].Item2.Process(inputs[1], inputs[0]), gc.Graph.Data[0].Value);
			Assert.Equal(gc.DisplayTypes[0].Item2.Process(inputs[1], inputs[2]), gc.Graph.Data[1].Value);
		}

		[Fact]
		public void CannotAddGCWithoutChosenVid()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);
			var altRef = new Node.Input { Source = sourceNode.Outputs[2] };
			DVM.NodeModel.Inputs.Add(altRef);

			DVM.Reference = DVM.Videos[0];

			DVM.AddGraphControl();

			Assert.Empty(DVM.GraphControls);
		}

		[Fact]
		public void ChangeAnnReferenceToReferenceWithoutLog()
		{
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff(), new DecisionDiff() };

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);

			var annVid = new Node.Input { Source = sourceNode.Outputs[2] };
			DVM.NodeModel.Inputs.Add(annVid);

			DVM.Reference = DVM.Videos[0];
			DVM.ChosenVideo = DVM.Videos[2];
			DVM.AddGraphControl();
			var gc = DVM.GraphControls.Single();
			gc.ChosenType = gc.Types[1];

			DVM.NodeModel.ProcessCore(inputs, 0);

			DVM.Handle(null);

			DVM.Reference = DVM.Videos[1];

			DVM.NodeModel.ProcessCore(inputs, 1);

			DVM.Handle(null);

			Assert.Equal(0.0, gc.Graph.Data[1].Value);
		}
	}
}
