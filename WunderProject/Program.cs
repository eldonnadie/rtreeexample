// external library to use effectively R-Trees in c# - https://github.com/viceroypenguin/RBush
using RBush;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RTreesProject.Models;

namespace RTreesProject
{
	class Program
	{
		static RTreesProjectTestService rtreesTestService = null;
		static Dictionary<int, BoundingBox> collidingBoundingBoxes = new Dictionary<int, BoundingBox>();

		static void Main(string[] args)
		{			
			try
			{
				string pairsFileLocation = ConfigurationManager.AppSettings["csvPairsFileLocation"];
				var pairsLines = File.ReadAllLines(pairsFileLocation);			
				var useful = pairsLines.ToList().GetRange(1, pairsLines.Length - 1);
				var test = new List<string>();
				test.AddRange(pairsLines);

				for (int i = 0; i < 1000; i++)
				{
					test.AddRange(useful);
				}
				
				rtreesTestService = new RTreesProjectTestService(ReadBoxes(test.ToArray()));


				string pointsFileLocation = ConfigurationManager.AppSettings["csvPointsFileLocation"];
				var pointsLines = File.ReadAllLines(pairsFileLocation);
				var points = ReadPoints(pointsLines);

				foreach (var point in points)
				{
					var boxes = rtreesTestService.BoxesThatMatch(point);
					foreach (var box in boxes)
					{
						if (!collidingBoundingBoxes.ContainsKey(box.Id))
						{
							collidingBoundingBoxes.Add(box.Id, box);
						}
					}
				}

				Console.ReadKey();
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine("File not found: " + ex.Message);
			}
			catch (DirectoryNotFoundException ex)
			{
				Console.WriteLine("Directory not found: " + ex.Message);
			}
			catch (Exception ex)
			{
				Console.WriteLine("General exception: " + ex.Message);
			}

			Console.ReadKey();
		}

		private static List<Point> ReadPoints(string[] pointsLines)
		{
			var result = new List<Point>();

			// i starts at 2 to ignore the columns headers
			for (int i = 2; i < pointsLines.Length; i++)
			{
				var coordinatesPointString = pointsLines[i].Split(',');
				var point = new Point(float.Parse(coordinatesPointString[0]), float.Parse(coordinatesPointString[1]));
				result.Add(point);
			}

			return result;
		}

		private static List<BoundingBox> ReadBoxes(string[] pairsLines)
		{
			var boxes = new List<BoundingBox>();
			// i starts at 2 to ignore the columns headers
			for (int i = 2; i < pairsLines.Length; i += 2)
			{
				var coordinatesPreviousPointString = pairsLines[i - 1].Split(',');
				var coordinatesPointString = pairsLines[i].Split(',');

				boxes.Add(new BoundingBox(
					double.Parse(coordinatesPreviousPointString[0]),
					double.Parse(coordinatesPointString[0]),
					double.Parse(coordinatesPreviousPointString[1]),
					double.Parse(coordinatesPointString[1])));
			}

			return boxes;
		}
	}


}
