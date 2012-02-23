using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Implementation
{
	/// <summary>
	/// Represents the the object usd to modify a graph's color
	/// and type in the diagram output window.
	/// </summary>
	public class GraphControl : PropertyChangedBase
	{
		private Tuple<string, IGraphType> chosenType;
		private System.Drawing.Color thisLineColor;
		private bool referenceSet;
		private bool referenceHasLogfile;

		/// <summary>
		/// Gets or sets the DiagramViewModel to which this GraphControl belongs
		/// </summary>
		public DiagramViewModel Parent { get; set; }

		/// <summary>
		/// Gets or sets the video of this GraphControl
		/// </summary>
		public Tuple<string, Node.Input> Video { get; set; }

		/// <summary>
		/// Gets or sets the graph of this GraphControl
		/// </summary>
		public DiagramGraph Graph { get; set; }

		/// <summary>
		/// Gets the all available IGraphTypes.
		/// </summary>
		public ObservableCollection<Tuple<string, IGraphType>> Types { get; set; }

		/// <summary>
		/// Gets or sets the types available with the current configuration
		/// </summary>
		public ObservableCollection<Tuple<string, IGraphType>> DisplayTypes { get; set; }

		/// <summary>
		/// Gets or sets the type currently chosen by the user.
		/// </summary>
		public Tuple<string, IGraphType> ChosenType
		{
			get { return chosenType; }
			set
			{
				if (chosenType == null) {
					chosenType = value;
					Graph.Type = value.Item2;
					SetLineColor();
					Parent.LineColors.Add(CurrentLineColor);
					Parent.AddGraph(this);
				} else {
					chosenType = value;
					Graph.Type = value.Item2;
					Parent.LineColors.RemoveAll(color => color.R == LineColor.R && color.G == LineColor.G && color.B == LineColor.B);
					SetLineColor();
					Parent.LineColors.Add(CurrentLineColor);
				}
			}
		}

		/// <summary>
		/// Gets or sets the current color for the line using the System.Drawing.Color object.
		/// </summary>
		public System.Drawing.Color CurrentLineColor { get; set; }

		/// <summary>
		/// Gets or sets the current color for the line using the System.Windows.Media.Color object.
		/// </summary>
		public Color LineColor { get; set; }

		/// <summary>
		/// Determines, wether a referencevideo was chosen.
		/// </summary>
		public bool ReferenceSet
		{
			get { return referenceSet; }
			set
			{
				referenceSet = value;
				SetDisplayTypes();
			}
		}

		/// <summary>
		/// Determines, wether the referencevideo has a logfile
		/// </summary>
		public bool ReferenceHasLogfile
		{
			get { return referenceHasLogfile; }
			set
			{
				referenceHasLogfile = value;
				SetDisplayTypes();
			}
		}

		/// <summary>
		/// Gets or sets the current color for the preview of the line.
		/// </summary>
		public SolidColorBrush GraphColor { get; set; }

		/// <summary>
		/// Sets the line colors to the various properties.
		/// </summary>
		public void SetLineColor()
		{
			LineColor = NewColor(ChosenType);
			NotifyOfPropertyChange(() => LineColor);
			GraphColor = new SolidColorBrush(LineColor);
			NotifyOfPropertyChange(() => GraphColor);
		}

		/// <summary>
		/// Generates a new color of the same tone as the basecolor of the given type
		/// </summary>
		public Color NewColor(Tuple<string, IGraphType> type)
		{
			System.Drawing.Color newColor;
			// Get basecolor of the type
			var baseColor = System.Drawing.Color.FromArgb(Parent.TypeColors[Types.IndexOf(type)].A, Parent.TypeColors[Types.IndexOf(type)].R, Parent.TypeColors[Types.IndexOf(type)].G, Parent.TypeColors[Types.IndexOf(type)].B);

			var h = baseColor.GetHue();

			var random = new Random();
			do {
				// Generate new color by using the hue of the basecolor and getting a random brightness and saturation.
				newColor = HslHelper.HslToRgb(h, random.NextDouble(), random.NextDouble());
			} while (Parent.LineColors.Contains(newColor) || newColor.GetHue().Equals(0.0) || newColor.GetBrightness().Equals(1.0) || newColor.GetBrightness().Equals(0.0) || Parent.LineColors.FindIndex(color => DiagramViewModel.IsInIntervall(newColor.GetBrightness(), color.GetBrightness(), 0.08) && DiagramViewModel.IsInIntervall(newColor.GetSaturation(), color.GetSaturation(), 0.08)) != -1);
			CurrentLineColor = newColor;

			return Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B);
		}

		/// <summary>
		/// Determines, which Types can be used by the current configuration.
		/// </summary>
		public void SetDisplayTypes()
		{
			var newTypes = new List<Tuple<string, IGraphType>>(Types);
			if (!ReferenceSet) {
				newTypes.RemoveAll(type => type.Item2.DependsOnReference);
			}
			if (!Video.Item2.Source.Node.OutputHasLogfile) {
				newTypes.RemoveAll(type => type.Item2.DependsOnLogfile);
			}
			if (!ReferenceHasLogfile) {
				newTypes.RemoveAll(type => type.Item2.DependsOnAnnotatedReference);
			}
			DisplayTypes = new ObservableCollection<Tuple<string, IGraphType>>(newTypes);
			NotifyOfPropertyChange(() => DisplayTypes);
		}
	}
}