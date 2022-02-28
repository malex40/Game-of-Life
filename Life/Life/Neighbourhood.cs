using System;
using System.Collections.Generic;
using System.Text;

namespace Life
{
    public abstract class Neighbourhood
    {

        private int order;
        private bool centre;

        public Neighbourhood( int order, bool centre)
        {
            this.order = order;
            this.centre = centre;
            
        }

        public int GetOrder()
        {
            return order;
        }

        public bool GetCentre()
        {
            return centre;
        }

        public abstract int GetNeighbours(int[,] universe, int row, int col, bool periodic);
         // take position of cell.
         // with type/order/centre... do something
         // return neighbourhood dimensions for CountNeighbours???
            
        



    }
}
