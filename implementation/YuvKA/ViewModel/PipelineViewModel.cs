using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace YuvKA
{
	public class PipelineViewModel
	{
		public ObservableCollection<NodeViewModel> Nodes { get; private set; }
		public ObservableCollection<EdgeViewModel> Edges { get; private set; }
	}
}
