using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.Pipeline.Implementation
{
    public class BrightnessContrastSaturationNode : YuvKA.Pipeline.Node
    {
        public int BrightnessLevel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int ContrastLevel
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int SaturationLevel
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
