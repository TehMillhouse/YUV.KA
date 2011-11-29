using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class MainViewModel
	{
		public ReplayStateViewModel ReplayStateViewModel { get; private set; }
		public PipelineViewModel PipelineViewModel { get; private set; }
		public ToolboxViewModel ToolboxViewModel { get; private set; }
	}
}
