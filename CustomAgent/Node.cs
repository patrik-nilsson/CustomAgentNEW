using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class Node
    {
        public int distanceFromStart;
        public int distanceToGoal;
        public int weight;
        public Point Position;
        public Node(int distanceFromStart, int distanceToGoal, Point Position)
        {
            this.distanceFromStart = distanceFromStart;
            this.distanceToGoal = distanceToGoal;
            weight = distanceFromStart + distanceToGoal;
            this.Position = Position;
        }
    }
}
