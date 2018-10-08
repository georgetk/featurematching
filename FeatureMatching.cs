using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace Scanner.BusinessLayer.BusinessLogic
{
    public class ImageBusiness
    {
        public void MatchFeatures()
        {

            string PathToImage1 = "C:\\Users\\Image1.jpg";
            string PathToImage2 = "C:\\Users\\Image2.jpg";

            Mat Image1 = CvInvoke.Imread(PathToImage1);

            /* Emgu.CV.Mat is a class which can store the pixel values.
             * Emgu.CV.CvInvoke is the library to invoke OpenCV functions. 
             * Imread loads an image from the specified path. 
             * Image1 now have the details of first image */

            Mat Image2 = CvInvoke.Imread(PathToImage2); // Image2 now have the details of second image 

            ORBDetector ORB = new ORBDetector(); // Emgu.CV.Features2D.ORBDetector class. Now, ORB is an instance of the class.

            VectorOfKeyPoint KeyPoints1 = new VectorOfKeyPoint(); // KeyPoints1 - for storing the keypoints of Image1

            VectorOfKeyPoint KeyPoints2 = new VectorOfKeyPoint(); // KeyPoints2 - for storing the keypoints of Image2

            Mat Descriptors1 = new Mat(); // Descriptors1 - for storing the descriptors of Image1

            Mat Descriptors2 = new Mat(); // Descriptors2 - for storing the descriptors of Image2


            //Feature Extraction from Image1
            ORB.DetectAndCompute(Image1, null, KeyPoints1, Descriptors1, false);

            /* Detects Keypoints in Image1 and then computes descriptors on the image from the keypoints. 
             * Keypoints will be stored into - KeyPoints1 and Descriptors will be stored into - Descriptors1*/


            //Feature Extraction from Image2
            ORB.DetectAndCompute(Image2, null, KeyPoints2, Descriptors2, false);

            int k = 2;
            /*  Count of best matches found per each descriptor 
            or less if a descriptor has less than k possible matches in total. */

            BFMatcher matcher = new BFMatcher(DistanceType.Hamming); // BruteForceMatcher to perform descriptor matching.

            matcher.Add(Descriptors1); // Descriptors of Image1 is added.

            VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch(); // For storing the output of matching operation.

            matcher.KnnMatch(Descriptors2, matches, k, null); // matches will now have the result of matching operation.


            /* After the matching operation, we will get a 2D array (as k = 2). 
             * For checking whether two Images are similar or not, 
             * we need the distance parameter from this array.
             * (matches[0][0].Distance, matches[0][0].Distance, matches[1][0].Distance, ...)
             * If Image1 and Image2 are same, all  distance values will be 0.
             * If they're similar the distance values will be lesser. 
             * Otherwise, distance values will be greater*/


            /* That is, for two images to be similar, the distance values present in the matches array,
             * should be lesser */


            List<float> matchList = new List<float>();


            /* The matching operation, in some situation, may result in false-positive results. 
             * For filtering out the false-positive results, David Lowe proposed a test 
             * https://www.cs.ubc.ca/~lowe/papers/ijcv04.pdf#page=20
             * This test rejects poor matches by computing the ratio between the best and second-best match. 
             * If the ratio is below some threshold, the match is discarded as being low-quality. */

            for (int b = 0; b < matches.Size; ++b)
            {
                const double ratio = 0.8; // As in Lowe's paper; can be tuned accordingly.
                if (matches[b][0].Distance < ratio * matches[b][1].Distance)
                {
                    matchList.Add(matches[b][0].Distance);
                }
            }
            

            matchList.Sort();
            matchList = matchList.Take(40).ToList();

            /* matchList will now contain first 40 matches.
             * Based on my research, I had found that for qualifying as a similar image,
             * there should be atleast 10 distance values, 
             * with distance value less than or equal to 45
             * **In this case. Tune this to your particular situation** */

            int distanceThreshold = 45;
            int FilterThreshold = 10;
            int FilterCount = 0;

            for (j = 0; j < matchList.Count; j++)
            {
                if ((matchList[j]) <= (distanceThreshold))
                {
                    FilterCount++;
                }
            }


            if (FilterCount >= FilterThreshold)
            {
                // Images are similar, perform the operation you want. 
                FilterCount = 0;

            }
            else
                FilterCount = 0;
        }
    }
}
 
 
 