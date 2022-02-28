using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Display;

namespace Life
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options = ArgumentProcessor.Process(args);           
            int[,] universe = InitializeUniverse(options);
            Grid grid = new Grid(options.Rows, options.Columns);
            Neighbourhood neighbourhood = GetNeighbourhood(options);

            Logging.Message("Press spacebar to begin the game...");
            WaitSpacebar();

            grid.InitializeWindow();

            Stopwatch stopwatch = new Stopwatch();
            

            int iteration = 0;
            bool steadyState = false;
            while (iteration <= options.Generations)
            {               
                stopwatch.Restart();

                
                if (iteration != 0)
                {       
                    universe = EvolveUniverse(universe, options.Periodic, neighbourhood, options.SurvivalRate, options.BirthRate);
                    if (CheckPredecessors(universe, options, iteration))
                    {
                        steadyState = true;
                        goto End;
                    }
                }
                UpdatePredecessors(universe, options);


                UpdateGrid(grid, options, iteration);
                

                grid.SetFootnote($"Generation: {iteration++}");
                grid.Render();

                if (options.StepMode)
                {
                    WaitSpacebar();
                }
                else
                {
                    while (stopwatch.ElapsedMilliseconds < 1000 / options.UpdateRate) ;
                }
            }                      
       
            End:
            grid.IsComplete = true;
            grid.Render();            
            WaitSpacebar();

            grid.RevertWindow();
            Logging.Message(($"Steady-state {(steadyState ? "" : "not ")}detected..."));
            OutputToFile(options);
            Logging.Message("Press spacebar to exit program...");
            WaitSpacebar();
        }

        private static int[,] EvolveUniverse(int[,] universe, bool periodic, Neighbourhood neighbourhood, List<int> survivalRate, List<int> birthRate)
        {
            const int ALIVE = 1;
            const int DEAD = 0;

            int rows = universe.GetLength(0);
            int columns = universe.GetLength(1);
            

            int[,] buffer = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {

                    int neighbours = neighbourhood.GetNeighbours(universe, i, j, periodic);
                    

                    if (universe[i, j] == ALIVE && survivalRate.Contains(neighbours))
                    {                       
                        buffer[i, j] = ALIVE;
                    }
                    else if (universe[i, j] == DEAD && birthRate.Contains(neighbours))
                    {
                        buffer[i, j] = ALIVE;
                    }
                    else
                    { 
                        buffer[i, j] = DEAD;
                    }
                }
            }

            return buffer.Clone() as int[,];
        }

        private static void UpdateGrid(Grid grid, Options options, int iteration)
        {
            
            if (!options.Ghost)
            {
                for (int i = 0; i < options.Rows; i++)
                {
                    for (int j = 0; j < options.Columns; j++)
                    {
                        grid.UpdateCell(i, j, (CellState)options.Predecessors[0][i, j]);
                    }
                }
            }
            else
            {
                for (int k = 3; k >= 0; k--)
                {
                    for (int i = 0; i < options.Rows; i++)
                    {
                        for (int j = 0; j < options.Columns; j++)
                        {
                            if (k <= iteration)
                            {
                                if (k == 3 || options.Predecessors[k][i, j] == 1)
                                {
                                    grid.UpdateCell(i, j, (CellState)(options.Predecessors[k][i, j] * (k + 1)));
                                }
                            }
                        }
                    }
                }
            }

        }

        private static void WaitSpacebar()
        {
            while (Console.ReadKey(true).Key != ConsoleKey.Spacebar) ;
        }

        private static int[,] InitializeUniverse(Options options)
        {
            int[,] universe;

            if (options.InputFile == null)
            {
                universe = InitializeFromRandom(options.Rows, options.Columns, options.RandomFactor);
            }
            else
            {
                    universe = InitializeFromFile(options.Rows, options.Columns, options.InputFile);
            }

            return universe;
        }

        private static int[,] InitializeFromRandom(int rows, int columns, double randomFactor)
        {
            int[,] universe = new int[rows, columns];

            Random random = new Random();
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[i, j] = random.NextDouble() < randomFactor ? 1 : 0;
                }
            }

            return universe;
        }
        
        private static int[,] InitializeFromFile(int rows, int columns, string inputFile)
        {
            int[,] universe = new int[rows, columns];
            int[] maxUniverse = { 0, 0 };
            bool IsValid = true;

            using (StreamReader reader = new StreamReader(inputFile))
            {
                string line = reader.ReadLine();
                if (line.Equals("#version=1.0"))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        string[] elements = line.Split(" ");

                        int row = int.Parse(elements[0]);
                        int column = int.Parse(elements[1]);
                        maxUniverse[0] = row > maxUniverse[0] ? row : maxUniverse[0];
                        maxUniverse[1] = column > maxUniverse[1] ? column : maxUniverse[1];
                        if (row < rows && column < columns)
                        {
                            universe[row, column] = 1;
                        }
                        else
                        {
                            IsValid = false;
                        }
                                                                   
                    }
                }

                else if(line.Equals("#version=2.0"))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        line = line.Replace(',', ' ');
                        line = line.Replace(':', ' ');
                        string[] elements = line.Split(" ", System.StringSplitOptions.RemoveEmptyEntries);


                        if (elements[1].Equals("cell"))
                        {
                            int row = int.Parse(elements[2]);
                            int column = int.Parse(elements[3]);
                            maxUniverse[0] = row > maxUniverse[0] ? row : maxUniverse[0];
                            maxUniverse[1] = column > maxUniverse[1] ? column : maxUniverse[1];
                            if (row < rows && column < columns)
                            {
                                universe[row, column] = (elements[0].Contains('o') ? 1 : 0);
                            }
                            else
                            {
                                IsValid = false;
                            }
                        }
                        else
                        {
                            int rowBottom = int.Parse(elements[2]);
                            int colBottom = int.Parse(elements[3]);
                            int rowTop = int.Parse(elements[4]);
                            int colTop = int.Parse(elements[5]);
                            maxUniverse[0] = rowTop > maxUniverse[0] ? rowTop : maxUniverse[0];
                            maxUniverse[1] = colTop > maxUniverse[1] ? colTop : maxUniverse[1];
                            if (rowTop < rows && colTop < columns)
                            {
                                if (elements[1].Equals("rectangle"))
                                {
                                    InFileRectangle(rowBottom, colBottom, rowTop, colTop, universe, elements[0]);
                                }
                                else if (elements[1].Equals("ellipse"))
                                {
                                    InFileEllipse(rowBottom, colBottom, rowTop, colTop, universe, elements[0]);
                                }
                            }
                            else
                            {
                                IsValid = false;
                            }

                        }

                    }
                }
            }

            if(!IsValid)
            {
                Logging.Warning($"Error initializing universe using \'{inputFile}\'.");
                Logging.Warning($"Minimum dimensions recommended: {maxUniverse[0] + 1} x {maxUniverse[1] + 1}. Universe initialized with only valid cells.");
            }

            return universe;
        }

        private static void InFileRectangle(int rowBottom, int colBottom, int rowTop, int colTop, int[,] universe, string state)
        {
   
            for (int i = rowBottom; i <= rowTop; i++)
            {
                for (int j = colBottom; j <= colTop; j++)
                {
                    universe[i, j] = (state.Contains('o') ? 1 : 0);
                }
            }

        }

        private static void InFileEllipse(int rowBottom, int colBottom, int rowTop, int colTop, int[,] universe, string state)
        {
            int width = rowTop - rowBottom + 1;
            int height = colTop - colBottom + 1;
            double centreX = ((double)(width - 1) / 2) + rowBottom;
            double centreY = ((double)(height - 1) / 2) + colBottom;

            for (int i = rowBottom; i <= rowTop; i++)
            {
                for (int j = colBottom; j <= colTop; j++)
                {
                    double valid = ((4 * Math.Pow((i - centreX), 2)) / Math.Pow(width, 2)) + ((4 * Math.Pow((j - centreY), 2)) / Math.Pow(height, 2));
                    if (valid <= 1)
                    {
                        universe[i, j] = (state.Contains('o') ? 1 : 0);
                    }
                }
            }

        }


        private static Neighbourhood GetNeighbourhood(Options options)
        {
            Neighbourhood neighbourhood = null;
            if (options.Type.Equals("moore"))
            {
                neighbourhood = new Moore(options.Order, options.Centre);
            }            
            
            else if (options.Type.Equals("vonneumann"))
            {
                neighbourhood = new VonNeumann(options.Order, options.Centre);
            }
            
            return neighbourhood;
        }
        

        private static void UpdatePredecessors(int[,] universe, Options options)
        {
            for (int i = options.Predecessors.Count() - 1; i > 0; i--)
            {
                options.Predecessors[i] = options.Predecessors[i - 1];
            }
            options.Predecessors[0] = universe;
        }


        private static bool CheckPredecessors(int[,] universe, Options options, int iteration)
        {
            int SameCounter = 0;
            for (int i = 1; i < options.Memory; i++)
            {
                SameCounter = 0;
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    for (int k = 0; k < universe.GetLength(1); k++)
                    {

                        if (iteration > i)
                        {
                            if (options.Predecessors[0][j, k] == options.Predecessors[i][j, k])
                            {
                                SameCounter++;
                            }
                        }
                    }
                }
                if (SameCounter ==  universe.Length)
                {
                    return true;
                }
            }

            return false;
        }

        public static void OutputToFile(Options options)
        {
            if (options.OutputFile != null)
            {
                using (StreamWriter writer = new StreamWriter(options.OutputFile))
                {
                    writer.WriteLine("#version=2.0");
                    for (int i = 0; i < options.Predecessors[0].GetLength(0); i++)
                    {
                        for (int j = 0; j < options.Predecessors[0].GetLength(1); j++)
                        {
                            if (options.Predecessors[0][i, j] == 1)
                            {
                                writer.WriteLine($"(o) cell: {i}, {j}");
                            }
                        }
                    }
                }
                Logging.Success($"Final generation written to file: {options.OutputFile}");
            }
        }




    }
}
