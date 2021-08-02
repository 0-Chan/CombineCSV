using System.Linq;  // 자주 쓰는 친구들만 using

namespace CombineCSV        // 재사용성을 고려할 것
{
	class Program
	{
        public enum setType
        {
            Gap = 1,
            Glass,
            Vehicle
		}

        public enum carCode
        {
            CN7,
            DN8,
            TL,
            DH
		}

 //       static System.Collections.Generic.List<string> SearchCSV(string logPath, setType type, carCode code = carCode.ALL);
 //       static void CombineCSVFiles(System.Collections.Generic.List<string> csvPaths, string resultPath);
        
        static void Main(string[] args)
		{
            string logPath = @"D:\\CombineCSV\\VALUES\\";
            string name = string.Empty;
            string input = string.Empty;
            int flag = 0;

            if (System.IO.Directory.Exists(logPath))
            {
                // 기간 입력
                System.Console.WriteLine("Please enter the date. (yyyyMMdd) \n");

                System.Console.Write("Start Date :");
                string date1 = System.Console.ReadLine();
                System.DateTime dt1 ;                
                System.DateTime.TryParseExact(date1, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dt1);

                System.Console.Write("End Date :");
                string date2 = System.Console.ReadLine();
                System.DateTime dt2;
                System.DateTime.TryParseExact(date2, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dt2);

                System.Console.WriteLine("\n start : {0} \n end : {1}", date1, date2);
                System.Console.WriteLine("\n start : {0} \n end : {1}", dt1, dt2);

                System.Console.WriteLine("\n What values do you want to combine?");
                do
                {
                    System.Console.Write("1. Gap // 2. Glass // 3. Vehicle (Enter a number) \n> ");
                    input = System.Console.ReadLine();
                    
                    var myType = (setType)System.Enum.Parse(typeof(setType), input);

                    System.Collections.Generic.List<string> csvPaths = null;

                    System.Console.WriteLine("Input type : " + myType);

                    switch (myType)
                    {
                        case setType.Gap:
                            System.Console.WriteLine("Combining 1.Gap setType CSV files...");
                            csvPaths = SearchCSV(logPath, date1, date2, carCode.CN7, setType.Gap);
//                          CombineCSVFiles(csvPaths);
                            flag = 1;
                            break;

                        case setType.Glass:
                            System.Console.WriteLine("Combining 2.Glass setType CSV files...");
                            csvPaths = SearchCSV(logPath, date1, date2, carCode.CN7, setType.Gap);
//                          CombineCSVFiles(csvPaths);
                            flag = 1;
                            break;

                        case setType.Vehicle:
                            System.Console.WriteLine("Combining 3.Vehicle setType CSV files...");
                            csvPaths = SearchCSV(logPath, date1, date2, carCode.CN7, setType.Gap);
//                          CombineCSVFiles(csvPaths);
                            flag = 1;
                            break;
                    }
                } while (flag == 0);
            }
            System.Console.WriteLine("Press any key to EXIT.");
            System.Console.ReadKey();
		}


        static System.Collections.Generic.List<string> SearchCSV(string logPath, string startDate, string endDate, carCode code, setType type)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(logPath);
            System.Collections.Generic.List<string> csvPaths = new System.Collections.Generic.List<string>();
            bool flag = false;

            // 모든 DIR을 이름 순서(= 날짜 순)로 정렬하여 list에
            var directoryNames = di.EnumerateDirectories()
                    .OrderBy(d => d.Name)
                    .Select(d => d.Name)
                    .ToList();

            // startDate와 비교하여 리스트에

            System.DateTime startDT;
            System.DateTime endDT;
            System.DateTime compareDT;

            System.DateTime.TryParseExact(startDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out startDT);
            System.DateTime.TryParseExact(endDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out endDT);


            for (int i = 0; i < directoryNames.Count; i++)
            {
                System.DateTime.TryParseExact(directoryNames[i], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out compareDT);

                if (System.DateTime.Compare(startDT, compareDT) <= 0 && flag == false)
                {
                    System.Console.WriteLine("Start! : " + directoryNames[i]);
					csvPaths.Add(directoryNames[i]);
                    flag = true;
               	}
                else if (System.DateTime.Compare(compareDT, endDT) <= 0 && flag == true)
                {
                    System.Console.WriteLine("… : " + directoryNames[i]);
                    csvPaths.Add(directoryNames[i]);
                }
                else if (System.DateTime.Compare(compareDT, endDT) > 0)
                {
                    System.Console.WriteLine("Done.");
                    break;
				}
            }

            string searchType = string.Empty;
            string searchCode = string.Empty;
            switch (code)
            {
                case carCode.CN7:
                    searchCode = @"\\CN7";
                    break;
                case carCode.DH:
                    searchCode = @"\\DH";
                    break;
                case carCode.DN8:
                    searchCode = @"\\DN8";
                    break;
                case carCode.TL:
                    searchCode = @"\\TL";
                    break;
            }

            switch (type)
            {
                case setType.Gap:
                    searchType = @"\\gap_values.csv";
                    break;
                case setType.Glass:
                    searchType = @"\\glass_shift_values.csv";
                    break;
                case setType.Vehicle:
                    searchType = @"\\vehicle_shift_values.csv";
                    break;
            }

            for (int j = 0; j < csvPaths.Count; j++)
            {
                csvPaths[j] = logPath + csvPaths[j] + searchCode + searchType;
                System.Console.WriteLine("checking! : " + csvPaths[j]);
            }

            System.Console.ReadKey();
            return csvPaths;
        }







        static void CombineCSVFiles(System.Collections.Generic.List<string> csvPaths, string resultPath) // output 경로 입력받기
        {
            System.IO.StreamWriter fileDest = new System.IO.StreamWriter(@"D:\CombineCSV\test.csv", true); // 경로 입력받기
            int i = 0;

            foreach (string count in csvPaths)
            {
                string file = csvPaths[i];
                if (file == null)
                {
                    System.Console.WriteLine("Error, CSV file is missing.");   // 폴더 내 Combine 할 csv 파일이 없음 → 종료
                    System.Environment.Exit(0);
                }

                string[] lines = System.IO.File.ReadAllLines(file);

                if (i > 0)
                {
                    lines = lines.Skip(1).ToArray();    // for문 쓰기
                }

                foreach (string line in lines)
                {
                    fileDest.WriteLine(line);
                }
                i++;
			}
            fileDest.Close();
		}
    }
}


















//               System.Console.WriteLine("ROOT PATH(logPath) : " + di.Name);
//               System.Console.WriteLine("LOGS COUNT : " + di.GetDirectories().Length);

//for (int a = 0; a < directories.Count; a++)
//{    System.Console.WriteLine(directories[a]);}


/*
            int j = di.GetDirectories().Length; // 총 날짜 폴더 개수
            string[] csvPaths = new string[i]; // 함수 안에서 만들기-

            int x = 0;
            // foreach 구문을 이용하여 폴더 내부에 있는 폴더정보(GetDirectories()를 이용해)를 가져옵니다.
            System.Console.WriteLine("-------------------------------------");
            System.Console.WriteLine("CSV NAME : " + searchType);

            foreach (var item in di.GetDirectories())
            {
                System.Console.WriteLine("{0}. FOLDER NAME : {1}", x + 1, item.Name);

                foreach (var fi in item.GetFiles())
                {
                    string fileName = System.IO.Path.GetFileName(fi.Name);
                    if (fileName == searchType)
                    {
                        csvPaths[x] = fi.FullName; // 차종이 2개면..
                        x++; // 리스트로
                    }
                }
            }
            System.Console.WriteLine("-------------------------------------");
 */