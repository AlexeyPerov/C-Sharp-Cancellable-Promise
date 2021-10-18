using System;

namespace Promise.Examples
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var running = true;

            while (running)
            {
                Console.WriteLine("Input an example id [1..6] to run or type 'q' to quit");

                var line = Console.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    line = line.Trim().ToLowerInvariant();

                    switch (line)
                    {
                        case "q":
                            running = false;
                            break;
                        case "1":
                            Example1.Run();
                            break;
                        case "2":
                            Example2.Run();
                            break;
                        case "3":
                            Example3.Run();
                            break;
                        case "4":
                            Example4.Run();
                            break;
                        case "5":
                            Example5.Run();
                            break;
                        case "6":
                            Example6.Run();
                            break;
                    }
                }
            }

        }
    }
}