using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace YuvKA.Pipeline.Implementation
{
	public class DiagramNode : OutputNode
	{
		[DisplayName("Enabled")]
		public bool IsEnabled { get; set; }

		[Browsable(false)]
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

		[Browsable(false)]
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

		#region INode Members


		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
