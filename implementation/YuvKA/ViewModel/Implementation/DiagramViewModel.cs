using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.ViewModel.Implementation
{
	public class DiagramViewModel : OutputWindowViewModel
	{

		public new DiagramNode NodeModel
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}


		public IList<IGraphType> Types
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}

		public void DeleteGraph(DiagramGraph graph)
		{
			throw new System.NotImplementedException();
		}

		public void AddGraph(Input video)
		{
			throw new System.NotImplementedException();
		}

		public class Input
		{
			public int Index { get; set; }
			public string Name { get { return "Video " + (Index + 1); } }
		}
	}
}
