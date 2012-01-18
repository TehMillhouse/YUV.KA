using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.Pipeline.Implementation;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel.Implementation
{
	public class OverlayViewModel : OutputWindowViewModel
	{
		public OverlayViewModel(Node nodeModel) : base(nodeModel)
		{
		}

		public new OverlayNode NodeModel
		{
			get
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
