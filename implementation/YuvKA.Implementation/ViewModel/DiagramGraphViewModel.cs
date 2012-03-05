using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using YuvKA.Pipeline.Implementation;
using YuvKA.ViewModel.Implementation;

namespace YuvKA.Implementation
{
	/// <summary>
	/// Represents the the object usd to modify a graph's color
	/// and type in the diagram output window.
	/// </summary>
	public class DiagramGraphViewModel : PropertyChangedBase
	{
		public DiagramGraphViewModel(DiagramGraph model, DiagramViewModel parent)
		{
			Model = model;
			Parent = parent;

			if (model.Type != null)
				LineColor = Parent.NewColor(Model.Type);
		}

		public DiagramViewModel Parent { get; private set; }

		public DiagramGraph Model { get; private set; }

		public string Name { get { return Parent.GetVideoName(Model.Video); } }

		/// <summary>
		/// Gets the types available with the current configuration
		/// </summary>
		public IEnumerable<GraphTypeViewModel> AvailableTypes
		{
			get
			{
				return Parent.Types.Where(t => (Parent.Reference != null || !t.Model.DependsOnReference)
					&& (Parent.Reference != null && Parent.Reference.Item2.Source.Node.OutputHasLogfile || !t.Model.DependsOnAnnotatedReference)
					&& (Model.Video.Source.Node.OutputHasLogfile || !t.Model.DependsOnLogfile)
					&& (Parent.Reference != null && Parent.Reference.Item2.Source.Node.OutputHasLogfile || !t.Model.DependsOnAnnotatedReference)
				);
			}
		}

		/// <summary>
		/// Gets or sets the type currently chosen by the user.
		/// </summary>
		public GraphTypeViewModel CurrentType
		{
			get { return Model.Type == null ? null : Parent.Types.Single(t => t.Model.GetType() == Model.Type.GetType()); }
			set
			{
				if (value != null) {
					Model.Type = value.Model;
					LineColor = Parent.NewColor(Model.Type);
				}
				else {
					Model.Type = null;
					LineColor = Colors.Transparent;
				}

				NotifyOfPropertyChange(() => LineColor);
				NotifyOfPropertyChange(() => GraphColor);
				Parent.NotifyOfPropertyChange(() => Parent.Lines);
			}
		}

		public LineGraph Line
		{
			get
			{
				var data = new EnumerableDataSource<KeyValuePair<int, double>>(Model.Data);
				data.SetXYMapping(kv => new System.Windows.Point(kv.Key, kv.Value));
				return new LineGraph(data) { LinePen = new Pen(new SolidColorBrush(LineColor), thickness: 1) };
			}
		}

		/// <summary>
		/// Gets the current color for the line.
		/// </summary>
		public Color LineColor { get; private set; }

		/// <summary>
		/// Gets the current color for the preview of the line.
		/// </summary>
		public SolidColorBrush GraphColor { get { return new SolidColorBrush(LineColor); } }
	}
}