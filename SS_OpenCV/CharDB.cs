using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SS_OpenCV
{
    class CharDB
    {
        private static CharDB __instance = null;
        public static CharDB GetInstance() { return (CharDB.__instance != null) ? __instance : __instance = new CharDB("./chardb"); }

        public Dictionary<char, Image<Bgr, Byte>> db = new Dictionary<char, Image<Bgr, byte>>();

        /// <summary>
        /// Files are expected to be named "X.[image_extension]" being X the character it represents
        /// </summary>
        /// <param name="folder">folder that contains the character images</param>
        public CharDB(String folder) {
            String[] files = Directory.GetFiles(folder);
            Console.WriteLine("[[Starting character image database]]");
            foreach (String file in files) {
                //loading each file
                //expecting files to be named "X.[image_extension]" being X the charecter it represents
                char C;
                //getting X
                string fn = Path.GetFileName(file).Split('.')[0];

                if (fn.Length == 1) C = fn.ToUpper()[0]; else continue;
                //loading image and making it binary
                Image < Bgr, Byte > ci = new Image<Bgr, Byte>(file);
                //inverting
                ImageClass.DNegative(ci);
                //making image binary
                ImageClass.OtsuBinarization(ci);
                Console.WriteLine("[Adding character {0} from file {1}]".PadLeft(4), C.ToString().ToUpper(), file);
                //saving image
                db.Add(C, ci);
            }

            //char c = 'A';
            //double diff = compareImages(db[c],db[c]);
            //Console.WriteLine("---cmp {0} diff= {1}]", c, diff);
        }

        public static double compareImages(Image<Bgr, Byte> img1, Image<Bgr, Byte> img2) {
            int diff = 0;
            unsafe
            {
                MIplImage m1 = img1.MIplImage;
                byte* dataPtr1 = (byte*)m1.imageData.ToPointer(); // Pointer to the image

                int width1 = img1.Width;
                int height1 = img1.Height;
                int padding1 = m1.widthStep - m1.nChannels * m1.width; // alinhament bytes (padding)


                MIplImage m2 = img2.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image

                int width2 = img2.Width;
                int height2 = img2.Height;
                int padding2 = m2.widthStep - m2.nChannels * m2.width; // alinhament bytes (padding)
                int lineOffset2 = m2.nChannels * width2 + padding2;

                int nChan = m1.nChannels; // number of channels - 3
                
                for (int y = 0; y < height1; y++)
                {
                    for (int x = 0; x < width1; x++)
                    {
                        //Mapping pixels
                        int x2 = x * width2 / width1;
                        int y2 = y * height2 / height1;

                        //comparing reds
                        diff += Math.Abs(dataPtr2[lineOffset2 * y2 + x2 * nChan] - dataPtr1[0]);

                        dataPtr1 += nChan;
                    }
                    dataPtr1 += padding1;
                }
                return 1 - diff/(width1*height1*256.0);
            }
        }

        public double match(Image<Bgr, Byte> img, out char bestMatch) {
            bestMatch = '?';
            double bestValue = 0;
            foreach (char c in db.Keys) {
                double result;
                if ((result = compareImages(img, db[c])) > bestValue) {
                    bestMatch = c;
                    bestValue = result;
                }
            }
            return bestValue;
        }
        public double matchLetter(Image<Bgr, Byte> img, out char bestMatch)
        {
            bestMatch = '?';
            double bestValue = 0;
            foreach (char c in db.Keys)
            {
                if (c < 'A' || 'Z' < c) continue;
                double result;
                if ((result = compareImages(img, db[c])) > bestValue)
                {
                    bestMatch = c;
                    bestValue = result;
                }
            }
            return bestValue;
        }
        public double matchNumber(Image<Bgr, Byte> img, out char bestMatch)
        {
            bestMatch = '?';
            double bestValue = 0;
            foreach (char c in db.Keys)
            {
                if (c < '0' || '9' < c) continue;
                double result;
                if ((result = compareImages(img, db[c])) > bestValue)
                {
                    bestMatch = c;
                    bestValue = result;
                }
            }
            return bestValue;
        }

    }
}
