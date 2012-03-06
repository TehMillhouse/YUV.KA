using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
		private IEventAggregator IoCAggregator(System.Type type, string str)
		{
			return new EventAggregator();
		}

		/// <summary>
		/// Test for the binding and validity of the EnumerationPropertyViewModel
		/// </summary>
		[Fact]
		public void EnumerationPropertyViewModelTest()
		{
			// Since this property viewmodel commits its change as soon as the change is made, the commitChange method
			// is executed, which requires a working IoC
			IoC.GetInstance = IoCAggregator;

			PropertyViewModel en = new EnumerationPropertyViewModel();
			BlurNode blur = new BlurNode();
			blur.Type = BlurType.Linear;
			PropertyDescriptor pd = TypeDescriptor.GetProperties(blur).Find("Type", true);
			Assert.NotNull(pd);
			en.Initialize(blur, pd);

			// Test if binding to property was successful
			Assert.Equal("Type", en.DisplayName);
			Assert.Equal(((EnumerationPropertyViewModel)en).Choices, new[] { BlurType.Gaussian, BlurType.Linear });

			en.Value = BlurType.Gaussian;
			Assert.Equal(BlurType.Gaussian, blur.Type);
			en.Value = BlurType.Linear;
			Assert.Equal(BlurType.Linear, blur.Type);
		}

		/// <summary>
		/// Test for the binding and validity of the FilePathPropertyViewModel
		/// </summary>
		[Fact]
		public void FilePathPropertyViewModelTest()
		{
			// Since this property viewmodel commits its change as soon as the change is made, the commitChange method
			// is executed, which requires a working IoC
			IoC.GetInstance = IoCAggregator;

			FilePathPropertyViewModel en = new FilePathPropertyViewModel();
			ImageInputNode image = new ImageInputNode();
			image.FileName = new YuvKA.Pipeline.FilePath(null);
			PropertyDescriptor pd = TypeDescriptor.GetProperties(image).Find("filename", true);
			en.Initialize(image, pd);

			Assert.Equal("Choose File...", en.ShortPath);
			image.FileName = new YuvKA.Pipeline.FilePath("lol");
			Assert.Equal("lol", en.ShortPath);
			en.Value = new YuvKA.Pipeline.FilePath("rofl");
			Assert.Equal("rofl", image.FileName.Path);
		}

		/// <summary>
		/// Test for the binding and validity of the NullableDoublePropertyViewModel
		/// </summary>
		[Fact]
		public void NullableDoublePropertyViewModelTest()
		{
			// Since this property viewmodel commits its change as soon as the change is made, the commitChange method
			// is executed, which requires a working IoC
			IoC.GetInstance = IoCAggregator;

			NullableDoublePropertyViewModel en = new NullableDoublePropertyViewModel();
			BrightnessContrastSaturationNode bcs = new BrightnessContrastSaturationNode();
			PropertyDescriptor pd = TypeDescriptor.GetProperties(bcs).Find("contrast", true);
			en.Initialize(bcs, pd);

			Assert.True(en.SlidersAreEnabled);
			Assert.Equal(pd.Attributes.OfType<RangeAttribute>().First().Maximum, en.Maximum);
			Assert.Equal(pd.Attributes.OfType<RangeAttribute>().First().Minimum, en.Minimum);
		}

		 ///<summary>
		 ///Test for the binding and validity of the RgbPropertyViewModel
		 ///</summary>
		[Fact]
		public void RgbPropertyViewModelTest()
		{
		    // Since this property viewmodel commits its change as soon as the change is made, the commitChange method
		    // is executed, which requires a working IoC
		    IoC.GetInstance = IoCAggregator;

		    ColorPropertyViewModel en = new ColorPropertyViewModel();
		    ColorInputNode clr = new ColorInputNode();
		    PropertyDescriptor pd = TypeDescriptor.GetProperties(clr).Find("color", true);
		    en.Initialize(clr, pd);

			clr.Color = System.Windows.Media.Color.FromRgb(1, 33, 7);
		    Assert.Equal(new YuvKA.VideoModel.Rgb(1, 33, 7), new YuvKA.VideoModel.Rgb(en.ChosenColor.R, en.ChosenColor.G, en.ChosenColor.B));
		    en.ChosenColor = System.Windows.Media.Color.FromRgb(42, 24, 22);
			Assert.Equal(System.Windows.Media.Color.FromRgb(42, 24, 22), clr.Color);
		}

		/// <summary>
		/// Test for the binding and validity of the SizePropertyViewModel
		/// </summary>
		[Fact]
		public void SizePropertyViewModelTest()
		{
			// Since this property viewmodel commits its change as soon as the change is made, the commitChange method
			// is executed, which requires a working IoC
			IoC.GetInstance = IoCAggregator;

			SizePropertyViewModel en = new SizePropertyViewModel();
			ColorInputNode clr = new ColorInputNode();
			PropertyDescriptor pd = TypeDescriptor.GetProperties(clr).Find("Size", true);
			en.Initialize(clr, pd);

			en.Height = 37;
			en.Width = 13;
			Assert.Equal(new YuvKA.VideoModel.Size(13, 37).Height, clr.Size.Height);
			Assert.Equal(new YuvKA.VideoModel.Size(13, 37).Width, clr.Size.Width);
		}

		/// <summary>
		/// Asserts that the general functions of the property viewmodels work as intended
		/// </summary>
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

		/// <summary>
		/// Test for the binding and validity of the ObservableCollectionOfDoublePropertyViewModel
		/// and its internal wrapper class around double
		/// </summary>
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
