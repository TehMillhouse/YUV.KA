using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class implements the possibility to overlay frames while using weights for each of them. The resulting frame will also be weighted.
	/// </summary>
	[Description("Averages its inputs according to the given weight distribution")]
	[DataContract]
	public class WeightedAveragedMergeNode : Node
	{
		private ObservableCollection<double> weights;
		/// <summary>
		///	Constructs a new WeightedAverageMergeNode.
		/// </summary>
		public WeightedAveragedMergeNode()
			: base(inputCount: null, outputCount: 1)
		{
			Name = "Weighted Merge";
			((ObservableCollection<Input>)Inputs).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnInputsChanged);
		}

		/// <summary>
		/// Stores the weights for each input.
		/// Missing weights will be completed with weights of the default value 1. All values range from 0 to 1.
		/// </summary>
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

		/// <summary>
		/// Adds up the weighted frames and finally averages them according to the following formula:
		/// If there are n frames to be merged and w_1,..., w_n are their weights, the resulting frame will be computed by using this formula:
		/// newPixel_xy = (sum(w_i * oldValue_xy_i))/sum(w_i)
		/// (xy represents the position of the pixel in the frame.)
		/// </summary>
		public override Frame[] Process(Frame[] inputs, int tick)
		{
			Size maxSize = Frame.MaxBoundaries(inputs);
			double sumOfWeights = Weights.Sum();
			Frame[] output = { new Frame(new Size(maxSize.Width, maxSize.Height)) };

			for (int x = 0; x < maxSize.Width; x++) {
				for (int y = 0; y < maxSize.Height; y++) {
					// sums up the weighted values
					double newR = 0, newG = 0, newB = 0;
					for (int i = 0; i < inputs.Length; i++) {
						newR += Weights[i] * inputs[i].GetPixelOrBlack(x, y).R;
						newG += Weights[i] * inputs[i].GetPixelOrBlack(x, y).G;
						newB += Weights[i] * inputs[i].GetPixelOrBlack(x, y).B;
					}
					// averages the values
					newR = newR / sumOfWeights;
					newG = newG / sumOfWeights;
					newB = newB / sumOfWeights;
					output[0][x, y] = new Rgb((byte)newR, (byte)newG, (byte)newB);
				}
			}
			return output;
		}

		/// <summary>
		/// Removes all inputs whose source is null and their weights.
		/// </summary>
		public override void CullInputs()
		{
			foreach (Node.Input input in Inputs.ToArray()) {
				if (input.Source == null) {
					Weights.RemoveAt(Inputs.IndexOf(input));
					Inputs.Remove(input);
				}
			}
		}

		private void OnInputsChanged(object sender, EventArgs e)
		{
			NotifyOfPropertyChange(() => Weights);
		}
	}
}
