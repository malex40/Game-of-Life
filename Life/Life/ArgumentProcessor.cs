using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Life
{
    static class ArgumentProcessor
    {

        public static Options Process(string[] args)
        {
            Options options = new Options();
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--dimensions":
                            ProcessDimensions(args, i, options);
                            break;
                        case "--generations":
                            ProcessGenerations(args, i, options);
                            break;
                        case "--max-update":
                            ProcessUpdateRate(args, i, options);
                            break;
                        case "--random":
                            ProcessRandomFactor(args, i, options);
                            break;
                        case "--seed":
                            ProcessInputFile(args, i, options);
                            break;
                        case "--periodic":
                            options.Periodic = true;
                            break;
                        case "--step":
                            options.StepMode = true;
                            break;
                        case "--neighbour":
                            ProcessNeighbourhood(args, i, options);
                            break;
                        case "--survival":
                            ProcessSurvivalBirth(args, i, options);
                            break;
                        case "--birth":
                            ProcessSurvivalBirth(args, i, options);
                            break;
                        case "--memory":
                            ProcessMemory(args, i, options);
                            break;
                        case "--output":
                            ProcessOutputFile(args, i, options);
                            break;
                        case "--ghost":
                            options.Ghost = true;
                            break;

                    }
                }
                options.CheckRules();
                Logging.Success("Command line arguments processed without issue!");
            }
            catch (Exception exception)
            {
                Logging.Warning(exception.Message);
                Logging.Message("Reverting to defaults for unprocessed arguments...");
            }
            finally
            {
                Logging.Message("The following options will be used:");
                Console.WriteLine(options);
            }

            return options;
        }

        private static void ProcessDimensions(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "dimensions", 2);

            if (!int.TryParse(args[i + 1], out int rows))
            {
                throw new ArgumentException($"Row dimension \'{args[i + 1]}\' is not a valid integer.");
            }

            if (!int.TryParse(args[i + 2], out int columns))
            {
                throw new ArgumentException($"Column dimension \'{args[i + 2]}\' is not a valid integer.");
            }

            options.Rows = rows;
            options.Columns = columns;
        }

        private static void ProcessGenerations(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "generations", 1);

            if (!int.TryParse(args[i + 1], out int generations))
            {
                throw new ArgumentException($"Generation count \'{args[i + 1]}\' is not a valid integer.");
            }

            options.Generations = generations;
        }

        private static void ProcessUpdateRate(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "max-update", 1);

            if (!double.TryParse(args[i + 1], out double updateRate))
            {
                throw new ArgumentException($"Update rate \'{args[i + 1]}\' is not a valid double.");
            }

            options.UpdateRate = updateRate;
        }

        private static void ProcessRandomFactor(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "random", 1);

            if (!double.TryParse(args[i + 1], out double randomFactor))
            {
                throw new ArgumentException($"Random factor \'{args[i + 1]}\' is not a valid double.");
            }

            options.RandomFactor = randomFactor;
        }

        private static void ProcessInputFile(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "seed", 1);

            options.InputFile = args[i + 1];
        }

        private static void ProcessSurvivalBirth(string[] args, int i, Options options)
        {
            
            List<int> buffer = new List<int>();
            string message = "";
            
            if (i + 1 < args.Length)
            {
                for (int j = i + 1; j < args.Length; j++)
                {
                    if(!args[j].StartsWith("--"))
                    {
                        message += args[j] + " ";
                    }
                    if (!int.TryParse(args[j], out int rate))
                    {
                        if (!args[j].Contains("..."))
                        {
                            break;
                        }
                        string[] values = args[j].Split("...", System.StringSplitOptions.RemoveEmptyEntries);

                        if (!int.TryParse(values[0], out int lower)
                            || !int.TryParse(values[1], out int higher))
                        {
                            break;
                        }
                        for (int loop = lower; loop <= higher; loop++)
                        {
                            buffer.Add(loop);
                        }
                        continue;
                    }
                    buffer.Add(rate);
                }
            }
                if (args[i].Equals("--survival"))
                {
                    options.SurvivalRate.Clear();
                    options.SurvivalMessage = message;
                    foreach (int item in buffer)
                    {
                        options.SurvivalRate.Add(item);
                    }                
                }
                else if (args[i].Equals("--birth"))
                {
                    options.BirthRate.Clear();
                    options.BirthMessage = message;
                    foreach (int item in buffer)
                    {
                        options.BirthRate.Add(item);
                    } 
                }
        }

        public static void ProcessMemory(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "memory", 1);

            if (!int.TryParse(args[i + 1], out int memory))
            {
                throw new ArgumentException($"Memory count \'{args[i + 1]}\' is not a valid integer.");
            }

            options.Memory = memory;
        }


        private static void ProcessOutputFile(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "seed", 1);

            options.OutputFile = args[i + 1];
        }

        private static void  ProcessNeighbourhood(string[] args, int i, Options options)
        {
            ValidateParameterCount(args, i, "neighbour", 3);
            
            
            if (!int.TryParse(args[i + 2], out int order))
            {
                throw new ArgumentException($"Order count \'{args[i + 2]}\' is not a valid integer.");
            }

            if (!bool.TryParse(args[i + 3], out bool centre))
            {
                throw new ArgumentException($"Order count \'{args[i + 3]}\' is not a valid Boolean.");
            }

            options.Type = args[i + 1].ToLower();
            options.Order = order;
            options.Centre = centre;

        }

        private static void ValidateParameterCount(string[] args, int i, string option, int numParameters)
        {
            if (i >= args.Length - numParameters)
            {
                throw new ArgumentException($"Insufficient parameters for \'--{option}\' option " +
                    $"(provided {args.Length - i - 1}, expected {numParameters})");
            }
        }
    }
}
