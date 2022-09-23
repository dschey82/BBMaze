using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maze.BL;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilename = @"c:\_temp\maze2.png"; //args[0];
            string destinationFilename = @"C:\_temp\maze2new.png"; // args[1];            

            AMaze Maze = new AMaze(inputFilename);
            Maze.FindSolution();

            if (Maze.WriteSolutionToFile(destinationFilename))
            {
                Console.WriteLine("Solution written to file named {0}.", destinationFilename);
            }
            else
            {
                Console.WriteLine("Unable to write to file {0}.", destinationFilename);
            }
            Console.WriteLine("Press any key to close.");
            Console.ReadLine();
        }
    }
}
