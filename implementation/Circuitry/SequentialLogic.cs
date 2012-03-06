using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Circuitry
{
	[DataContract]
	public class SaveStateNode : DigitalOutputNode
	{
		public static Dictionary<Tuple<string, int>, bool> States = new Dictionary<Tuple<string, int>, bool>();

		public SaveStateNode()
			: base(1)
		{
			Name = "Save State";
		}

		[DataMember]
		[Browsable(true)]
		[Description]
		public string StateName { get; set; }

		public override bool ProcessNodeInBackground { get { return true; } }

		public override void ProcessDigital(bool[] inputs, int tick)
		{
			States[Tuple.Create(StateName, tick)] = inputs[0];
		}
	}

	public class LoadStateNode : DigitalInputNode
	{
		public LoadStateNode()
			: base(1)
		{
			Name = "Load State";
		}

		[DataMember]
		[Browsable(true)]
		[Description]
		public string StateName { get; set; }

		public override bool OutputDigital(int tick)
		{
			return SaveStateNode.States.ContainsKey(Tuple.Create(StateName, tick - 1)) ? SaveStateNode.States[Tuple.Create(StateName, tick - 1)] : false;
		}
	}
}
