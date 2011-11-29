using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class DiagramNode : Node
	{
		public int? ReferenceVideo
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public List<DiagramGraph> Graphs
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[Import(ReplayState)]
		public ReplayState ReplayState { get; set; }

		#region INode Members


		public override void ProcessFrame()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
