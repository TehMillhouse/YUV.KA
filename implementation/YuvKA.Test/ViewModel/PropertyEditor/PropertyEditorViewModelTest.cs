using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using Caliburn.Micro;
using Xunit;
using YuvKA;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.VideoModel;
using YuvKA.ViewModel;
using YuvKA.ViewModel.PropertyEditor;
using YuvKA.ViewModel.PropertyEditor.Implementation;
using System.Linq;

namespace YuvKA.Test.ViewModel.PropertyEditor
{
	public class PropertyEditorViewModelTest
	{
		private IEnumerable<object> derp(System.Type nope) {
			return new List<object>{ new IntPropertyViewModel() };
		}

		/// <summary>
		/// Tests if the pevm assigns the correct propertyviewmodels.
		/// </summary>
		[Fact]
		public void TestPEVM() {
			PropertyEditorViewModel pevm = new PropertyEditorViewModel();
			DelayNode node = new DelayNode { Delay = 0 };
			IoC.GetAllInstances = derp;
			pevm.Source = node;
			Assert.Equal(node, pevm.Source);
			Assert.Equal(true, pevm.Properties.Single() is IntPropertyViewModel);
			Assert.Equal(1, pevm.Properties.Count());
		}
	}
}
