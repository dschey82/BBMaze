using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maze.BL;

namespace Tests.Maze.BL
{
    [TestClass]
    public class TestsAMaze
    {
        [TestMethod]
        public void TestGetCenterPixel()
        {
            Bitmap test = new Bitmap(40, 40);
            for (int y = 0; y < test.Height; y++)
            {
                for (int x = 0; x < test.Width; x++)
                {
                    test.SetPixel(x, y, Color.FromArgb(255,255,0,0));
                }
            }

            AMaze testMaze = new AMaze(test);

            Assert.AreEqual(new Point(19, 19), testMaze.StartLocation);
        }

        [TestMethod]
        public void TestGetBitmapFromFile()
        {
            string fileName = @"C:\_temp\maze1.png";
            Bitmap testBmp = new Bitmap(fileName);

            AMaze testMaze = new AMaze(fileName);

            Assert.AreEqual(testBmp.GetPixel(5,5), testMaze.GetBitmap().GetPixel(5,5));
        }

        [TestMethod]
        public void TestIsPixelInFinish()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreEqual(testMaze.IsPixelInFinish(17, 417), true);
        }

        [TestMethod]
        public void TestIsPixelInFinishNegative()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreNotEqual(testMaze.IsPixelInFinish(325, 325), true);
        }

        [TestMethod]
        public void TestIsPixelInWall()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreEqual(testMaze.IsPixelInWall(88, 132), true);
        }

        [TestMethod]
        public void TestIsPixelInWallNegative()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreNotEqual(testMaze.IsPixelInWall(85, 132), true);
        }

        [TestMethod]
        public void TestMazeConstructorStartLocation()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreEqual(testMaze.StartLocation, new Point(243,418));
        }

        [TestMethod]
        public void TestMazeConstructorStartLocationNegative()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreNotEqual(testMaze.StartLocation, new Point(5,5));
        }

        [TestMethod]
        public void TestMazeConstructorEndLocation()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreEqual(testMaze.EndLocation, new Point(22,417));
        }

        [TestMethod]
        public void TestMazeConstructorEndLocationNegative()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            Assert.AreNotEqual(testMaze.EndLocation, new Point(5,5));
        }

        /// <summary>
        /// Trivial test for now, but useful for future maze solution implementations
        /// </summary>
        [TestMethod]
        public void TestFindNearestWall()
        {
            string fileName = @"C:\_temp\maze1.png";

            AMaze testMaze = new AMaze(fileName);

            RightHandMazeSolver rhms = new RightHandMazeSolver(testMaze);

            testMaze.FindSolution();

            Assert.AreEqual(testMaze.GetSolutionPoints()[0], rhms.GetSolution()[0]);
        }
    }
}
