using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor.AI
{
    class CustomAgent : Agent
    {
        GameData data;
        LinkedList<MoveAction> moves = new LinkedList<MoveAction>();
        List<LinkedList<Node>> paths = new List<LinkedList<Node>>();
        Point blocked;
        Point currentPosition;
        bool initalized = false;
        int goalPosition;
        int optimalDirection;
        int knownNumberOfEnemyWalls = 1000;
        int distanceToGoal;
        int distanceFromStart;
        int currentPath;
        int currentWeight;
        int forward;
        int backward;
        int rightward;
        int leftward;
        int index;

        public static void Main()
        {
            new CustomAgent().Start();
        }

        public override Action DoAction(GameData status)
        {
            if (!initalized)
            {
                data = status;
                Initalize();
                initalized = true;
            }
            if (moves.Count > 0)
            {
                moves.RemoveFirst();
            }

            foreach (Player p in status.Players)
            {
                if (p != status.Self)
                {
                    blocked = p.Position;
                    if (ThereIsBlocker())
                    {
                        knownNumberOfEnemyWalls = p.NumberOfWalls;
                        moves.Clear();
                        paths.Clear();
                        distanceFromStart = 1;
                        FindPath("none");
                    }
                    else if (p.NumberOfWalls < knownNumberOfEnemyWalls)
                    {
                        knownNumberOfEnemyWalls = p.NumberOfWalls;
                        if (ThereIsWall())
                        {
                            moves.Clear();
                            paths.Clear();
                            distanceFromStart = 1;
                            FindPath("none");
                        }
                    }
                }
            }
            return moves.First();
        }

        private void FindPath(string direction)
        {
            bool pathAddedToCurrentPath = false;
            if (direction == "none")
            {
                DrawInitialPath();
                FindLowestWeight();
            }
            if (currentWeight < paths[currentPath].First.Value.weight)
            {
                FindLowestWeight();
                //direction = getDirection();
            }
            distanceFromStart = paths[currentPath].First.Value.distanceFromStart;
            currentPosition = paths[currentPath].First.Value.Position;

            if (direction != "backward" && !WallBetween(currentPosition, new Point(currentPosition.X, currentPosition.Y + optimalDirection)) && IsWithinGameBoard(currentPosition.X, currentPosition.Y + optimalDirection))
            {
                distanceFromStart++;
                paths[currentPath].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), new Point(currentPosition.X, currentPosition.Y + optimalDirection)));
                pathAddedToCurrentPath = true;
            }
            else
            {
                paths.RemoveAt(currentPath);
            }

            //if (direction != "left" && !WallBetween(currentPosition, new Point(currentPosition.X + 1, currentPosition.Y)) && IsWithinGameBoard(currentPosition.X + 1, currentPosition.Y))
            //{
            //    if (pathAddedToCurrentPath)
            //    {
            //        LinkedList<Node> temp = paths[currentPath];
            //        temp.AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), new Point(currentPosition.X + 1, currentPosition.Y)));
            //        paths.Add(temp);
            //    }
            //    else
            //    {
            //        distanceFromStart++;
            //        paths[currentPath].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), new Point(currentPosition.X + 1, currentPosition.Y)));
            //        pathAddedToCurrentPath = true;
            //    }
            //}

            //if (direction != "right" && !WallBetween(currentPosition, new Point(currentPosition.X - 1, currentPosition.Y)) && IsWithinGameBoard(currentPosition.X - 1, currentPosition.Y))
            //{
            //    if (pathAddedToCurrentPath)
            //    {
            //        LinkedList<Node> temp = paths[currentPath];
            //        temp.AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), data.Self.Position));
            //        paths.Add(temp);
            //    }
            //    else
            //    {
            //        distanceFromStart++;
            //        paths[currentPath].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), data.Self.Position));
            //        pathAddedToCurrentPath = true;
            //    }
            //}

            //if (direction != "forward" && !WallBetween(currentPosition, new Point(currentPosition.X, currentPosition.Y - optimalDirection)) && IsWithinGameBoard(currentPosition.X, currentPosition.Y - optimalDirection))
            //{
            //    if (pathAddedToCurrentPath)
            //    {
            //        LinkedList<Node> temp = paths[currentPath];
            //        temp.AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), new Point(currentPosition.X, currentPosition.Y - optimalDirection)));
            //        paths.Add(temp);
            //    }
            //    else
            //    {
            //        distanceFromStart++;
            //        paths[currentPath].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), new Point(currentPosition.X, currentPosition.Y - optimalDirection)));
            //        pathAddedToCurrentPath = true;
            //    }
            //}

            if (currentPosition.Y == goalPosition)
            {
                foreach (Node n in paths[currentPath])
                {
                    moves.AddFirst(new MoveAction(n.Position.X, n.Position.Y));
                }
                return;
                //Put nodes into moveaction list.
            }
            else
            {
                FindPath("unknown");
            }
            return;
        }

        private bool ThereIsWall()
        {
            if (paths.Count == 0)
            {
                return true;
            }
            if (WallBetween(data.Self.Position, paths[currentPath].ElementAt(0).Position))
            {
                return true;
            }
            for (int x = 0; x < paths[currentPath].Count - 1; x++)
            {
                if (WallBetween(paths[currentPath].ElementAt(x).Position, paths[currentPath].ElementAt(x + 1).Position))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ThereIsBlocker()
        {
            if (paths.Count == 0)
            {
                return true;
            }
            foreach (MoveAction p in moves)
            {
                if (data.Tiles[p.Column, p.Row].IsOccupied)
                {
                    return true;
                }
            }
            return false;
        }

        private void DrawInitialPath()
        {
            index = 0;  
            if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X, forward)) && IsWithinGameBoard(data.Self.Position.X, forward))
            {
                paths.Add(new LinkedList<Node>());
                paths[index].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - forward), new Point(data.Self.Position.X, forward)));
                index++;
            }
            if (!WallBetween(data.Self.Position, new Point(rightward, data.Self.Position.Y)) && IsWithinGameBoard(rightward, data.Self.Position.Y))
            {
                paths.Add(new LinkedList<Node>());
                paths[index].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), new Point(rightward, data.Self.Position.Y)));
                index++;
            }
            if (!WallBetween(data.Self.Position, new Point(leftward, data.Self.Position.Y)) && IsWithinGameBoard(leftward, data.Self.Position.Y))
            {
                paths.Add(new LinkedList<Node>());
                paths[index].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - data.Self.Position.Y), new Point(leftward, data.Self.Position.Y)));
                index++;
            }
            if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X, backward)) && IsWithinGameBoard(data.Self.Position.X, backward))
            {
                paths.Add(new LinkedList<Node>());
                paths[index].AddFirst(new Node(distanceFromStart, Math.Abs(goalPosition - backward), new Point(data.Self.Position.X, backward)));
                index++;
            }
        }

        private void FindLowestWeight()
        {
            int x = 0;
            for (int i = 0; i < paths.Count; i++)
            {
                for (int j = i + 1; j < paths.Count; j++)
                {
                    if (paths[i].First.Value.weight < paths[j].First.Value.weight)
                    {
                        x = i;
                    }
                }
            }
            currentPath = x;
            currentWeight = paths[x].First.Value.weight;
        }

        private bool WallBetween(Point start, Point end)
        {
            //if (start.X == end.X)
            //{
            //    int num = Math.Min(start.Y, end.Y);
            //    return data.HorizontalWall[start.X, num];
            //}
            //if (start.Y == end.Y)
            //{
            //    int num2 = Math.Min(start.X, end.X);
            //    return data.VerticalWall[num2, start.Y];
            //}
            //return true;
            return false;
        }

        protected bool IsWithinGameBoard(int column, int row)
        {
            if (0 <= column && column < data.Tiles.GetLength(0) && 0 <= row)
            {
                return row < data.Tiles.GetLength(1);
            }
            return false;
        }

        private void GetGoal()
        {
            if (data.Self.Position.Y == 0)
            {
                goalPosition = data.Tiles.GetLength(1);
                optimalDirection = 1;
            }
            else if (data.Self.Position.Y == data.Tiles.GetLength(1))
            {
                goalPosition = 0;
                optimalDirection = -1;
            }
        }
        private void Initalize()
        {
            GetGoal();
            forward = data.Self.Position.Y + optimalDirection;
            backward = data.Self.Position.Y - optimalDirection;
            rightward = data.Self.Position.X + 1;
            leftward = data.Self.Position.X - 1;
        }
    }
}