using System;
using System.Threading;
using Xunit;
using YuvKA.Pipeline;

namespace YuvKA.Test.Pipeline
{
	public class PipelineDriverTest
	{
		/// <summary>
		/// Driver should not be implemented yet.
		/// </summary>
		[Fact]
		public void NotImplemented()
		{
			Assert.Throws<NotImplementedException>(
				() => new PipelineDriver().RenderTicks(null, 0, new CancellationToken())
			);
		}
	}
}
