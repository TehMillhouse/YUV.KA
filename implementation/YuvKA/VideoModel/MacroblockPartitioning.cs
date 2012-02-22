namespace YuvKA.VideoModel
{
	/// <summary>
	/// An enumeration specifying the different kinds of decisions an encoder can take for breaking up a macroblock into smaller blocks
	/// </summary>
	public enum MacroblockPartitioning
	{
		InterSkip,
		Inter16x16,
		Inter16x8,
		Inter8x16,
		Inter8x8,
		Inter8x4,
		Inter4x8,
		Inter4x4,
		Inter8x8OrBelow,
		Intra4x4,
		Intra16x16,
		Intra8x8,
		IntraPCM,
		Unknown
	}
}
