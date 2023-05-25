using System.Text;

namespace FirebirdNetConverter.Application;

public class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Enter path to firebird file:");

            var pathInput = Console.ReadLine();

            if (pathInput == null)
            {
                Console.WriteLine("Incorrect path.");
                Console.ReadKey();
                return;
            }

            var stream = new StreamReader(pathInput);

            var fileOutput = new FileInfo($"{pathInput}_converted");
            var lineBuilder = new StringBuilder();

            var line = stream.ReadLine();
            using (var streamWriter = fileOutput.CreateText())
            {
                while (line != null)
                {
                    try
                    {
                        lineBuilder.Clear();

                        var splitted = line.Trim().Replace(" TYPE OF", "").Split(' ');

                        var columnName = splitted[0];
                        var sqlType = splitted[1].ToUpper();

                        lineBuilder.Append("public ");
                        if (sqlType.Contains("VARCHAR")
                            || sqlType.Contains("CHAR")
                            || sqlType.Contains("CHARACTER")
                            || sqlType.Contains("T_STRING"))
                        {
                            lineBuilder.Append("string?");
                        }
                        else if (sqlType.Contains("INTEGER"))
                        {
                            lineBuilder.Append("int?");
                        }
                        else if (sqlType.Contains("SMALLINT"))
                        {
                            lineBuilder.Append("short?");
                        }
                        else if (sqlType.Contains("T_ID"))
                        {
                            lineBuilder.Append("long?");
                        }
                        else if (sqlType.Contains("TIMESTAMP")
                                 || sqlType.Contains("DATE"))
                        {
                            lineBuilder.Append("DateTime?");
                        }
                        else if(sqlType.Contains("TIME"))
                        {
                            lineBuilder.Append("TimeSpan?");
                        }
                        else if (sqlType.Contains("BOOLEAN"))
                        {
                            lineBuilder.Append("bool?");
                        }
                        else if (sqlType.Contains("NUMERIC"))
                        {
                            lineBuilder.Append("decimal?");
                        }

                        lineBuilder.Append($" {columnName}");
                        lineBuilder.Append(" { get; set; }");

                        streamWriter.WriteLine($"[Column(\"{columnName}\")]");
                        streamWriter.WriteLine(lineBuilder.ToString());
                        streamWriter.WriteLine();

                        line = stream.ReadLine();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error in {line}");
                        Console.WriteLine(ex.Message);
                        line = stream.ReadLine();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Convertion complete.");
        Console.ReadKey();
    }
}
