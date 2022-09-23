using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.BL
{
    public class AMaze
    {
        public int Size { get; }
        public int Height { get; }
        public int Width { get; }
        public Point StartLocation { get; set; }
        public Point EndLocation { get; set; } 

        Point[] m_SolutionPoints;
        FileInfo m_InputFile;
        Bitmap m_Bitmap;
        Bitmap m_SolutionBitmap;

        // Static Values for this test
        // TODO: detect colors from file
        // JPEG images can distort colors, so these values won't necessarily work
        Color _startColor = Color.FromArgb(255, 255, 0, 0);
        Color _endColor = Color.FromArgb(255, 0, 0, 255);
        Color _whiteColor = Color.FromArgb(255, 255, 255, 255);
        Color _wallColor = Color.FromArgb(255, 0, 0, 0);
        Color _solutionColor = Color.FromArgb(255, 0, 255, 0);        

        /// <summary>
        /// Test Constructor that can take in Bitmaps
        /// </summary>
        /// <param name="bmp"></param>
        public AMaze(Bitmap bmp)
        {
            m_Bitmap = bmp;
            Height = m_Bitmap.Height;
            Width = m_Bitmap.Width;

            PreProcessColors();
            SetImageBordersToBlack();
            // Find Start and End locations
            StartLocation = GetCenterPixel(_startColor);
            EndLocation = GetCenterPixel(_endColor);
        }

        public AMaze(string inputFilename)
        {
            // Import Bitmap File
            m_InputFile = new FileInfo(inputFilename);
            GetBitmapFromFile();
            Height = m_Bitmap.Height;
            Width = m_Bitmap.Width;

            PreProcessColors();
            SetImageBordersToBlack();
            // Find Start and End locations
            StartLocation = GetCenterPixel(_startColor);
            EndLocation = GetCenterPixel(_endColor);
        }

        public void FindSolution()
        {
            // Use the Right Hand Maze Walk Solution
            // TODO: Add logic to determine which maze algorithm is best (and implement them)
            IMazeSolver ims = new RightHandMazeSolver(this);
            m_SolutionPoints = ims.GetSolution();
            m_SolutionBitmap = ims.GetSolvedBitmap(_solutionColor);
        }

        public bool WriteSolutionToFile(string pDestFile)
        {
            try
            {
                m_SolutionBitmap.Save(pDestFile);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public Point[] GetSolutionPoints()
        {
            return m_SolutionPoints;
        }
        public Bitmap GetBitmap()
        {
            return m_Bitmap;
        }
        public bool IsPixelInFinish(int x, int y)
        {
            return m_Bitmap.GetPixel(x, y) == _endColor;
        }

        public bool IsPixelInWall(int x, int y)
        {
            return m_Bitmap.GetPixel(x, y) == _wallColor;
        }

        // Pre-processing required to clean up JPEG conversions of maze images
        // Attempts to cleanup pixels so that they are recognizable by solver
        // as Wall/Start/Finish locations. May still be issues if the finish zone 
        // bleeds across an interior wall
        private void PreProcessColors()
        {
            for (int y = 0; y < m_Bitmap.Height; y++)
            {
                for (int x = 0; x < m_Bitmap.Width; x++)
                {
                    Color CurrentColor = m_Bitmap.GetPixel(x, y);
                    // If gray scale, convert to white or black
                    if (CurrentColor.R - CurrentColor.B <= 50 && CurrentColor.B - CurrentColor.G <= 50)
                    {
                        if (CurrentColor.R > 128) m_Bitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                        else m_Bitmap.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                    }
                    // If Reddish/Blueish color, convert to Red/Blue
                    {
                        if (CurrentColor.R - CurrentColor.B > 50 && CurrentColor.R - CurrentColor.G > 50) m_Bitmap.SetPixel(x, y, Color.FromArgb(255, 255, 0, 0));
                        if (CurrentColor.B - CurrentColor.R > 50 && CurrentColor.B - CurrentColor.G > 50) m_Bitmap.SetPixel(x, y, Color.FromArgb(255, 0, 0, 255));
                    }
                }
            }            
        }

        /// <summary>
        /// set borders of image to full black color
        /// </summary>
        private void SetImageBordersToBlack()
        {
            for (int y = 0; y < m_Bitmap.Height; y++)
            {
                m_Bitmap.SetPixel(0, y, _wallColor);
                m_Bitmap.SetPixel(Width - 1, y, _wallColor);
            }
            for (int x = 0; x < m_Bitmap.Width; x++)
            {
                m_Bitmap.SetPixel(x, 0, _wallColor);
                m_Bitmap.SetPixel(x, Height - 1, _wallColor);
            }
        }

        private void GetBitmapFromFile()
        {
            if (m_InputFile.Exists)
            {
                try
                {
                    m_Bitmap = new Bitmap(m_InputFile.FullName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unknown error occurred.");
                    Console.WriteLine(e.StackTrace);
                    throw e;
                }
            }
            else
            {
                Console.WriteLine("Input file can not be found. Please verify the file exists and try again.");
            }            
        }

        // Finds the averaged (x,y) location of all pixels with target color
        // If color is not found in image, return a Point(-1,-1)
        private Point GetCenterPixel(Color pTargetColor)
        {
            int xSum = 0;
            int ySum = 0;
            int pixelCount = 0;
            for (int y = 0; y < m_Bitmap.Height; y++)
            {
                for (int x = 0; x < m_Bitmap.Width; x++)
                {
                    if (m_Bitmap.GetPixel(x,y) == pTargetColor)
                    {
                        xSum += x;
                        ySum += y;
                        pixelCount++;
                    }
                }
            }
            if (pixelCount > 0) return new Point(xSum / pixelCount, ySum / pixelCount);
            else
            {
                Console.WriteLine("Could not find {0} in image.", pTargetColor.Name);
                return new Point(-1, -1);
            }
        }
    }
}
