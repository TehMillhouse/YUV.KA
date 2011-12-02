using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuvKA.VideoModel
{
	public static class YuvEncoder
	{
		public class Video : IDisposable
		{
			Stream stream;

			public Frame this[int index]
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public int FrameCount
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public static Video Decode(string fileName, string logFileName, int width, int height)
		{
			throw new System.NotImplementedException();
		}

		public static void Encode(string fileName, IEnumerable<Frame> frames)
		{
			throw new System.NotImplementedException();
		}
	}
}
