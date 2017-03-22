using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

namespace OPENCVsharptest
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] cornerarray = new int[5000, 2];
            int[,] linearray = new int[5000, 4];
            int f = 0;
            int l = 0;
            /* using (Mat src = new Mat("test.jpg", ImreadModes.GrayScale))
             {
                 using (Window window = new Window("Load & Display", src))
                 {
                     Window.WaitKey();
                 }
             }
             drawcircle();
             getcorners(ref cornerarray, ref f);*/
            getlines(ref linearray, ref l);
            /* for(int z = 0; z<1000; z++)
             {
                 Console.Write(cornerarray[z,0]);
                 Console.Write(cornerarray[z,1]);
                 Console.WriteLine("");
             }    */
            for (int z = 0; z < 1000; z++)
            {
                Console.Write(linearray[z, 0]);
                Console.Write(" ");
                Console.Write(linearray[z, 1]);
                Console.Write(" ----> ");
                Console.Write(linearray[z, 2]);
                Console.Write(" ");
                Console.Write(linearray[z, 3]);
                Console.WriteLine("");
            }
            Console.ReadLine();
        }
//____________________________________________________________________________________
        static Mat getEdges(Mat image, int threshold)
        {
            // Get the gradient image
            Mat result = new Mat();
            Cv2.MorphologyEx(image, result, MorphTypes.Gradient, new Mat());
            applyThreshold(result, threshold);

            return result;
        }
        //____________________________________________________________________________________
        static void drawcircle()
        {
            Mat image = Mat.Zeros(400, 400, MatType.CV_8UC3);
            Cv2.Circle(image, new Point(200, 200), 100, new Scalar(255, 128, 0), 2);
            using (new Window("Image", image))
                Cv2.WaitKey(0);
        }
        //____________________________________________________________________________________
        static void applyThreshold(Mat result, int threshold)
        {
            Cv2.Threshold(result, result, threshold, 255, ThresholdTypes.Binary);
        }
        //____________________________________________________________________________________
        static Mat createACross()
        {
            Mat cross = new Mat(5, 5, MatType.CV_8U, new Scalar(0));

            // creating the cross-shaped structuring element
            for (int i = 0; i < 5; i++)
            {
                cross.Set<byte>(2, i, 1);
                cross.Set<byte>(i, 2, 1);
            }

            return cross;
        }
        //____________________________________________________________________________________
        static Mat createADiamond()
        {
            Mat diamond = new Mat(5, 5, MatType.CV_8U, new Scalar(1));

            // Creating the diamond-shaped structuring element
            diamond.Set<byte>(0, 0, 0);
            diamond.Set<byte>(1, 0, 0);
            diamond.Set<byte>(3, 0, 0);
            diamond.Set<byte>(4, 0, 0);
            diamond.Set<byte>(0, 1, 0);
            diamond.Set<byte>(4, 1, 0);
            diamond.Set<byte>(0, 3, 0);
            diamond.Set<byte>(4, 3, 0);
            diamond.Set<byte>(4, 4, 0);
            diamond.Set<byte>(0, 4, 0);
            diamond.Set<byte>(1, 4, 0);
            diamond.Set<byte>(3, 4, 0);

            return diamond;
        }
        //____________________________________________________________________________________
        static Mat createASquare()
        {
            Mat Square = new Mat(5, 5, MatType.CV_8U, new Scalar(1));

            return Square;
        }
        //____________________________________________________________________________________
        static Mat createAXShape()
        {
            Mat x = new Mat(5, 5, MatType.CV_8U, new Scalar(0));

            // Creating the x-shaped structuring element
            for (int i = 0; i < 5; i++)
            {
                x.Set<byte>(i, i, 1);
                x.Set<byte>(4 - i, i, 1);
            }

            return x;
        }
        //____________________________________________________________________________________
        static void IDTheCorners(Mat binary, Mat image, ref int[,] cornerarray, ref int f)
        {
            for (int r = 0; r < binary.Rows; r++)
                for (int c = 0; c < binary.Cols; c++)
                    if (binary.At<byte>(r, c) != 0)
                    {
                        Cv2.Circle(image, c, r, 5, new Scalar(128));
                        cornerarray[f,0] = r;
                        cornerarray[f,1] = c;
                        f++;
                    }
        }
        //____________________________________________________________________________________
        static void getcorners(ref int[,] cornerarray , ref int f)
        {
            Mat src = new Mat("testobj.jpg", ImreadModes.GrayScale);

            // Show Edges
            Mat edges = getEdges(src, 50);
            new Window("Edges", edges);

            // Corner detection
            // Get All Processing Images
            Mat cross = createACross();
            Mat diamond = createADiamond();
            Mat square = createASquare();
            Mat x = createAXShape();
            Mat dst = new Mat();

            // Dilate with a cross
            Cv2.Dilate(src, dst, cross);

            // Erode with a diamond
            Cv2.Erode(dst, dst, diamond);

            Mat dst2 = new Mat();

            // Dilate with a X
            Cv2.Dilate(src, dst2, x);

            // Erode with a square
            Cv2.Erode(dst2, dst2, square);

            // Corners are obtain by differencing the two closed images
            Cv2.Absdiff(dst, dst2, dst);
            applyThreshold(dst, 45);

            // The following code Identifies the founded corners by
            // drawing circle on the src image.
            IDTheCorners(dst, src, ref cornerarray, ref f);
            new Window("Corner on Image", src);
            Cv2.WaitKey();
        }
        //____________________________________________________________________________________
        static void getlines(ref int[,] linearray, ref int l)
        {
            Mat imageIn = Cv2.ImRead("testobj.jpg", ImreadModes.GrayScale);
            Mat imageIn2 = new Mat();
            Mat edges = new Mat();
            Point[][] kontur;
            HierarchyIndex[] hierarchy;

            Cv2.EqualizeHist(imageIn, imageIn2);
            Cv2.Canny(imageIn2, edges, 95, 100);
            //edges = getEdges(imageIn2, 100);
            Cv2.FindContours(imageIn2, out kontur, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            var markers = new Mat(imageIn2.Size(), MatType.CV_32S, s: Scalar.All(0));

            var componentCount = 0;
            var contourIndex = 0;
            while ((contourIndex >= 0))
            {
                Cv2.DrawContours(
                    markers,
                    kontur,
                    contourIndex,
                    color: Scalar.Red,
                    thickness: -1,
                    lineType: LineTypes.Link8,
                    hierarchy: hierarchy,
                    maxLevel: int.MaxValue);

                componentCount++;
                contourIndex = hierarchy[contourIndex].Next;
            }




            //HoughLinesP
            LineSegmentPoint[] segHoughP = Cv2.HoughLinesP(edges, 1, Math.PI / 180, 100, 0, 50);
            Mat imageOutP = imageIn.EmptyClone(); 

            foreach (LineSegmentPoint s in segHoughP)
            {
                imageOutP.Line(s.P1, s.P2, Scalar.White, 1, LineTypes.AntiAlias, 0);
                linearray[l, 0] = s.P1.X;
                linearray[l, 1] = s.P1.Y;
                linearray[l, 2] = s.P2.X;
                linearray[l, 3] = s.P2.Y;
                l++;
            }

            using (new Window("start", WindowMode.AutoSize, imageIn))
            using (new Window("equalized", WindowMode.AutoSize, imageIn2))
            using (new Window("konturen", WindowMode.AutoSize, markers))
            using (new Window("Edges", WindowMode.AutoSize, edges))
            using (new Window("HoughLinesP", WindowMode.AutoSize, imageOutP))
            {
                Window.WaitKey(0);
            }
        }

    }
}
