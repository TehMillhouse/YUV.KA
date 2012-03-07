using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
	[Description("This Node averages its inputs according to the given weight distribution")]
	[DataContract]
	public class WeightedAveragedMergeNode : Node
	{
		/// <summary>
		///	Constructs a new WeightedAverageMergeNode.
		/// </summary>
		public WeightedAveragedMergeNode()
			: base(inputCount: null, outputCount: 1)
		{
			Name = "Weighted Merge";
			Weights = new ObservableCollection<double>();

			((ObservableCollection<Input>)Inputs).CollectionChanged += (_, e) => {
				if (e.Action == NotifyCollectionChangedAction.Add)
					Weights.Add(1.0);
				else if (e.Action == NotifyCollectionChangedAction.Remove)
					Weights.RemoveAt(e.OldStartingIndex);
			};
		}

		/// <summary>
		/// Stores the weights for each input.
		/// Missing weights will be completed with weights of the default value 1. All values range from 0 to 1.
		/// </summary>
		[DataMember]
		[Range(0.0, 1.0)]
		[Description("Weights of inputs relative to each other")]
		[Browsable(true)]
		public ObservableCollection<double> Weights { get; private set; }

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
	}
}
