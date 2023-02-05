using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.Yapp
{
	/// <summary>
	/// Create a list of poisson disc samples from the sampler and re-use it for drawng the gizmo.
	/// Create a new list upon request which is usually after the current samples list has been used for painting and we need a new random one.
	/// </summary>
    public class PoissonDiscSampleProvider
    {
		public const float MIN_DISC_SIZE = 0.1f;

		/// <summary>
		/// Singleton
		/// </summary>
        private static PoissonDiscSampleProvider _instance;

		public static PoissonDiscSampleProvider Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PoissonDiscSampleProvider();
				}

				return _instance;
			}
		}

		private PoissonDiscSampler sampler;

		private IEnumerable<Vector2> samplesList;
		private SampleParameters sampleParameters;

		/// <summary>
		/// Get the list of samples.
		/// The samples are re-used unless forceNew is true. This is usually after the current sample set has been used for painting.
		/// Otherwise the sample set will be used for drawing the gizmo
		/// </summary>
		/// <param name="brushSize"></param>
		/// <param name="brushRadius"></param>
		/// <param name="discRadius"></param>
		/// <param name="forceNew"></param>
		/// <returns></returns>
		public IEnumerable<Vector2> Samples( float brushSize, float brushRadius, float discRadius, bool forceNewDistribution)
        {
			if (discRadius < MIN_DISC_SIZE)
			{
				Debug.LogError("Poisson Disc Size is too small. Setting it to " + MIN_DISC_SIZE);
				discRadius = MIN_DISC_SIZE;
			}

			bool createNewList = forceNewDistribution || sampler == null || sampleParameters.HasChanged(brushSize, brushRadius, discRadius);

			if ( createNewList)
            {
				sampler = new PoissonDiscSampler(brushSize, brushSize, discRadius);
				samplesList = CreateSamplesList(brushSize, brushSize, discRadius);
			}

			// store parameters for reuse of the list
			sampleParameters = new SampleParameters(brushSize, brushRadius, discRadius);
			 
			return samplesList;
		}

		/// <summary>
		/// Create a list of samples from the sampler.
		/// The sampler uses a lazy list, we need a fixed one.
		/// In addition we consider only a brush radius, not a rectangle like the poisson disc sampler does.
		/// All samples outside the radius will be removed
		/// </summary>
		/// <param name="brushSize"></param>
		/// <param name="brushRadius"></param>
		/// <param name="discRadius"></param>
		/// <returns></returns>
		private List<Vector2> CreateSamplesList(float brushSize, float brushRadius, float discRadius)
        {
			List<Vector2> list = new List<Vector2>();
			foreach (Vector2 sample in sampler.Samples())
			{
				// ensure the samples are within the brush

				// the samples are within a rectangle of [0,0,brushRadius,brushRadius]
				// however we deal with a cirular brush
				// => shift the center of the brush from [0,0] to the center of the rectangle, ie [brushRadius * 0.5f, brushRadius * 0.5f]
				Vector2 brushCenterPos = new Vector2(brushRadius * 0.5f, brushRadius * 0.5f);

				// now consider the samples as positioned from the brush center instead of [0,0]
				// calculate the distance and ensure it's within brush radius from the center, which is half the actual radius
				// skip the sample in case it's outside the circular brush
				float distance = Vector2.Distance(sample, brushCenterPos);
				if (distance > brushRadius * 0.5f)
				{
					continue;
				}

				list.Add(sample);
			}

			return list;
		}

		/// <summary>
		/// Parameters of the current samples.
		/// This is used to decide if we need to create a new sample list or if we should keep the currently created one.
		/// </summary>
		private struct SampleParameters
		{
			float brushSize;
			float brushRadius;
			float discRadius;

			public SampleParameters(float brushSize, float brushRadius, float discRadius)
			{
				this.brushSize = brushSize;
				this.brushRadius = brushRadius;
				this.discRadius = discRadius;
			}

			public bool HasChanged(float brushSize, float brushRadius, float discRadius)
            {
				return this.brushSize != brushSize || this.brushRadius != brushRadius || this.discRadius != discRadius;
			}

		}
	}
}
