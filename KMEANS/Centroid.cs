using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMEANS
{
    internal class Centroid
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public List<int[]> Cluster { get; set; }

        public Centroid(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Centroid() 
        { 
            GetRandomCentroids();
            Cluster = new List<int[]>();
        }

        public void GetRandomCentroids()
        {
            Random random = new Random();

            // Randomly generate RGB values
            R = random.Next(256);
            G = random.Next(256);
            B = random.Next(256);
        }
    }
}
