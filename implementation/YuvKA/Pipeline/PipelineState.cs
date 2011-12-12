using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Runtime.Serialization;
using System.ComponentModel.Composition;

namespace YuvKA.Pipeline
{
	[DataContract]
	public class PipelineState
	{
		[DataMember]
		public int CurrentTick
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		/// <summary>
		/// Replay speed in frames per second
		/// </summary>
		[DataMember]
		public int Speed
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[DataMember]
		public PipelineGraph Graph
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		[Import(typeof(IEventAggregator))]
		public IEventAggregator Events { get; set; }

		public void Start()
		{
			throw new System.NotImplementedException();
		}

		public void Stop()
		{
			throw new System.NotImplementedException();
		}
	}
}
