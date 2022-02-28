using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Life
{

    class Options
    {
        private const int MIN_DIMENSION = 4;
        private const int MAX_DIMENSION = 48;
        private const int MIN_GENERATION = 4;
        private const double MIN_UPDATE = 1.0;
        private const double MAX_UPDATE = 30.0;
        private const double MIN_RANDOM = 0.0;
        private const double MAX_RANDOM = 1.0;
        private const int MIN_ORDER = 1;
        private const int MAX_ORDER = 10;
        private const int MIN_SURVIVAL = 0;
        private const int MIN_BIRTH = 0;
        private const int MIN_MEMORY = 4;
        private const int MAX_MEMORY = 512;

        private int rows = 16;
        private int columns = 16;
        private int generations = 50;
        private double updateRate = 5.0;
        private double randomFactor = 0.5;
        private string inputFile = null;
        private string type = "moore";
        private int order = 1;
        private List<int> survivalRate = new List<int>() { 2, 3 };
        private List<int> birthRate = new List<int>() { 3 };
        private string survivalMessage = "2...3 ";
        private string birthMessage = "3 ";
        private int memory = 16;
        private int[][,] predecessors = new int[16][,];
        private string outputFile = null;
                

        public int Rows 
        {
            get => rows;
            set 
            {
                if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                {
                    throw new ArgumentException($"Row dimension \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_DIMENSION} - {MAX_DIMENSION})");
                }
                rows = value;
            } 
        }

        public int Columns
        {
            get => columns;
            set
            {
                if (value < MIN_DIMENSION || value > MAX_DIMENSION)
                {
                    throw new ArgumentException($"Column dimension \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_DIMENSION} - {MAX_DIMENSION})");
                }
                columns = value;
            }
        }

        public int Generations
        {
            get => generations;
            set
            {
                if (value < MIN_GENERATION)
                {
                    throw new ArgumentException($"Generation count \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_GENERATION} and above)");
                }
                generations = value;
            }
        }

        public double UpdateRate
        {
            get => updateRate;
            set
            {
                if (value < MIN_UPDATE || value > MAX_UPDATE)
                {
                    throw new ArgumentException($"Update rate \'{value:F2}\' is outside of the acceptable " +
                        $"range of values ({MIN_UPDATE} - {MAX_UPDATE})");
                }
                updateRate = value;
            }
        }

        public double RandomFactor
        {
            get => randomFactor;
            set
            {
                if (value < MIN_RANDOM || value > MAX_RANDOM)
                {
                    throw new ArgumentException($"Random factor \'{value:F2}\' is outside of the acceptable " +
                        $"range of values ({MIN_RANDOM} - {MAX_RANDOM})");
                }
                randomFactor = value;
            }
        }

        public string InputFile
        {
            get =>  inputFile;
            set
            {
                if (!File.Exists(value))
                {
                    throw new ArgumentException($"File \'{value}\' does not exist.");
                }
                if (!Path.GetExtension(value).Equals(".seed"))
                {
                    throw new ArgumentException($"Incompatible file extension \'{Path.GetExtension(value)}\'");
                }
                inputFile = value;
            }
        }

        public bool Periodic { get; set; } = false;

        public bool StepMode { get; set; } = false;

        public string Type
        {
            get => type;
            set
            {
                List<string> types = new List<string>() { "moore", "vonneumann" };
                if (!types.Contains(value.ToLower()))
                {
                    throw new ArgumentException($"Neighbourhood Type \'{value}\' is outside of the acceptable " +
                        $"options ('Moore' or 'VonNeumann')");
                }
                type = value;
            }
        }

        public int Order
        {
            get => order;
            set
            {
                if (value < MIN_ORDER || value > MAX_ORDER)
                {
                    throw new ArgumentException($"Order size \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_ORDER} - {MAX_ORDER})");
                }
                order = value;
            }
        }

        public bool Centre { get; set; } = false;         

        public List<int> SurvivalRate
        {
            get => survivalRate;
            set
            {                
                survivalRate = value;
            }
        }

        public List<int> BirthRate
        {
            get => birthRate;
            set
            {
                birthRate = value;
            }
        }
        
        public string SurvivalMessage
        {
            get => survivalMessage;
            set
            {
                survivalMessage = value;
            }
        }

        public string BirthMessage
        {
            get => birthMessage;
            set
            {
                birthMessage = value;
            }
        }


        public int Memory
        {
            get => memory;
            set
            {
                if (value < MIN_MEMORY || value > MAX_MEMORY)
                {
                    throw new ArgumentException($"Memory size \'{value}\' is outside of the acceptable " +
                        $"range of values ({MIN_MEMORY} - {MAX_MEMORY})");
                }
                memory = value;
                predecessors = new int[memory][,];
            }
        }

        public  int[][,] Predecessors
        {
            get => predecessors;
            set
            {
                predecessors = value;
            }
                
        }


        public string OutputFile
        {
            get => outputFile;
            set
            {
               
                if (!Path.GetExtension(value).Equals(".seed"))
                {
                    throw new ArgumentException($"Incompatible file extension \'{Path.GetExtension(value)}\'");
                }
                outputFile = value;
            }
        }

        public bool Ghost { get; set; } = false;




        public override string ToString()
        {
           
            const int padding = 30;            
            string output = "\n";
            output += "Input File: ".PadLeft(padding) + (InputFile != null ? InputFile : "N/A") + "\n";
            output += "Output File: ".PadLeft(padding) + (OutputFile != null ? OutputFile : "N/A") + "\n";
            output += "Generations: ".PadLeft(padding) + $"{Generations}\n";
            output += "Memory: ".PadLeft(padding) + $"{Memory}\n";
            output += "Update Rate: ".PadLeft(padding) + $"{UpdateRate} updates/s\n";
            output += "Rules: ".PadLeft(padding) + $"S( {SurvivalMessage}) " +
                      $"B( {BirthMessage})\n";
            output += "Neighbourhood: ".PadLeft(padding) + (Type.Equals("moore") ? "Moore" : "VonNeumann") +
                       $" ({Order}) \n";
            output += "Periodic: ".PadLeft(padding) + (Periodic ? "Yes" : "No") + "\n";
            output += "Rows: ".PadLeft(padding) + Rows + "\n";
            output += "Columns: ".PadLeft(padding) + Columns + "\n";
            output += "Random Factor: ".PadLeft(padding) + $"{100 * RandomFactor:F2}%\n";
            output += "Step Mode: ".PadLeft(padding) + (StepMode ? "Yes" : "No") + "\n";
            output += "Ghost Mode: ".PadLeft(padding) + (Ghost ? "Yes" : "No") + "\n";
            return output;
        }

        public void CheckRules()
        {
            int checkSum = 0;

            if (Type.Equals("moore"))
            {
                checkSum = (((Order + 1) * 4) * Order) + (Centre ? 1 : 0);
            }

            if (Type.Equals("vonneumann"))
            {
                checkSum = (((Order + 1) * 2) * Order) + (Centre ? 1 : 0);
            }
       
            if (!(SurvivalRate.Count() == 0))
            {
                if (SurvivalRate.Min() < 0)
                {
                    survivalRate = new List<int>() { 2, 3 };
                    survivalMessage = "2...3 ";
                    throw new ArgumentException($"Survival rate is outside of the " +
                        $"acceptable range of values ({MIN_SURVIVAL} and above)");
                }               

                if (SurvivalRate.Max() > checkSum)
                {
                    survivalRate = new List<int>() { 2, 3 };
                    survivalMessage = "2...3 ";
                    throw new ArgumentException($"Survival rate is outside of the " +
                        $"acceptable range of values (Less than or equal to the number of neighbouring cells)");
                }

            }

            if (!(BirthRate.Count() == 0))
            {
                if (BirthRate.Min() < 0)
                {
                    birthRate = new List<int>() { 3 };
                    birthMessage = "3 ";
                    throw new ArgumentException($"Survival rate is outside of the " +
                        $"acceptable range of values ({MIN_BIRTH} and above)");
                }

                if (BirthRate.Max() > checkSum)
                {
                    birthRate = new List<int>() { 3 };
                    birthMessage = "3 ";
                    throw new ArgumentException($"Survival rate is outside of the " +
                        $"acceptable range of values (Less than or equal to the number of neighbouring cells)");
                }

            }            

        }

    }
}
