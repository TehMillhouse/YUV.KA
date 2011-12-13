using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[Description("Averages its inputs according to the given weight distribution")]
    [DataContract]
	public class AveragedMergeNode : Node
	{
        [DataMember]
		[Range(0.0, 1.0)]
		[Description("Weights of inputs relative to each other")]
		public ObservableCollection<double> Weights { get { throw new NotImplementedException(); } }

		public override Frame[] ProcessFrame(Frame[] inputs, int tick)
		{
			throw new NotImplementedException();
		}
	}
}
