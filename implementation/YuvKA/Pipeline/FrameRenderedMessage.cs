using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline
{
    public class TickRenderedMessage
    {
        public Frame this[Node.Output output]
        {
            get { throw new NotImplementedException(); }
        }
    }
}
