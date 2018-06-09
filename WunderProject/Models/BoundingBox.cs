using RBush;
using System;
namespace RTreesProject.Models
{
	/// <summary>
	/// The bounding box class, extends spatial data used by RBush library to use R-Trees
	/// </summary>
	class BoundingBox : ISpatialData
	{
		private static int idCounter;
		public BoundingBox(double x1, double x2, double y1, double y2)
		{
			this.Id = ++idCounter;
			// Envelope is a container, it can be a box or a single point
			this.Envelope = new Envelope(
				Math.Min(x1, x2),
				Math.Min(y1, y2),
				Math.Max(x1, x2),
				Math.Max(y1, y2));
		}

		public Envelope Envelope { get; set; }
		public int Id { get; set; }
	}
}
