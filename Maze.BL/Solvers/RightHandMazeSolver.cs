using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.BL
{
    // clockwise rotational order
    enum Direction
    {
        PlusX,       
        PlusY,
        MinusX,
        MinusY,
    }

    public class RightHandMazeSolver : IMazeSolver
    {
        AMaze m_Maze;
        Point[] m_SolutionPoints;
        Bitmap m_SolutionBitmap;

        Dictionary<Direction, Point> CheckAhead = new Dictionary<Direction, Point>
        {
            { Direction.PlusX,  new Point( 1, 0) },
            { Direction.MinusX, new Point(-1, 0) },
            { Direction.PlusY,  new Point( 0, 1) },
            { Direction.MinusY, new Point( 0,-1) },
        };
        
        Dictionary<Direction, Point> CheckRight = new Dictionary<Direction, Point>
        {
            { Direction.PlusX,  new Point( 0, 1) },
            { Direction.MinusX, new Point( 0,-1) },
            { Direction.PlusY,  new Point(-1, 0) },
            { Direction.MinusY, new Point( 1, 0) },
        };

        public RightHandMazeSolver(AMaze pMaze)
        {
            m_Maze = pMaze;
         }

        public Point[] GetSolution()
        {
            SolveMaze();
            return m_SolutionPoints;
        }

        public Bitmap GetSolvedBitmap(Color pSolutionColor)
        {
            DrawSolution(pSolutionColor);
            return m_SolutionBitmap;
        }

        private void DrawSolution(Color pSolutionColor)
        {
            Bitmap workingBmp = m_Maze.GetBitmap();
            foreach (Point p in m_SolutionPoints)
            {
                workingBmp.SetPixel(p.X, p.Y, pSolutionColor);
            }
            m_SolutionBitmap = workingBmp;
        }

        private void SolveMaze()
        {
            Point CurrentLocation = m_Maze.StartLocation;
            Direction CurrentDirection = Direction.PlusX;
            
            FindNearestWall(ref CurrentLocation, ref CurrentDirection);
            m_SolutionPoints = WalkTheMaze(CurrentLocation, CurrentDirection);            
        }

        // Walk the right-hand wall from CurrentLocation starting in CurrentDirection
        // Stop when reaching a pixel with the finish color
        private Point[] WalkTheMaze(Point CurrentLocation, Direction CurrentDirection)
        {
            List<Point> SolutionPoints = new List<Point>();
            
            while (!m_Maze.IsPixelInFinish(CurrentLocation.X, CurrentLocation.Y))
            {
                // check if right-hand wall is still there, if not, turn right and move one pixel that direction and add to solution set
                if (!m_Maze.IsPixelInWall(CurrentLocation.X + CheckRight[CurrentDirection].X, CurrentLocation.Y + CheckRight[CurrentDirection].Y))
                {
                    CurrentDirection = (Direction)(((int)CurrentDirection + 1) % 4);    // rotate 90 degrees cw
                    CurrentLocation.X += CheckAhead[CurrentDirection].X;
                    CurrentLocation.Y += CheckAhead[CurrentDirection].Y;
                    SolutionPoints.Add(CurrentLocation);
                }
                // else check if we can continue moving in current direction, if not, turn left & don't move until next iteration when we check wall in that direction
                else if (m_Maze.IsPixelInWall(CurrentLocation.X + CheckAhead[CurrentDirection].X, CurrentLocation.Y + CheckAhead[CurrentDirection].Y))
                {
                    CurrentDirection = (Direction)(((int)CurrentDirection + 3) % 4);    // rotate 90 degrees ccw (+3 = -1 for 90 deg rotations)
                }
                // else move in current direction and add to solution set
                else
                {
                    CurrentLocation.X += CheckAhead[CurrentDirection].X;
                    CurrentLocation.Y += CheckAhead[CurrentDirection].Y;
                    SolutionPoints.Add(CurrentLocation);
                }
            }
            return SolutionPoints.ToArray<Point>();

            // replaced switch with dictionaries and enum math
            // saved ~2 seconds on processing maze2.png test case
            #region ReplacedSwitch
            //switch (CurrentDirection)
            //{
            //    case Direction.PlusX:
            //        if (!m_Maze.IsPixelInWall(CurrentLocation.X, CurrentLocation.Y + 1))
            //        {
            //            CurrentDirection = Direction.PlusY;
            //            CurrentLocation.Y++;
            //            SolutionPoints.Add(CurrentLocation);
            //        }
            //        else if (m_Maze.IsPixelInWall(CurrentLocation.X + 1, CurrentLocation.Y))
            //        {
            //            CurrentDirection = Direction.MinusY;
            //        }
            //        else
            //        {
            //            CurrentLocation.X++;
            //            SolutionPoints.Add(CurrentLocation);
            //        }
            //        break;
            //    case Direction.MinusX:
            //        if (!m_Maze.IsPixelInWall(CurrentLocation.X, CurrentLocation.Y - 1))
            //        {
            //            CurrentDirection = Direction.MinusY;
            //            CurrentLocation.Y--;
            //            SolutionPoints.Add(CurrentLocation);
            //        }
            //        else if (m_Maze.IsPixelInWall(CurrentLocation.X - 1, CurrentLocation.Y))
            //        {
            //            CurrentDirection = Direction.PlusY;
            //        }
            //        else
            //        {
            //            CurrentLocation.X--;
            //            SolutionPoints.Add(CurrentLocation);
            //        }
            //        break;
            //    case Direction.PlusY:
            //        if (!m_Maze.IsPixelInWall(CurrentLocation.X-1, CurrentLocation.Y))
            //        {
            //            CurrentDirection = Direction.MinusX;
            //            CurrentLocation.X--;
            //            SolutionPoints.Add(CurrentLocation);
            //        }
            //        else if (m_Maze.IsPixelInWall(CurrentLocation.X, CurrentLocation.Y+1))
            //        {
            //            CurrentDirection = Direction.PlusX;
            //        }
            //        else
            //        {
            //            CurrentLocation.Y++;
            //            SolutionPoints.Add(CurrentLocation);
            //        }

            //        break;
            //    case Direction.MinusY:
            //        if (!m_Maze.IsPixelInWall(CurrentLocation.X + 1, CurrentLocation.Y))
            //        {
            //            CurrentDirection = Direction.PlusX;
            //            CurrentLocation.X++;
            //            SolutionPoints.Add(CurrentLocation);
            //        }
            //        else if (m_Maze.IsPixelInWall(CurrentLocation.X, CurrentLocation.Y - 1))
            //        {
            //            CurrentDirection = Direction.MinusX;
            //        }
            //        else
            //        {
            //            CurrentLocation.Y--;
            //            SolutionPoints.Add(CurrentLocation);
            //        }                        
            //        break;
            //    default:
            //        break;
            //}       
            #endregion
        }

        private void FindNearestWall(ref Point CurrentLocation, ref Direction CurrentDirection)
        {
            // find nearest wall and start direction
            int distance = 1;
            while (distance < Math.Max(m_Maze.Height, m_Maze.Width))
            {
                if (m_Maze.IsPixelInWall(CurrentLocation.X + distance, CurrentLocation.Y))
                {
                    CurrentLocation.X = CurrentLocation.X + distance - 1;
                    CurrentDirection = Direction.MinusY;
                    break;
                }
                else if (m_Maze.IsPixelInWall(CurrentLocation.X - distance, CurrentLocation.Y))
                {
                    CurrentLocation.X = CurrentLocation.X - distance + 1;
                    CurrentDirection = Direction.PlusY;
                    break;
                }
                else if (m_Maze.IsPixelInWall(CurrentLocation.X, CurrentLocation.Y + distance))
                {
                    CurrentLocation.Y = CurrentLocation.Y + distance - 1;
                    CurrentDirection = Direction.PlusX;
                    break;
                }
                else if (m_Maze.IsPixelInWall(CurrentLocation.X, CurrentLocation.Y - distance))
                {
                    CurrentLocation.Y = CurrentLocation.Y - distance + 1;
                    CurrentDirection = Direction.MinusX;
                    break;
                }
                else
                {
                    distance++;
                }
            }
        }
    }
}
