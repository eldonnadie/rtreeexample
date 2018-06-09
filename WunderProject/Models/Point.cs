using RBush;

namespace RTreesProject.Models
{
	/// <summary>
	/// The point class, extends spatial data used by RBush library to use R-Trees
	/// </summary>
	public class Point : ISpatialData
	{
		public Point(double lon, double lat)
		{
			// Envelope is a container, it can be a box or a single point
			this.Envelope = new Envelope(lon, lat, lon, lat);
		}

		public Envelope Envelope { get; set; }
	}
}
