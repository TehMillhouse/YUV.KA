using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Microsoft.Win32;
using Moq;
using Xunit;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.Test.ViewModel;
using YuvKA.ViewModel;
using System.Runtime.Serialization;

namespace YuvKA.Test
{
	public class GlobalTestT30
	{
		[Fact]
		public void Test30()
		{
			//Step 1: Construct any Pipeline
			MainViewModel mvm = MainViewModelTest.GetInstance();
			ColorInputNode colorInput = new ColorInputNode();
			mvm.Model.Graph.AddNode(colorInput);
			InverterNode inverter = new InverterNode();
			mvm.Model.Graph.AddNode(inverter);
			mvm.Model.Graph.AddEdge(colorInput.Outputs[0], inverter.Inputs[0]);
			Assert.Contains(colorInput, mvm.Model.Graph.Nodes);
			Assert.Contains(inverter, mvm.Model.Graph.Nodes);
			Assert.Equal(colorInput.Outputs[0], inverter.Inputs[0].Source);
			
			//Step 2: Save Pipeline
			using (var stream = File.Create(@"..\..\..\..\output\test30.yuvka"))
				new NetDataContractSerializer().Serialize(stream, mvm.Model);
			Assert.True(File.Exists(@"..\..\..\..\output\test30.yuvka"));
			
			//Step 3: Clear Pipeline
			mvm.Clear();
			Assert.Empty(mvm.Model.Graph.Nodes);
			
			//Step 4: Reload Pipeline
			using (var stream = File.OpenRead(@"..\..\..\..\output\test30.yuvka"))
				mvm.Model = (PipelineState)new NetDataContractSerializer().Deserialize(stream);
			Assert.True(mvm.Model.Graph.Nodes[0] is ColorInputNode);
			Assert.True(mvm.Model.Graph.Nodes[1] is InverterNode);
			Assert.Equal(mvm.Model.Graph.Nodes[0].Outputs[0], mvm.Model.Graph.Nodes[1].Inputs[0].Source);
		}
	}
}
