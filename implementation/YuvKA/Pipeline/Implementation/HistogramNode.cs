using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
    public class HistogramNode : Node
    {
        public HistogramType Type
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int[] Data { get; private set; }
    
        public override void ProcessFrame()
        {
            throw new NotImplementedException();
        }
    }
}
