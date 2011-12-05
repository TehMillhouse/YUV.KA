using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace YuvKA.ViewModel.PropertyEditor
{
	public abstract class PropertyViewModel
	{
		public object Source { get; private set; }
		public PropertyDescriptor Property { get; private set; }
	}
}
