using System;
using System.Windows.Media;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay.DataSources;


namespace YuvKA.Implementation
{
	public class LineGraphViewModel : PropertyChangedBase
	{
		/// <summary>
		/// Gets or sets the point data source.
		/// </summary>
		/// <value>The point data source.</value>
		public CompositeDataSource PointDataSource
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name of the line graph.</value>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public Color Color
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the entity id.
		/// </summary>
		/// <value>The entity id.</value>
		public Guid EntityId
		{
			get;
			set;
		}

		public bool LineAndMarker
		{
			get;
			set;
		}

		public int Thickness
		{
			get;
			set;
		}
	}
}
