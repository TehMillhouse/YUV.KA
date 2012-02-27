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
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] {new PixelDiff()};

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			
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
			// Set the ChosenType of the GraphControl to PixelDifference
			gc.ChosenType = gc.DisplayTypes[0];
			
			// Make sure that lines are automatically added.
			Assert.NotEmpty(DVM.LineGraphs);
			Assert.NotEmpty(DVM.NodeModel.Graphs);
		}

		/// <summary>
		/// Checks whether the DiagramVM can delete a line from its diagram.
		/// </summary>
		[Fact]
		public void CanDeleteLine()
		{
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Set the GraphTypes
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
			// ChosenType = PixelDifference
			gc.ChosenType = gc.DisplayTypes[0];
			
			Assert.NotEmpty(DVM.LineGraphs);
			Assert.NotEmpty(DVM.NodeModel.Graphs);

			DVM.DeleteGraphControl(gc);

			Assert.Empty(DVM.LineGraphs);
			Assert.Empty(DVM.NodeModel.Graphs);
		}

		/// <summary>
		/// Checks whether the calculated data is correctly relayed to the DiagramVM.
		/// </summary>
		[Fact]
		public void GetsData()
		{
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Set the GraphTypes
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
			// ChosenType = PixelDifference
			gc.ChosenType = gc.DisplayTypes[0];

			DVM.NodeModel.ProcessCore(inputs, 0);

			var dataBefore = DVM.LineGraphs.Single().PointDataSource;

			// data is drawn to the diagram by the DiagramVM
			DVM.Handle(null);

			Assert.NotEqual(DVM.LineGraphs.Single().PointDataSource, dataBefore);
		}

		/// <summary>
		/// Checks whether the DiagramVM only displayes those DiagramTypes which are possible with the current configuration.
		/// </summary>
		[Fact]
		public void ShowOnlyAvailableTypes()
		{
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Set the GraphTypes
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

		/// <summary>
		/// Checks whether the DiagramVM can change the DiagramType chosen by the user at runtime.
		/// </summary>
		[Fact]
		public void ResetChosenType()
		{
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Set the GraphTypes
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
			// ChosenType = PixelDifference
			gc.ChosenType = gc.DisplayTypes[0];

			// porcess one frame with PixelDifference
			DVM.NodeModel.ProcessCore(inputs, 0);
			DVM.Handle(null);

			// ChosenType = PeakSignalNoiseRatio
			gc.ChosenType = gc.DisplayTypes[1];

			// porcess one frame with PeakSignalNoiseRatio
			DVM.NodeModel.ProcessCore(inputs, 1);
			DVM.Handle(null);

			Assert.Equal(gc.DisplayTypes[0].Item2.Process(inputs[1], inputs[0]), gc.Graph.Data[0].Value);
			Assert.Equal(gc.DisplayTypes[1].Item2.Process(inputs[1], inputs[0]), gc.Graph.Data[1].Value);
		}

		/// <summary>
		/// Checks whether the GraphControls of the DiagramVM are updated if the reference video is changed.
		/// </summary>
		[Fact]
		public void ResetReferenceWithExistingGraphControls()
		{
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Set the GraphTypes
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
			// ChosenType = PixelDifference
			gc.ChosenType = gc.DisplayTypes[0];

			// porcess one frame with first reference
			DVM.NodeModel.ProcessCore(inputs, 0);
			DVM.Handle(null);

			DVM.Reference = DVM.Videos[2];

			// porcess one frame with second reference
			DVM.NodeModel.ProcessCore(inputs, 1);
			DVM.Handle(null);

			Assert.Equal(gc.DisplayTypes[0].Item2.Process(inputs[1], inputs[0]), gc.Graph.Data[0].Value);
			Assert.Equal(gc.DisplayTypes[0].Item2.Process(inputs[1], inputs[2]), gc.Graph.Data[1].Value);
		}

		/// <summary>
		/// Checks that a GraphControl is not added if no video is chosen.
		/// </summary>
		[Fact]
		public void CannotAddGcWithoutChosenVid()
		{
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Set the GraphTypes
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
			// No video chosen.

			DVM.AddGraphControl();

			Assert.Empty(DVM.GraphControls);
		}
	}
}
