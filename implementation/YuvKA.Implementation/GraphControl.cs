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
	public class GraphControl : PropertyChangedBase
	{
		private Tuple<string, IGraphType> chosenType;
		private List<System.Drawing.Color> theseLineColors;
		private bool referenceSet;
		private bool referenceHasLogfile;

		public DiagramViewModel Parent { get; set; }

		public Tuple<string, Node.Input> Video { get; set; }

		public DiagramGraph Graph { get; set; }

		public ObservableCollection<Tuple<string, IGraphType>> Types { get; set; }

		public ObservableCollection<Tuple<string, IGraphType>> DisplayTypes { get; set; }

		public Tuple<string, IGraphType> ChosenType
		{
			get { return chosenType; }
			set
			{
				if (chosenType == null) {
					chosenType = value;
					Graph.Type = value.Item2;
					SetLineColor();
					foreach (var lineColor in TheseLineColors) {
						LineColors.Add(lineColor);
					}
					Parent.AddGraph(this);
				}
				else {
					chosenType = value;
					Graph.Type = value.Item2;
					TheseLineColors.RemoveAll(color => color.R == LineColor.R && color.G == LineColor.G && color.B == LineColor.B);
					SetLineColor();
				}
			}
		}

		public List<Color> TypeColors { get; set; }

		public List<System.Drawing.Color> TheseLineColors
		{
			get { return theseLineColors ?? (theseLineColors = new List<System.Drawing.Color>()); }
			set { theseLineColors = value; }
		}

		public List<System.Drawing.Color> LineColors { get; set; }

		public Color LineColor { get; set; }

		public bool ReferenceSet
		{
			get { return referenceSet; }
			set
			{
				referenceSet = value;
				SetDisplayTypes();
			}
		}

		public bool ReferenceHasLogfile
		{
			get { return referenceHasLogfile; }
			set
			{
				referenceHasLogfile = value;
				SetDisplayTypes();
			}
		}

		public SolidColorBrush GraphColor { get; set; }

		public void SetLineColor()
		{
			LineColor = NewColor(ChosenType);
			NotifyOfPropertyChange(() => LineColor);
			GraphColor = new SolidColorBrush(LineColor);
			NotifyOfPropertyChange(() => GraphColor);
		}

		public Color NewColor(Tuple<string, IGraphType> type)
		{
			System.Drawing.Color newColor;
			var typeCollection = new ObservableCollection<Tuple<string, IGraphType>>(Types);
			var baseColor = System.Drawing.Color.FromArgb(TypeColors[typeCollection.IndexOf(type)].A, TypeColors[typeCollection.IndexOf(type)].R, TypeColors[typeCollection.IndexOf(type)].G, TypeColors[typeCollection.IndexOf(type)].B);

			var h = baseColor.GetHue();

			var random = new Random();
			do {
				newColor = HslHelper.HslToRgb(h, random.NextDouble(), random.NextDouble());
			} while (LineColors.Contains(newColor) || newColor.GetHue().Equals(0.0) || newColor.GetBrightness().Equals(1.0) || newColor.GetBrightness().Equals(0.0) || LineColors.FindIndex(color => DiagramViewModel.IsInIntervall(newColor.GetBrightness(), color.GetBrightness(), 0.08) && DiagramViewModel.IsInIntervall(newColor.GetSaturation(), color.GetSaturation(), 0.08)) != -1);
			TheseLineColors.Add(newColor);

			return Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B);
		}

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