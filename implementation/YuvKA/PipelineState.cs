using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class PipelineState
	{
		public PipelineGraph Graph { get; private set; }
		public ReplayState ReplayState { get; private set; }
	}
}
