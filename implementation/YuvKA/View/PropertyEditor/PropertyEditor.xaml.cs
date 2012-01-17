using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuvKA.ViewModel.PropertyEditor;

namespace YuvKA.View.PropertyEditor
{
	/// <summary>
	/// Interaction logic for PropertyEditor.xaml
	/// </summary>
	public partial class PropertyEditor : UserControl
	{
		// Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(object), typeof(PropertyEditor), new FrameworkPropertyMetadata(OnSourceChanged));

		PropertyEditorViewModel vm;

		public PropertyEditor()
		{
			InitializeComponent();
			ic.DataContext = vm = new PropertyEditorViewModel();
		}

		public object Source
		{
			get { return (object)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var self = ((PropertyEditor)d);
			self.vm.Source = self.Source;
		}
	}
}
