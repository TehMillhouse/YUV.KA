using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
    public class OverlayNode : Node
    {
        public Frame OverlayedOutput
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public IOverlayType Type
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public override void ProcessFrame(int frameIndex)
        {
            throw new NotImplementedException();
        }
    }
}
