using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using YuvKA.Pipeline.Implementation;
using YuvKA.Pipeline;

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

		public void AddGraph(Node.Input video)
		{
			throw new System.NotImplementedException();
		}
	}
}
