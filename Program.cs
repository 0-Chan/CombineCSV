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
            string logPath = "D:/CombineCSV/VALUES";

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
                            csvPaths = SearchCsv(logPath, startDT, endDT, "CN7", CsvCode.Gap);

                            Console.Write("Directory to store results.\n>");
                            dest = Console.ReadLine();
                            checker = CombineCsvFiles(csvPaths, dest);

                            passFlag = true;
                            break;

                        case CsvCode.Glass:
                            Console.WriteLine("Combining 2.Glass CSV files...");
                            Console.WriteLine("1. CN7");
                            csvPaths = SearchCsv(logPath, startDT, endDT, "CN7", CsvCode.Glass);

                            Console.Write("Directory to store results.\n>");
                            dest = Console.ReadLine();
                            checker = CombineCsvFiles(csvPaths, dest);
                            passFlag = true;
                            break;

                        case CsvCode.Vehicle:
                            Console.WriteLine("Combining 3.Vehicle CSV files...");
                            Console.WriteLine("1. CN7");
                            csvPaths = SearchCsv(logPath, startDT, endDT, "CN7", CsvCode.Vehicle);

                            Console.Write("Directory to store results.\n>");
                            dest = Console.ReadLine();
                            checker = CombineCsvFiles(csvPaths, dest);
                            passFlag = true;
                            break;
                    }
                } while (passFlag == false);
            }

            Console.WriteLine("Press any key to EXIT.");
            Console.ReadKey();
        }


        static List<string> SearchCsv(string logPath, DateTime startDT, DateTime endDT, string carCode, CsvCode csv)
        {

            // 날짜 잘못 입력 시
            //logPath exist 
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
                endIndex = ~endIndex -1;
                endIndex -= 1;
            }

            // endIndex 

            for (int k = startIndex; k <= endIndex; k++)
            {
                datePaths.Add(directoryNames[k]);
            }

            string csvPath = string.Empty;

            switch (csv)
            {
                case CsvCode.Gap:
                    csvPath = "gap_values.csv";
                    break;
                case CsvCode.Glass:
                    csvPath = "glass_shift_values.csv";
                    break;
                case CsvCode.Vehicle:
                    csvPath = "vehicle_shift_values.csv";
                    break;
            }

            var resultPaths = new List<string>();
            string resultPath = string.Empty;
            for (int j = 0; j < datePaths.Count; j++)
            {
                resultPath = $"{logPath}/{datePaths[j]}/{carCode}/{csvPath}";

                if (File.Exists(resultPath))
                {
                    resultPaths.Add(resultPath);
                }
            }

            return resultPaths;
        }


        static bool CombineCsvFiles(List<string> csvPaths, string resultPath)
        {
            List<string> allLines = new List<string>();

            // exception 체크

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