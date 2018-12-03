using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.AI
{
    class Node
    {
        public int weight { get; private set; }
        public Point position { get; private set; }
        public int stepsFromStart { get; private set; }
        public int stepsToGoal { get; private set; }
        public Node(int stepsFromStart, int stepsToGoal, Point position)
        {
            weight = stepsFromStart + stepsToGoal;
            this.position = position;
            this.stepsFromStart = stepsFromStart;
            this.stepsToGoal = stepsToGoal;
        }
    }
}
