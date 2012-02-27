using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using Caliburn.Micro;
using Xunit;
using YuvKA;
using System.Linq;
using System.Collections.ObjectModel;
using YuvKA.ViewModel.PropertyEditor.Implementation;
using YuvKA.ViewModel.PropertyEditor;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.Test.ViewModel.PropertyEditor
{
	public class PropertyViewModelTest : PropertyChangedBase
	{
		[Fact]
		public void GeneralPropertyViewModelTest()
		{
			PropertyViewModel pvm = new IntPropertyViewModel();
			DelayNode node = new DelayNode();
			node.Delay = 3;
			PropertyDescriptor pd = TypeDescriptor.GetProperties(node).Find("Delay", true);
			pvm.Initialize(node, pd);
			Assert.Equal("Delay", pvm.DisplayName);
			pvm.Value = 1;
			Assert.Equal(1, pvm.Value);
		}

		[Fact]
		public void ObservableCollectionOfDoublePropertyViewModelTest() 
		{
			WeightedAveragedMergeNode wamn = new WeightedAveragedMergeNode();
			PropertyDescriptor pd = TypeDescriptor.GetProperties(wamn).Find("weights", true);
			ObservableCollectionOfDoublePropertyViewModel ocodpvm = new ObservableCollectionOfDoublePropertyViewModel();
			ocodpvm.Initialize(wamn, pd);

			YuvKA.Pipeline.Node.Input input = new YuvKA.Pipeline.Node.Input();
			wamn.Inputs.Add(input);
			Assert.Equal(wamn.Inputs.Count(), ocodpvm.Values.Count());
			wamn.Inputs.Add(input);
			Assert.Equal(2, ocodpvm.Values.Count());
			wamn.Weights[1] = 10;
			Assert.Equal(wamn.Weights[1], ocodpvm.Values.ElementAt(1).Value);
			ocodpvm.Values.ElementAt(1).Value = 1337;
			Assert.Equal(1337, wamn.Weights[1]);
		}
	}
}
