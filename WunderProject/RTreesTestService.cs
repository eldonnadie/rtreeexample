using RBush;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using RTreesProject.Models;

namespace RTreesProject
{
	class RTreesProjectTestService
	{
		/// <summary>
		/// All the bounding boxes
		/// </summary>
		private IList<BoundingBox> BoundingBoxes = null;

		// Separate the full bounding boxes into smaller buckets, determined by configuration, later on.
		private IList<RBush<BoundingBox>> BoxesEnginesBuckets = null;
		private static int MaxConcurrentEngines = 0;

		public RTreesProjectTestService(List<BoundingBox> boxes)
		{
			Initialize(boxes);
		}

		void Initialize(List<BoundingBox> boxes)
		{
			MaxConcurrentEngines = int.Parse(ConfigurationManager.AppSettings["maxConcurrentEngines"]);
			BoundingBoxes = boxes;
			BoxesEnginesBuckets = new List<RBush<BoundingBox>>();
			SplitToBuckets();
		}

		/// <summary>
		/// Method used to split all the bounding boxes into a predefined config based smaller buckets to be able to parallely searched
		/// </summary>
		private void SplitToBuckets()
		{
			int subListSize = BoundingBoxes.Count / MaxConcurrentEngines;
			int elementsIterated = 0;

			while (elementsIterated < BoundingBoxes.Count)
			{
				int remainingElements = Math.Min(BoundingBoxes.Count - elementsIterated, subListSize);
				var newBush = new RBush<BoundingBox>();
				newBush.BulkLoad((BoundingBoxes as List<BoundingBox>).GetRange(elementsIterated, remainingElements));
				BoxesEnginesBuckets.Add(newBush);
				elementsIterated += subListSize;
			}
		}

		/// <summary>
		/// Method used to get an array of boxes that match 1 or 2 points
		/// </summary>
		/// <param name="origin">The origin point to compare</param>
		/// <param name="destination">The destination point to compare</param>
		/// <returns>The bounding boxes that match</returns>
		public BoundingBox[] BoxesThatMatch(Point origin, Point destination = null)
		{
			var result = ArrayList.Synchronized(new ArrayList());

			// I tested this, 8 simultaneous buckets seem to do the work
			// I also tested with 2, 4, 8 & 16. Between 8 and 16 didn't see much difference		
			// I used time stamps, var now = DateTime.Now; comparing just iterating a single list and multiple buckets
			// the multiple buckets approach worked better, I didn't include it to make the code look cleaner
			Parallel.ForEach(BoxesEnginesBuckets, (boxEngineBucket) =>
			{
				if (origin != null)
				{
					result.AddRange(boxEngineBucket.Search(origin.Envelope).ToList());
				}
				if (destination != null)
				{
					result.AddRange(boxEngineBucket.Search(destination.Envelope).ToList());
				}
			});

			return result.ToArray(typeof(BoundingBox)) as BoundingBox[];
		}
	}
}
