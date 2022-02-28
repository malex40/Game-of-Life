using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Life
{
    class VonNeumann : Neighbourhood
    {

        public VonNeumann(int order, bool centre) : base(order, centre)
        {
            
        }

        public override int GetNeighbours(int[,] universe, int i, int j, bool periodic)
        {
            int rows = universe.GetLength(0);
            int columns = universe.GetLength(1);

            int order = base.GetOrder();



            int neighbours = 0;
            if (!periodic)
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        if (r >= 0 && r < rows && c >= 0 && c < columns)
                        {
                            neighbours += universe[r, c];
                        }
                    }
                }
            }
            else
            {
                for (int r = i - order; r <= i + order; r++)
                {
                    for (int c = j - order; c <= j + order; c++)
                    {
                        neighbours += universe[Modulus(r, rows), Modulus(c, columns)];
                    }
                }
            }

            if (!base.GetCentre())
            {
                neighbours -= universe[i, j];
            }

            return neighbours;
        }

        // "Borrowed" from: https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        private static int Modulus(int x, int m)
        {
            return (x % m + m) % m;
        }


    }
}
