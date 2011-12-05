using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace YuvKA.ViewModel.PropertyEditor.Implementation
{
	[Export(typeof(PropertyViewModel))]
	[ExportMetadata("PropertyType", typeof(bool))]
	public sealed class BooleanPropertyViewModel : PropertyViewModel
	{
	}
}
