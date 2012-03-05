using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };
			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);

			DVM.Reference = DVM.Videos.ElementAt(0);
			DVM.ChosenVideo = DVM.Videos.ElementAt(1);
			DVM.AddGraph();

			var dgvm = DVM.Graphs.Single();
			// Set the ChosenType of the DiagramGraphViewModel to PixelDifference
			dgvm.CurrentType = dgvm.AvailableTypes.First();

			// Make sure that lines are automatically added.
			//Assert.NotEmpty(DVM.Lines);
			Assert.NotEmpty(DVM.NodeModel.Graphs);
		}

		/// <summary>
		/// Checks whether the DiagramVM can delete a line from its diagram.
		/// </summary>
		[Fact]
		public void CanDeleteLine()
		{
			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

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

			DVM.Reference = DVM.Videos.ElementAt(0);
			DVM.ChosenVideo = DVM.Videos.ElementAt(1);
			DVM.AddGraph();

			var dgvm = DVM.Graphs.Single();
			// Set the ChosenType of the DiagramGraphViewModel to PixelDifference
			dgvm.CurrentType = dgvm.AvailableTypes.First();

			Assert.NotEmpty(DVM.Lines);
			Assert.NotEmpty(DVM.NodeModel.Graphs);

			DVM.DeleteGraph(dgvm);

			Assert.Empty(DVM.Lines);
			Assert.Empty(DVM.NodeModel.Graphs);
		}

		/// <summary>
		/// Checks whether the calculated data is correctly relayed to the DiagramVM.
		/// </summary>
		[Fact]
		public void GetsData()
		{
			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);

			DVM.Reference = DVM.Videos.ElementAt(0);
			DVM.ChosenVideo = DVM.Videos.ElementAt(1);
			DVM.AddGraph();

			var dgvm = DVM.Graphs.Single();
			// Set the ChosenType of the DiagramGraphViewModel to PixelDifference
			dgvm.CurrentType = dgvm.AvailableTypes.First();

			DVM.NodeModel.ProcessCore(inputs, 0);

			var dataBefore = DVM.Lines.Single().DataSource;

			// data is drawn to the diagram by the DiagramVM
			DVM.Handle(null);

			Assert.NotEqual(DVM.Lines.Single().DataSource, dataBefore);
		}

		/// <summary>
		/// Checks whether the DiagramVM only displayes those DiagramTypes which are possible with the current configuration.
		/// </summary>
		[Fact]
		public void ShowOnlyAvailableTypes()
		{
			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff(), new DecisionDiff(), new IntraBlockFrequency(), new PeakSignalNoiseRatio() };

			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

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

			DVM.ChosenVideo = DVM.Videos.ElementAt(1);
			DVM.AddGraph();
			var dgvmNoRefNoLog = DVM.Graphs.Last();
			Assert.False(dgvmNoRefNoLog.AvailableTypes.ToList().Exists(type => type.Model.DependsOnReference || type.Model.DependsOnLogfile));

			sourceNode.SettableOutputHasLogfile = true;
			DVM.ChosenVideo = DVM.Videos.ElementAt(2);
			DVM.AddGraph();
			Assert.NotNull(DVM.Graphs);
			var dgvmLogNoRef = DVM.Graphs.Last();
			Assert.False(dgvmLogNoRef.AvailableTypes.ToList().Exists(type => type.Model.DependsOnReference));
			Assert.True(dgvmLogNoRef.AvailableTypes.ToList().Exists(type => type.Model.DependsOnLogfile));
			sourceNode.SettableOutputHasLogfile = false;

			DVM.Reference = DVM.Videos.ElementAt(0);
			DVM.ChosenVideo = DVM.Videos.ElementAt(1);
			DVM.AddGraph();
			var dgvmLoggedRefNoLog = DVM.Graphs.Last();
			Assert.True(dgvmLoggedRefNoLog.AvailableTypes.ToList().Exists(type => type.Model.DependsOnReference));
			Assert.False(dgvmLoggedRefNoLog.AvailableTypes.ToList().Exists(type => type.Model.DependsOnLogfile));

			sourceNode.SettableOutputHasLogfile = true;
			DVM.ChosenVideo = DVM.Videos.ElementAt(2);
			DVM.AddGraph();
			var dgvmLoggedRefLog = DVM.Graphs.Last();
			Assert.True(dgvmLoggedRefLog.AvailableTypes.ToList().Exists(type => type.Model.DependsOnReference));
			Assert.True(dgvmLoggedRefLog.AvailableTypes.ToList().Exists(type => type.Model.DependsOnLogfile));
			Assert.True(dgvmLoggedRefLog.AvailableTypes.ToList().Exists(type => type.Model.DependsOnAnnotatedReference));
		}

		/// <summary>
		/// Checks whether the DiagramVM can change the DiagramType chosen by the user at runtime.
		/// </summary>
		[Fact]
		public void ResetChosenType()
		{
			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff(), new PeakSignalNoiseRatio() };

			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

			// Add Input Node for DiagramNode with 3 Outputs.
			var sourceNode = new AnonymousNode(AnonNodeHelper.SourceNode, 3);
			var inputs = sourceNode.Process(null, 0);

			// Add reference video to node.
			var reference = new Node.Input { Source = sourceNode.Outputs[0] };
			DVM.NodeModel.Inputs.Add(reference);

			// Add other Outputs as Inputs to DiagramNode.
			var video = new Node.Input { Source = sourceNode.Outputs[1] };
			DVM.NodeModel.Inputs.Add(video);

			DVM.Reference = DVM.Videos.ElementAt(0);
			DVM.ChosenVideo = DVM.Videos.ElementAt(1);
			DVM.AddGraph();

			var dgvm = DVM.Graphs.Single();
			// ChosenType = PixelDifference
			dgvm.CurrentType = dgvm.AvailableTypes.First();

			// porcess one frame with PixelDifference
			DVM.NodeModel.ProcessCore(inputs, 0);
			DVM.Handle(null);

			// ChosenType = PeakSignalNoiseRatio
			dgvm.CurrentType = dgvm.AvailableTypes.ElementAt(1);

			// porcess one frame with PeakSignalNoiseRatio
			DVM.NodeModel.ProcessCore(inputs, 1);
			DVM.Handle(null);

			Assert.Equal(dgvm.AvailableTypes.ElementAt(0).Model.Process(inputs[1], inputs[0]), dgvm.Model.Data[0].Value);
			Assert.Equal(dgvm.AvailableTypes.ElementAt(1).Model.Process(inputs[1], inputs[0]), dgvm.Model.Data[1].Value);
		}

		/// <summary>
		/// Checks whether the GraphControls of the DiagramVM are updated if the reference video is changed.
		/// </summary>
		[Fact]
		public void ResetReferenceWithExistingGraphControls()
		{
			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

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

			DVM.Reference = DVM.Videos.ElementAt(0);
			DVM.ChosenVideo = DVM.Videos.ElementAt(1);
			DVM.AddGraph();

			var dgvm = DVM.Graphs.Single();
			// ChosenType = PixelDifference
			dgvm.CurrentType = dgvm.AvailableTypes.ElementAt(0);

			// porcess one frame with first reference
			DVM.NodeModel.ProcessCore(inputs, 0);
			DVM.Handle(null);

			DVM.Reference = DVM.Videos.ElementAt(2);

			// porcess one frame with second reference
			DVM.NodeModel.ProcessCore(inputs, 1);
			DVM.Handle(null);

			Assert.Equal(dgvm.AvailableTypes.ElementAt(0).Model.Process(inputs[1], inputs[0]), dgvm.Model.Data[0].Value);
			Assert.Equal(dgvm.AvailableTypes.ElementAt(0).Model.Process(inputs[1], inputs[2]), dgvm.Model.Data[1].Value);
		}

		/// <summary>
		/// Checks that a DiagramGraphViewModel is not added if no video is chosen.
		/// </summary>
		[Fact]
		public void CannotAddDgvmWithoutChosenVid()
		{
			// Set the GraphTypes
			IoC.GetAllInstances = type => new IGraphType[] { new PixelDiff() };

			// Get the parent PipelineVM and add DiagramNode
			var vm = MainViewModelTest.GetInstance().PipelineViewModel;
			var node = new NodeViewModel(new DiagramNode(), vm);
			vm.Nodes.Add(node);
			vm.Parent.Model.Graph.AddNode(node.Model);

			var DVM = new DiagramViewModel((DiagramNode)vm.Nodes.Single().Model);

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

			DVM.Reference = DVM.Videos.ElementAt(0);
			// No video chosen.
			Assert.False(DVM.CanAddGraph);
		}
	}
}