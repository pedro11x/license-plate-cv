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

        Dictionary<char, Image<Bgr, Byte>> db = new Dictionary<char, Image<Bgr, byte>>();

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
                //saving image
                ImageClass.OtsuBinarization(ci);
                Console.WriteLine("[Adding character {0} from file {1}]".PadLeft(4), C.ToString().ToUpper(), file);
                db.Add(C, ci);
            }
        }



    }
}
