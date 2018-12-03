using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor.AI
{
    class CustomAgent : Agent
    {
        string direction;
        List<LinkedList<Node>> paths;
        List<MoveAction> movelist;
        Point optimalDirection;
        Point wrongDirection;
        Point left;
        Point right;
        int index;
        int enemyWalls;
        bool initialized = false;
        int goalPosition;

        public static void Main()
        {
            new CustomAgent().Start();
        }

        public override Action DoAction(GameData status)
        {
            if (!initialized)
            {
                Initialize(status);
                initialized = true;
            }
            if (movelist.Count > 0)
            {
                movelist.RemoveAt(movelist.Count - 1);
            }

            foreach (Player p in status.Players)
            {
                if (p != status.Self)
                {
                    if (movelist.Count < 1)
                    {
                        CreatePath(status);
                    }
                    else if (enemyWalls > p.NumberOfWalls)
                    {
                        enemyWalls = p.NumberOfWalls;
                        CreatePath(status);
                    }
                    else
                    {
                        foreach (MoveAction n in movelist)
                        {
                            if (status.Tiles[n.Column, n.Row].IsOccupied)
                            {
                                CreatePath(status);
                                break;
                            }
                        }
                    }
                }
            }

            return movelist[movelist.Count - 1];
        }

        private void CreatePath(GameData status)
        {
            movelist.Clear();
            index = 0;
            DrawInitialPaths(status);
            FindPath(status);
            paths.Clear();
        }

        private void DrawInitialPaths(GameData status)
        {
            if (IsWithinGameBoard(status.Self.Position.X, status.Self.Position.Y + optimalDirection.Y, status) && !status.Tiles[status.Self.Position.X, status.Self.Position.Y + optimalDirection.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + optimalDirection.X, status.Self.Position.Y + optimalDirection.Y), status))
                {
                    paths.Add(new LinkedList<Node>());
                    paths[index].AddFirst(new Node(1, Math.Abs(status.Self.Position.Y + optimalDirection.Y - goalPosition), new Point(status.Self.Position.X, status.Self.Position.Y + optimalDirection.Y)));
                    index++;
                }
            }
            if (IsWithinGameBoard(status.Self.Position.X, status.Self.Position.Y + wrongDirection.Y, status) && !status.Tiles[status.Self.Position.X, status.Self.Position.Y + wrongDirection.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + wrongDirection.X, status.Self.Position.Y + wrongDirection.Y), status))
                {
                    paths.Add(new LinkedList<Node>());
                    paths[index].AddFirst(new Node(1, Math.Abs(status.Self.Position.Y + wrongDirection.Y - goalPosition), new Point(status.Self.Position.X, status.Self.Position.Y + wrongDirection.Y)));
                    index++;
                }
            }
            if (IsWithinGameBoard(status.Self.Position.X + left.X, status.Self.Position.Y, status) && !status.Tiles[status.Self.Position.X + left.X, status.Self.Position.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + left.X, status.Self.Position.Y), status))
                {
                    paths.Add(new LinkedList<Node>());
                    paths[index].AddFirst(new Node(1, Math.Abs(status.Self.Position.Y - goalPosition), new Point(status.Self.Position.X + left.X, status.Self.Position.Y)));
                    index++;
                }
            }
            if (IsWithinGameBoard(status.Self.Position.X + right.X, status.Self.Position.Y, status) && !status.Tiles[status.Self.Position.X + right.X, status.Self.Position.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + right.X, status.Self.Position.Y), status))
                {
                    paths.Add(new LinkedList<Node>());
                    paths[index].AddFirst(new Node(1, Math.Abs(status.Self.Position.Y - goalPosition), new Point(status.Self.Position.X + right.X, status.Self.Position.Y)));
                    index++;
                }
            }
        }
        public int GetLowestWeight(GameData status)
        {
            int x = 0;
            for (int i = 0; i < paths.Count; i++)
            {
                if (paths[x].First.Value.weight > paths[i].First.Value.weight)
                {
                    x = i;
                }
            }
            return x;
        }

        public string GetDirection(Point directionValue)
        {
            if (directionValue.Y == optimalDirection.Y)
            {
                return "forward";
            }
            if (directionValue.Y == wrongDirection.Y)
            {
                return "back";
            }
            if (directionValue.X == -1)
            {
                return "left";
            }
            if (directionValue.X == 1)
            {
                return "right";
            }
            return "none?";
        }

        private void DeleteUnnecessary()
        {
            for(int i = 0; i < paths.Count; i++)
            {
                bool breakout = false;
                foreach(Node n in paths[i])
                {
                    foreach(Node m in paths[i])
                    {
                        if(m != paths[i].First.Value)
                        {
                            if(n.position == m.position)
                            {
                                paths.RemoveAt(i);
                                breakout = true;
                                index--;
                            }
                        }
                        if (breakout)
                            break;
                    }
                    if (breakout)
                        break;
                }
            }
        }

        public void FindPath(GameData status)
        {
          //  DeleteUnnecessary();
            int currentPath = GetLowestWeight(status);
            Node toSkip = new Node(0, 0, new Point());
            if (paths[currentPath].Count > 1)
            {
                Point directionValue = new Point(paths[currentPath].First.Value.position.X - paths[currentPath].First.Next.Value.position.X, paths[currentPath].First.Value.position.Y - paths[currentPath].First.Next.Value.position.Y);
                direction = GetDirection(directionValue);
            }
            else
            {
                direction = "none";
            }

            bool firstStepAdded = false;
            if (direction != "back" && IsWithinGameBoard(paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + optimalDirection.Y, status))
            {
                if (!WallBetween(paths[currentPath].First.Value.position, new Point(paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + optimalDirection.Y), status) && !status.Tiles[paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + optimalDirection.Y].IsOccupied)
                {
                    toSkip = new Node(paths[currentPath].First.Value.stepsFromStart + 1, Math.Abs(paths[currentPath].First.Value.position.Y + optimalDirection.Y - goalPosition), new Point(paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + optimalDirection.Y));
                    paths[currentPath].AddFirst(toSkip);
                    firstStepAdded = true;
                }
            }
            if (direction != "right" && IsWithinGameBoard(paths[currentPath].First.Value.position.X + left.X, paths[currentPath].First.Value.position.Y, status))
            {
                if (!WallBetween(paths[currentPath].First.Value.position, new Point(paths[currentPath].First.Value.position.X + left.X, paths[currentPath].First.Value.position.Y), status) && !status.Tiles[paths[currentPath].First.Value.position.X + left.X, paths[currentPath].First.Value.position.Y].IsOccupied)
                {
                    if (!firstStepAdded)
                    {
                        toSkip = new Node(paths[currentPath].First.Value.stepsFromStart + 1, Math.Abs(paths[currentPath].First.Value.position.Y - goalPosition), new Point(paths[currentPath].First.Value.position.X + left.X, paths[currentPath].First.Value.position.Y));
                        paths[currentPath].AddFirst(toSkip);
                        firstStepAdded = true;
                    }
                    else
                    {
                        paths.Add(new LinkedList<Node>());
                        foreach (Node n in paths[currentPath])
                        {
                            paths[index].AddLast(n);
                        }
                        paths[index].AddFirst(new Node(paths[currentPath].First.Value.stepsFromStart + 1, Math.Abs(paths[currentPath].First.Value.position.Y - goalPosition), new Point(paths[currentPath].First.Value.position.X + left.X, paths[currentPath].First.Value.position.Y)));
                        index++;
                    }
                }
            }
            if (direction != "left" && IsWithinGameBoard(paths[currentPath].First.Value.position.X + right.X, paths[currentPath].First.Value.position.Y, status))
            {
                if (!WallBetween(paths[currentPath].First.Value.position, new Point(paths[currentPath].First.Value.position.X + right.X, paths[currentPath].First.Value.position.Y), status) && !status.Tiles[paths[currentPath].First.Value.position.X + right.X, paths[currentPath].First.Value.position.Y].IsOccupied)
                {
                    if (!firstStepAdded)
                    {
                        toSkip = new Node(paths[currentPath].First.Value.stepsFromStart + 1, Math.Abs(paths[currentPath].First.Value.position.Y - goalPosition), new Point(paths[currentPath].First.Value.position.X + right.X, paths[currentPath].First.Value.position.Y));
                        paths[currentPath].AddFirst(toSkip);
                        firstStepAdded = true;
                    }
                    else
                    {
                        paths.Add(new LinkedList<Node>());
                        foreach (Node n in paths[currentPath])
                        {
                            paths[index].AddLast(n);
                        }
                        paths[index].AddFirst(new Node(paths[currentPath].First.Value.stepsFromStart + 1, Math.Abs(paths[currentPath].First.Value.position.Y - goalPosition), new Point(paths[currentPath].First.Value.position.X + right.X, paths[currentPath].First.Value.position.Y)));
                        index++;
                    }
                }
            }
            if (direction != "forward" && IsWithinGameBoard(paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + wrongDirection.Y, status))
            {
                if (!WallBetween(paths[currentPath].First.Value.position, new Point(paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + wrongDirection.Y), status) && !status.Tiles[paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + wrongDirection.Y].IsOccupied)
                {
                    if (!firstStepAdded)
                    {
                        toSkip = new Node(paths[currentPath].First.Value.stepsFromStart + 1, Math.Abs(paths[currentPath].First.Value.position.Y + wrongDirection.Y - goalPosition), new Point(paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + wrongDirection.Y));
                        paths[currentPath].AddFirst(toSkip);
                        firstStepAdded = true;
                    }
                    else
                    {
                        paths.Add(new LinkedList<Node>());
                        foreach (Node n in paths[currentPath])
                        {
                            paths[index].AddLast(n);
                        }
                        paths[index].AddFirst(new Node(paths[currentPath].First.Value.stepsFromStart + 1, Math.Abs(paths[currentPath].First.Value.position.Y + wrongDirection.Y - goalPosition), new Point(paths[currentPath].First.Value.position.X, paths[currentPath].First.Value.position.Y + wrongDirection.Y)));
                        index++;
                    }
                }
            }
            if(paths.Count > 200)
            {
                JustWalk(status);
            }
            else if (paths[currentPath].First.Value.position.Y == goalPosition)
            {
                foreach (Node n in paths[currentPath])
                {
                    if (n != toSkip)
                    {
                        movelist.Add(new MoveAction(n.position.X,n.position.Y));
                    }
                }
            }
            else
            {
                if (!firstStepAdded)
                {
                    paths.RemoveAt(currentPath);
                    index--;
                }
                FindPath(status);
            }
        }

        private void JustWalk(GameData status)
        {
            bool ignoreRest = false;
            if (IsWithinGameBoard(status.Self.Position.X + right.X, status.Self.Position.Y, status) && !status.Tiles[status.Self.Position.X + right.X, status.Self.Position.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + right.X, status.Self.Position.Y), status))
                {
                    movelist.Add(new MoveAction(status.Self.Position.X + right.X, status.Self.Position.Y));
                    ignoreRest = true;
                }
            }
            if (!ignoreRest && IsWithinGameBoard(status.Self.Position.X, status.Self.Position.Y + wrongDirection.Y, status) && !status.Tiles[status.Self.Position.X, status.Self.Position.Y + wrongDirection.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + wrongDirection.X, status.Self.Position.Y + wrongDirection.Y), status))
                {
                    movelist.Add(new MoveAction(status.Self.Position.X, status.Self.Position.Y + wrongDirection.Y));
                    ignoreRest = true;
                }
            }
            if (!ignoreRest && IsWithinGameBoard(status.Self.Position.X + left.X, status.Self.Position.Y, status) && !status.Tiles[status.Self.Position.X + left.X, status.Self.Position.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + left.X, status.Self.Position.Y), status))
                {
                    movelist.Add(new MoveAction(status.Self.Position.X + left.X, status.Self.Position.Y));
                    ignoreRest = true;
                }
            }
            if (!ignoreRest && IsWithinGameBoard(status.Self.Position.X, status.Self.Position.Y + optimalDirection.Y, status) && !status.Tiles[status.Self.Position.X, status.Self.Position.Y + optimalDirection.Y].IsOccupied)
            {
                if (!WallBetween(status.Self.Position, new Point(status.Self.Position.X + optimalDirection.X, status.Self.Position.Y + optimalDirection.Y), status))
                {
                    movelist.Add(new MoveAction(status.Self.Position.X, status.Self.Position.Y + optimalDirection.Y));
                }
            }
        }

        private bool WallBetween(Point start, Point end, GameData status)
        {
            if (start.X == end.X)
            {
                int num = Math.Min(start.Y, end.Y);
                return status.HorizontalWall[start.X, num];
            }
            if (start.Y == end.Y)
            {
                int num2 = Math.Min(start.X, end.X);
                return status.VerticalWall[num2, start.Y];
            }
            return true;
        }

        protected bool IsWithinGameBoard(int column, int row, GameData status)
        {
            if (0 <= column && column < status.Tiles.GetLength(0) && 0 <= row)
            {
                return row < status.Tiles.GetLength(1);
            }
            return false;
        }

        private void Initialize(GameData status)
        {
            paths = new List<LinkedList<Node>>();
            movelist = new List<MoveAction>();
            if (status.Self.Position.Y == 0)
            {
                goalPosition = status.Tiles.GetLength(1)-1;
                optimalDirection = new Point(0, 1);
                wrongDirection = new Point(0, -1);
            }
            else
            {
                goalPosition = 0;
                optimalDirection = new Point(0, -1);
                wrongDirection = new Point(0, 1);
            }
            left = new Point(-1, 0);
            right = new Point(1, 0);
            enemyWalls = 100;
        }
    }
}