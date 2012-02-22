namespace YuvKA.ViewModel
{
	/// <summary>
	/// Indicates the validity status of an edge being dragged.
	/// </summary>
	public enum EdgeStatus
	{
		/// <summary>
		/// Edge not yet connected to two nodes
		/// </summary>
		Indeterminate,

		/// <summary>
		/// Connection not valid, edge will be removed when dropped
		/// </summary>
		Invalid,

		/// <summary>
		/// Connection valid, edge will be accepted when dropped
		/// </summary>
		Valid
	}
}