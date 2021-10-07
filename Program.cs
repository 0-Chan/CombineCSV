using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CombineCSV
{
    class Program
    {
        public enum CsvType
        {
            Gap,
            Glass,
            Vehicle
        }

        static void Main(string[] args)
        {
            string logPath = "D:/CombineCSV/VALUES";

            string dest = string.Empty;
            string[] carCode = new string[] { "CN7", "CN9" };
            List<string> csvPaths = null;
            bool passFlag = false;
            bool checker = true;

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
                var myType = (CsvType)input;

                switch (myType)
                {
                    case CsvType.Gap:
                        Console.WriteLine("Combining 1.Gap CSV files...");
                        Console.WriteLine("{0}", carCode);
                        csvPaths = SearchCsv(logPath, startDT, endDT, carCode, "Gap");

                        Console.Write("Directory to store results.\n>");
                        dest = Console.ReadLine();
                        checker = CombineCsvFiles(csvPaths, dest);
                        if (!checker)
                        {
                            Console.WriteLine("Combine failed.");
                            Environment.Exit(1);
                        }
                        passFlag = true;
                        break;

                    case CsvType.Glass:
                        Console.WriteLine("Combining 2.Glass CSV files...");
                        Console.WriteLine("{0}", carCode);
                        csvPaths = SearchCsv(logPath, startDT, endDT, carCode, "Glass");

                        Console.Write("Directory to store results.\n>");
                        dest = Console.ReadLine();
                        checker = CombineCsvFiles(csvPaths, dest);
                        if (!checker)
                        {
                            Console.WriteLine("Combine failed.");
                            Environment.Exit(1);
                        }
                        passFlag = true;
                        break;

                    case CsvType.Vehicle:
                        Console.WriteLine("Combining 3.Vehicle CSV files...");
                        Console.WriteLine("{0}", carCode);
                        csvPaths = SearchCsv(logPath, startDT, endDT, carCode, "Vehicle");

                        Console.Write("Directory to store results.\n>");
                        dest = Console.ReadLine();
                        checker = CombineCsvFiles(csvPaths, dest);
                        if (!checker)
                        {
                            Console.WriteLine("Combine failed.");
                            Environment.Exit(1);
                        }
                        passFlag = true;
                        break;
                }
            } while (passFlag == false);

            Console.WriteLine("Press any key to EXIT.");
            Console.ReadKey();
        }


        static List<string> SearchCsv(string logPath, DateTime startDT, DateTime endDT, string[] carCode, string csvType, bool particularTime = true)
        {
            var emptyString = new List<string>();
            if (!Directory.Exists(logPath))
            {
                return emptyString;
            }
            if (DateTime.Compare(startDT, endDT) > 0)
            {
                return emptyString;
            }

            if (!particularTime)
            {
                startDT = DateTime.MinValue;
                endDT = DateTime.MaxValue;
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
                startIndex = ~startIndex;
                // 리스트에 특정 값이 없다면, 음수로 표현 되기에 ~ 을 통해 양수로 변환(1의 보수).
            }

            int endIndex = directoryNames.BinarySearch(endDT.ToString("yyyyMMdd"));
            if (endIndex < 0)
            {
                endIndex = ~endIndex - 1;
            }

            for (int k = startIndex; k <= endIndex; k++)
            {
                datePaths.Add(directoryNames[k]);
            }

            string csvPath = string.Empty;

            switch (csvType)
            {
                case "Gap":
                    csvPath = "gap_values.csv";
                    break;
                case "Glass":
                    csvPath = "glass_shift_values.csv";
                    break;
                case "Vehicle":
                    csvPath = "vehicle_shift_values.csv";
                    break;
            }

            var resultPaths = new List<string>();
            string resultPath = string.Empty;
            for (int j = 0; j < datePaths.Count; j++)
            {
                for (int k = 0; k < carCode.Length; k++)
                {
                    resultPath = $"{logPath}/{datePaths[j]}/{carCode[k]}/{csvPath}";

                    if (File.Exists(resultPath))
                    {
                        resultPaths.Add(resultPath);
                    }
                }
            }

            return resultPaths;
        }


        static bool CombineCsvFiles(List<string> csvPaths, string resultPath)
        {
            List<string> allLines = new List<string>();
            if (!csvPaths.Any())
            {
                return false;
            }

            allLines.AddRange(File.ReadAllLines(csvPaths[0]));
            for (int i = 1; i < csvPaths.Count; i++)
            {
                allLines.AddRange(File.ReadAllLines(csvPaths[i]).Skip(1));
            }
            File.WriteAllLines(resultPath, allLines);

            return true;
        }
    }
}