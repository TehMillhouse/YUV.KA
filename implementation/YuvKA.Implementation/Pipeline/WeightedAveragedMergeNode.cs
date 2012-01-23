using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.VideoModel;
using System;

namespace YuvKA.Pipeline.Implementation
{
	[Description("Averages its inputs according to the given weight distribution")]
	[DataContract]
	public class WeightedAveragedMergeNode : Node
	{
		private ObservableCollection<double> weights;
		public WeightedAveragedMergeNode()
			: base(inputCount: null, outputCount: 1)
		{
			Name = "Weighted Merge";
			((ObservableCollection<Input>)Inputs).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnInputsChanged);
		}

		public void OnInputsChanged(object sender, EventArgs e)
		{
			NotifyOfPropertyChange(() => Weights);
		}

		[DataMember]
		[Range(0.0, 1.0)]
		[Description("Weights of inputs relative to each other")]
		[Browsable(true)]
		public ObservableCollection<double> Weights
		{
			get
			{
				if (weights == null) {
					weights = new ObservableCollection<double>(Inputs.Select(i => 1.0));
				}
				else if (weights.Count < Inputs.Count) {
					for (int i = 0; i < Inputs.Count - weights.Count; i++) {
						weights.Add(1.0);
					}
				}

				return weights;
			}
			set
			{
				weights = value;
			}
		}

		public override Frame[] Process(Frame[] inputs, int tick)
		{
			Size maxSize = Frame.MaxBoundaries(inputs);
			double sumOfWeights = Weights.Sum();
			Frame[] output = { new Frame(new Size(maxSize.Width, maxSize.Height)) };

			for (int x = 0; x < maxSize.Width; x++) {
				for (int y = 0; y < maxSize.Height; y++) {
					double newR = 0, newG = 0, newB = 0;
					for (int i = 0; i < inputs.Length; i++) {
						newR += Weights[i] * inputs[i].GetPixelOrBlack(x, y).R;
						newG += Weights[i] * inputs[i].GetPixelOrBlack(x, y).G;
						newB += Weights[i] * inputs[i].GetPixelOrBlack(x, y).B;
					}
					newR = newR / sumOfWeights;
					newG = newG / sumOfWeights;
					newB = newB / sumOfWeights;
					output[0][x, y] = new Rgb((byte)newR, (byte)newG, (byte)newB);
				}
			}
			return output;
		}
	}
}
