using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CombineCSV
{
	class Program
	{
		public enum CsvCode
		{
			Gap,
			Glass,
			Vehicle
		}

		static void Main(string[] args)
		{
			string logPath = "D:/CombineCSV/VALUES/";
			//string logPath = "/home/cody/workspace/VALUES/"; For Linux file system

			string dest = string.Empty;
			List<string> csvPaths = null;
			bool passFlag = false;
			bool checker = true;

			if (Directory.Exists(logPath))
			{
				Console.WriteLine("Please enter the date in 'yyyyMMdd' type.");
				Console.Write("Start Date :");
				string startInput = Console.ReadLine();
				Console.Write("End Date :");
				string endInput = Console.ReadLine();

				checker = DateTime.TryParseExact(startInput, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime startDT);
				checker = DateTime.TryParseExact(endInput, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime endDT);
				if (checker == false)
				{
					Console.WriteLine("DateTime Parse failed.");
					Environment.Exit(1);
				}

				Console.WriteLine("\n Which values do you want to combine?");
				do
				{
					Console.Write("1. Gap // 2. Glass // 3. Vehicle (Enter a number.)\n>");
					int input = Convert.ToInt32(Console.ReadLine());
					input = input - 1;
					var myType = (CsvCode)input;

					switch (myType)
					{
						case CsvCode.Gap:
							Console.WriteLine("Combining 1.Gap CSV files...");
							Console.WriteLine("1. CN7");
							csvPaths = SearchCSV(logPath, startDT, endDT, "CN7", CsvCode.Gap);

							Console.Write("Directory to store results.\n>");
							dest = Console.ReadLine();
							checker = CombineCSVFiles(csvPaths, dest);

							passFlag = true;
							break;

						case CsvCode.Glass:
							Console.WriteLine("Combining 2.Glass CSV files...");
							Console.WriteLine("1. CN7");
							csvPaths = SearchCSV(logPath, startDT, endDT, "CN7", CsvCode.Glass);

							Console.Write("Directory to store results.\n>");
							dest = Console.ReadLine();
							checker = CombineCSVFiles(csvPaths, dest);
							passFlag = true;
							break;

						case CsvCode.Vehicle:
							Console.WriteLine("Combining 3.Vehicle CSV files...");
							Console.WriteLine("1. CN7");
							csvPaths = SearchCSV(logPath, startDT, endDT, "CN7", CsvCode.Vehicle);

							Console.Write("Directory to store results.\n>");
							dest = Console.ReadLine();
							checker = CombineCSVFiles(csvPaths, dest);
							passFlag = true;
							break;
					}
				} while (passFlag == false);
			}

			Console.WriteLine("Press any key to EXIT.");
			Console.ReadKey();
		}


		static List<string> SearchCSV(string logPath, DateTime startDT, DateTime endDT, string carCode, CsvCode csv)
		{
			TimeSpan timeCheck = startDT - endDT;
			TimeSpan zero = TimeSpan.Zero;
			if (timeCheck.CompareTo(zero) > 0)
			{
				DateTime tmp = startDT;
				startDT = endDT;
				endDT = tmp;
			}

			var di = new DirectoryInfo(logPath);
			var datePaths = new List<string>();

			var directoryNames = di.EnumerateDirectories()
					.OrderBy(d => d.Name)
					.Select(d => d.Name)
					.ToList();

			int startIndex = directoryNames.BinarySearch(startDT.ToString("yyyyMMdd"));
			if (startIndex < 0)
			{
				startIndex = ~startIndex;   // 만약 해당 리스트에 값이 없다면, 음수로 표현 되기에 ~ 을 통해 양수로 변환.
			}

			int endIndex = directoryNames.BinarySearch(endDT.ToString("yyyyMMdd"));
			if (endIndex < 0)
			{
				endIndex = ~endIndex;
			}
			else
			{
				endIndex += 1;
			}

			for (int k = 0; startIndex < endIndex; startIndex++, k++)
			{
				datePaths.Add(directoryNames[startIndex]);
			}

			string carPath = '/' + carCode;
			string csvPath = string.Empty;

			switch (csv)
			{
				case CsvCode.Gap:
					csvPath = "/gap_values.csv";
					break;
				case CsvCode.Glass:
					csvPath = "/glass_shift_values.csv";
					break;
				case CsvCode.Vehicle:
					csvPath = "/vehicle_shift_values.csv";
					break;
			}

			var resultPaths = new List<string>();
			for (int j = 0; j < datePaths.Count; j++)
			{
				resultPaths.Add(logPath + datePaths[j] + carPath + csvPath);
			}

			return resultPaths;
		}


		static bool CombineCSVFiles(List<string> csvPaths, string resultPath)
		{
			//string[] lines = new string[] { };
			IEnumerable<string> allLines = Enumerable.Empty<string>();
			for (int i = 0; i < csvPaths.Count; i++)
			{
				string file = csvPaths[i];

				if (i == 0)
				{
					allLines = File.ReadAllLines(file);
				}
				else
				{
					allLines = allLines.Concat(File.ReadAllLines(file).Skip(1));
				}
			}
				File.WriteAllLines(resultPath, allLines);

			return true;
		}
	}
}






//if (file == null)
//// 예외처리 구문 수정 -> NULL 이 아니라 다양한 상황 가정해서
//{
//	Console.WriteLine("Error, CSV file is missing.");
//	return false;
//}