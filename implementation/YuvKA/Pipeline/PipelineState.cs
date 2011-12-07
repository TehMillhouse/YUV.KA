using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace YuvKA.Pipeline
{
	public class PipelineState
	{
		public int FrameIndex
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

        public PipelineState(IEventAggregator events) { throw new NotImplementedException(); }

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
