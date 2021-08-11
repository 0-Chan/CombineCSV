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

        public enum CarCode // NO Enum with CarCode!
        {
            CN7,
            DN8,
            TL,
            DH
		}

        static void Main(string[] args)
		{
            string logPath = "/home/cody/workspace/VALUES/";
            string dest = string.Empty;
            System.Collections.Generic.List<string> csvPaths = null;
            bool passFlag = false;
            bool checker = true;

            if (System.IO.Directory.Exists(logPath))
            {
                System.Console.WriteLine("Please enter the date in 'yyyyMMdd' type.");
                System.Console.Write("Start Date :");
                string startInput = System.Console.ReadLine();
                System.Console.Write("End Date :");
                string endInput = System.Console.ReadLine();

                checker = System.DateTime.TryParseExact(startInput, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out System.DateTime startDT);
                checker = System.DateTime.TryParseExact(endInput, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out System.DateTime endDT);

                if (checker == false)
                {
                    System.Console.WriteLine("DateTime Parse failed.");
                    System.Environment.Exit(1);
                }

                System.Console.WriteLine("\n Which values do you want to combine?");
                do
                {
                    System.Console.Write("1. Gap // 2. Glass // 3. Vehicle (Enter a number.)\n>");
                    int input = System.Convert.ToInt32(System.Console.ReadLine());
                    input = input - 1;
                    var myType = (CsvCode)input;

                    switch (myType)
                    {
                        case CsvCode.Gap:
                            System.Console.WriteLine("Combining 1.Gap CSV files...");
                            System.Console.WriteLine("1. CN7");
                            csvPaths = SearchCSV(logPath, startDT, endDT, CarCode.CN7, CsvCode.Gap);

                            System.Console.Write("Directory to store results.\n>");
                            dest = System.Console.ReadLine();
                            checker = CombineCSVFiles(csvPaths, dest);

                            passFlag = true;
                            break;

                        case CsvCode.Glass:
                            System.Console.WriteLine("Combining 2.Glass CSV files...");
                            System.Console.WriteLine("1. CN7");
                            csvPaths = SearchCSV(logPath, startDT, endDT, CarCode.CN7, CsvCode.Glass);

                            System.Console.Write("Directory to store results.\n>");
                            dest = System.Console.ReadLine();
                            checker = CombineCSVFiles(csvPaths, dest);
                            passFlag = true;
                            break;

                        case CsvCode.Vehicle:
                            System.Console.WriteLine("Combining 3.Vehicle CSV files...");
                            System.Console.WriteLine("1. CN7");
                            csvPaths = SearchCSV(logPath, startDT, endDT, CarCode.CN7, CsvCode.Vehicle);

                            System.Console.Write("Directory to store results.\n>");
                            dest = System.Console.ReadLine();
                            checker = CombineCSVFiles(csvPaths, dest);
                            passFlag = true;
                            break;
                    }
                } while (passFlag == false);
            }

            System.Console.WriteLine("Press any key to EXIT.");
            System.Console.ReadKey();
		}


        static System.Collections.Generic.List<string> SearchCSV(string logPath, System.DateTime startDT, System.DateTime endDT, CarCode car, CsvCode csv) // input datetime
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logPath);
            System.Collections.Generic.List<string> datePaths = new System.Collections.Generic.List<string>();
            bool beginFlag = false;
            bool endFlag = false;

            var directoryNames = di.EnumerateDirectories()
                    .OrderBy(d => d.Name)
                    .Select(d => d.Name)
                    .ToList();

            int refNum = directoryNames.Count - 1;
            int cenNum = refNum / 2;
            double x = 0;

            for (;beginFlag == false;)
            {
                System.DateTime.TryParseExact(directoryNames[cenNum], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out System.DateTime centerDT);
                if (System.DateTime.Compare(startDT, centerDT) == 0 || cenNum <= 0 || cenNum >= directoryNames.Count - 1)
                {
                    datePaths.Add(directoryNames[cenNum]);
                    beginFlag = true;
                    break;
                }
                else if (System.DateTime.Compare(startDT, centerDT) < 0) // startDT is later!
                {
                    refNum = cenNum;
                    cenNum = cenNum / 2;
                    System.Console.WriteLine("yyyyyyy :" + cenNum);
                }
                else if (System.DateTime.Compare(startDT, centerDT) > 0) // startDT is earlier.
                {
                    x = ((double)refNum - (double)cenNum) / 2 + 0.5;
                    cenNum += (int)x;
                    System.Console.WriteLine("xxxxxxxxx :"+ cenNum);
                }
            }


            for (;endFlag == false;)
            {
                cenNum++;

                if (cenNum >= directoryNames.Count)
                {
                    endFlag = true;
                    break;
                }

                System.DateTime.TryParseExact(directoryNames[cenNum], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out System.DateTime compareDT);
                if (System.DateTime.Compare(endDT, compareDT) >= 0)
                {
                    datePaths.Add(directoryNames[cenNum]);
                }
                else
                {
                    endFlag = true;
                    break;
                }
            }


            // Print the list.
            foreach (var datePath in datePaths)
            {
                System.Console.WriteLine(datePath);
            }

            System.Console.WriteLine("Press Ctrl+C key to EXIT.");
            System.Console.ReadKey();





            /*
            for (int i = 0; i < directoryNames.Count; i++)
            {
                System.DateTime.TryParseExact(directoryNames[i], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out System.DateTime compareDT);

                if (System.DateTime.Compare(startDT, compareDT) <= 0 && beginFlag == false)
                {
                    datePaths.Add(directoryNames[i]);
                    beginFlag = true;
                }
                else if (System.DateTime.Compare(compareDT, endDT) <= 0 && beginFlag == true)
                {
                    datePaths.Add(directoryNames[i]);
                }
                else if (System.DateTime.Compare(compareDT, endDT) > 0)
                {
                    System.Console.WriteLine("Done.");
                    break;
		       }
            }
            */

            string csvPath = string.Empty;
            string carPath = string.Empty;
            switch (car)
            {
                case CarCode.CN7:
                    carPath = "/CN7";
                    break;
                case CarCode.DH:
                    carPath = "/DH";
                    break;
                case CarCode.DN8:
                    carPath = "/DN8";
                    break;
                case CarCode.TL:
                    carPath = "/TL";
                    break;
            }

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

            System.Collections.Generic.List<string> resultPaths = new System.Collections.Generic.List<string>();
            for (int j = 0; j < datePaths.Count; j++)
            {
                resultPaths.Add(logPath + datePaths[j] + carPath + csvPath);
                System.Console.WriteLine("{0}.Path : {1} ", j+1, datePaths[j]);
            }

            return resultPaths;
        }


        static bool CombineCSVFiles(System.Collections.Generic.List<string> csvPaths, string resultPath)
        {
            System.IO.StreamWriter fileDest = new System.IO.StreamWriter(resultPath, true);

            for(int i = 0; i < csvPaths.Count; i++)
            {
                string file = csvPaths[i];
                if (file == null)
                {
                    System.Console.WriteLine("Error, CSV file is missing.");
                    return false;
                }

                string[] lines = System.IO.File.ReadAllLines(file);
                if (i > 0)
                {
                    lines = lines.Skip(1).ToArray();
                }

                foreach (string line in lines)
                {
                    fileDest.WriteLine(line);
                }
			}
            fileDest.Close();
            return true;
		}
    }
}








/*
            int j = di.GetDirectories().Length; // 총 날짜 폴더 개수
            string[] csvPaths = new string[i];

            int x = 0;
            // foreach 구문을 이용하여 폴더 내부에 있는 폴더정보(GetDirectories()를 이용해)를 가져옵니다.
            System.Console.WriteLine("-------------------------------------");
            System.Console.WriteLine("CSV NAME : " + csvPath);

            foreach (var item in di.GetDirectories())
            {
                System.Console.WriteLine("{0}. FOLDER NAME : {1}", x + 1, item.Name);

                foreach (var fi in item.GetFiles())
                {
                    string fileName = System.IO.Path.GetFileName(fi.Name);
                    if (fileName == csvPath)
                    {
                        csvPaths[x] = fi.FullName; // 차종이 2개면..
                        x++;
                    }
                }
            }
            System.Console.WriteLine("-------------------------------------");
 */
