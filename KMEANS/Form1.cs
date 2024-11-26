using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace KMEANS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Centroids = new List<Centroid>();
        }

        /// <summary>
        /// Event handler for the ChoosePicBtn button click event. Opens a file dialog to allow the user 
        /// to select an image file, displays the selected image in the PictureIn PictureBox, 
        /// and initiates k-means clustering to reduce the image colors.
        /// </summary>
        /// <param name="sender">The source of the event, typically the ChoosePicBtn button.</param>
        /// <param name="e">Event arguments for the button click event.</param>
        private void ChoosePicBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    Image image = Image.FromFile(filePath);
                    PictureIn.Image = image;
                    PictureIn.SizeMode = PictureBoxSizeMode.StretchImage;

                    // Call the ReducePictureKmeans function synchronously
                    ReducePictureKmeans(filePath);
                }
            }
        }


        /// <summary>
        /// Reduces the colors in an image to the specified number of clusters using k-means clustering.
        /// This function loads the image, initializes centroids, and iteratively clusters and updates
        /// pixel values to match the nearest centroid color until convergence or maximum iterations.
        /// The processed image is displayed in the PictureOut PictureBox.
        /// </summary>
        /// <param name="filepath">The file path of the image to be processed.</param>
        private void ReducePictureKmeans(string filepath)
        {
            // Load the image and convert it to a 3D byte array
            byte[,,] bixel = _42Entwickler.ImageLib.ArrayImage.ReadAs3DArray(filepath);

            // Initialize random centroids
            Random random = new Random();
            for (int i = 0; i < NumberOfClusters; i++)
            {
                Centroid centroid = new Centroid();
                Centroids.Add(centroid);
            }

            // Main k-means loop
            bool hasChanged = true;
            while (hasChanged)
            {
                ClusterPixels(bixel);
                MoveCentroids(bixel);
                hasChanged = CheckChanged();
            }

            // Update the pixels with the centroid colors
            UpdatePixels(bixel);

            // Create the image from the processed bixel array
            Image image = _42Entwickler.ImageLib.ArrayImage.CreateImage(bixel);

            // Display the image in PictureOut directly
            if (PictureOut.Image != null)
            {
                PictureOut.Image.Dispose();
            }

            PictureOut.Image = image;
            PictureOut.SizeMode = PictureBoxSizeMode.StretchImage;
        }


        /// <summary>
        /// Extracts the color of each pixel from the given bitmap and returns them as a list of Color objects.
        /// </summary>
        /// <param name="bitmap">The bitmap from which to extract pixel colors.</param>
        /// <returns>A list of Color objects representing the colors of each pixel in the bitmap.</returns>
        private List<Color> GetPixels(Bitmap bitmap)
        {
            List<Color> pixels = new List<Color>();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    pixels.Add(pixelColor);
                }
            }
            return pixels;
        }

        /// <summary>
        /// Assigns each pixel in the image to the nearest centroid based on Euclidean distance,
        /// grouping pixels to form clusters around each centroid. This function iterates over
        /// all pixels in the image and calculates their distance to each centroid, clearing
        /// previous clusters before starting a new assignment.
        /// </summary>
        /// <param name="bixel">A 3D byte array representing the RGB values of each pixel in the image.</param>
        private void ClusterPixels(byte[,,] bixel)
        {
            // Clear clusters before re-clustering
            foreach (var centroid in Centroids)
            {
                centroid.Cluster.Clear();
            }

            // Proceed with clustering
            for (int i = 0; i < bixel.GetLength(0); i++)
            {
                for (int j = 0; j < bixel.GetLength(1); j++)
                {
                    CalculateMinimalEuclideDistance(bixel, i, j);
                }
            }
        }

        /// <summary>
        /// Calculates the Euclidean distance between a given pixel and a centroid in RGB color space.
        /// This distance is used to determine the closest centroid for each pixel during clustering.
        /// </summary>
        /// <param name="bixels">A 3D byte array representing the RGB values of each pixel in the image.</param>
        /// <param name="i">The row index of the pixel in the image array.</param>
        /// <param name="j">The column index of the pixel in the image array.</param>
        /// <param name="centroid">The centroid to which the distance is being calculated.</param>
        /// <returns>The Euclidean distance between the specified pixel and the given centroid.</returns>
        private double CalcEuclide(byte[,,] bixels, int i, int j, Centroid centroid)
        {
            int R1 = bixels[i, j, 0];
            int G1 = bixels[i, j, 1];
            int B1 = bixels[i, j, 2];

            int R2 = centroid.R;
            int G2 = centroid.G;
            int B2 = centroid.B;

            double distance = Math.Sqrt(
                Math.Pow(R2 - R1, 2) +
                Math.Pow(G2 - G1, 2) +
                Math.Pow(B2 - B1, 2)
            );
            return distance;
        }

        /// <summary>
        /// Finds the closest centroid to a specific pixel by calculating the Euclidean distance 
        /// between the pixel and each centroid, and assigns the pixel to the nearest centroid’s cluster.
        /// </summary>
        /// <param name="bixels">A 3D byte array representing the RGB values of each pixel in the image.</param>
        /// <param name="i">The row index of the pixel in the image array.</param>
        /// <param name="j">The column index of the pixel in the image array.</param>
        private void CalculateMinimalEuclideDistance(byte[,,] bixels, int i, int j)
        {
            double minDistance = double.MaxValue;
            Centroid closestCentroid = null;
            foreach (Centroid centroid in Centroids)
            {
                double tempResult = CalcEuclide(bixels, i, j, centroid);
                if (tempResult < minDistance)
                {
                    minDistance = tempResult;
                    closestCentroid = centroid;
                }
            }
            if (closestCentroid != null)
            {
                closestCentroid.Cluster.Add(new int[] { i, j });
            }
        }

        /// <summary>
        /// Determines the closest centroid to a given pixel by calculating the Euclidean distance 
        /// between the pixel and each centroid. Assigns the pixel to the nearest centroid’s cluster.
        /// </summary>
        /// <param name="bixels">A 3D byte array representing the RGB values of each pixel in the image.</param>
        /// <param name="i">The row index of the pixel in the image array.</param>
        /// <param name="j">The column index of the pixel in the image array.</param>
        private void UpdatePixels(byte[,,] bixels)
        {
            foreach (var centroid in Centroids)
            {
                foreach (var position in centroid.Cluster)
                {
                    int i = position[0];
                    int j = position[1];
                    bixels[i, j, 0] = (byte)centroid.R;
                    bixels[i, j, 1] = (byte)centroid.G;
                    bixels[i, j, 2] = (byte)centroid.B;
                }
            }
        }

        /// <summary>
        /// Checks if a given pixel, specified by its coordinates, is currently assigned to any centroid's cluster.
        /// This method iterates through each centroid's cluster to determine if the pixel is already assigned.
        /// </summary>
        /// <param name="i">The row index of the pixel in the image array.</param>
        /// <param name="j">The column index of the pixel in the image array.</param>
        /// <returns>True if the pixel is found in any cluster; otherwise, false.</returns>
        private bool IsPixelInAnyCluster(int i, int j)
        {
            foreach (var centroid in Centroids)
            {
                if (centroid.Cluster.Any(pixel => pixel[0] == i && pixel[1] == j))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adjusts the position of each centroid to the mean location of all pixels assigned to its cluster.
        /// This method calculates the new RGB values for each centroid based on the average RGB values of 
        /// the pixels in its cluster, moving the centroid to the new center of gravity.
        /// </summary>
        /// <param name="bixels">A 3D byte array representing the RGB values of each pixel in the image.</param>
        private void MoveCentroids(byte[,,] bixels)
        {
            foreach (var centroid in Centroids)
            {
                MoveCentroid(centroid, bixels);
            }
        }

        /// <summary>
        /// Moves a single centroid to the mean RGB position of all pixels currently assigned to its cluster.
        /// This method calculates the average RGB values of the pixels in the centroid’s cluster and updates 
        /// the centroid’s RGB values to these averages, effectively relocating the centroid.
        /// </summary>
        /// <param name="centroid">The centroid to be moved to the new center of its assigned cluster.</param>
        /// <param name="pixels">A 3D byte array representing the RGB values of each pixel in the image.</param>
        private void MoveCentroid(Centroid centroid, byte[,,] pixels)
        {
            int n = centroid.Cluster.Count;

            if (n == 0)
            {
                // No pixels assigned to this centroid.
                // Optionally, reinitialize the centroid to a random position.
                return;
            }

            // Initialize sums for RGB values
            long sumR = 0, sumG = 0, sumB = 0;

            // Sum up the R, G, B values of all pixels in the cluster
            foreach (var position in centroid.Cluster)
            {
                int i = position[0];
                int j = position[1];

                sumR += pixels[i, j, 0];
                sumG += pixels[i, j, 1];
                sumB += pixels[i, j, 2];
            }

            // Calculate the mean values
            int meanR = (int)(sumR / n);
            int meanG = (int)(sumG / n);
            int meanB = (int)(sumB / n);

            // Assign the mean values back to the centroid
            centroid.R = meanR;
            centroid.G = meanG;
            centroid.B = meanB;
        }
        private const double Threshold = 1.0;

        /// <summary>
        /// Checks if any centroids have moved significantly since the last iteration, based on a predefined threshold.
        /// This method compares the current positions of each centroid with their previous positions to determine 
        /// if the k-means algorithm should continue. It also tracks the number of iterations to enforce a maximum limit.
        /// </summary>
        /// <returns>True if any centroids have moved beyond the threshold distance; otherwise, false, indicating convergence.</returns>
        private bool CheckChanged()
        {
            bool hasChanged = false;

            if (previousCentroidPositions.Count == 0)
            {
                foreach (var centroid in Centroids)
                {
                    previousCentroidPositions.Add((centroid.R, centroid.G, centroid.B));
                }
                return true;
            }

            for (int i = 0; i < Centroids.Count; i++)
            {
                var centroid = Centroids[i];
                var previousPosition = previousCentroidPositions[i];

                double distance = Math.Sqrt(
                    Math.Pow(centroid.R - previousPosition.R, 2) +
                    Math.Pow(centroid.G - previousPosition.G, 2) +
                    Math.Pow(centroid.B - previousPosition.B, 2)
                );

                if (distance > Threshold)
                {
                    hasChanged = true;
                }

                previousCentroidPositions[i] = (centroid.R, centroid.G, centroid.B);
            }

            currentIteration++;
            if (currentIteration >= NumberOfMaxIterations)
            {
                return false;
            }

            return hasChanged;
        }

        /// <summary>
        /// Event handler for when the PictureIn PictureBox is clicked. Opens a file dialog to allow the user
        /// to select an image, then displays the selected image in PictureIn and processes it with k-means 
        /// clustering to reduce the number of colors.
        /// </summary>
        /// <param name="sender">The source of the event, typically the PictureIn PictureBox.</param>
        /// <param name="e">Event arguments for the click event.</param>
        private void PictureIn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    Image image = Image.FromFile(filePath);
                    PictureIn.Image = image;
                    PictureIn.SizeMode = PictureBoxSizeMode.StretchImage;

                    // Call the ReducePictureKmeans function synchronously
                    ReducePictureKmeans(filePath);
                }
            }
        }

        private int NumberOfClusters { get; set; } = 10;
        private int NumberOfMaxIterations { get; set; } = 100;
        private int currentIteration = 0; // Tracks the current iteration
        private List<(int R, int G, int B)> previousCentroidPositions = new List<(int, int, int)>();

        private List<Centroid> Centroids { get; set; }

    }
}
