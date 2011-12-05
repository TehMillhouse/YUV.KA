using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.ViewModel.PropertyEditor
{
	public class PropertyEditorViewModel
	{
		public object Source { get; set; }
		public IEnumerable<PropertyViewModel> Properties { get { throw new NotImplementedException(); } }
	}
}
