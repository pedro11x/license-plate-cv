using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SS_OpenCV.LPRecognition;

namespace SS_OpenCV
{
    class LPLocation
    {
        public static Image<Bgr, Byte> getLicensePlate(Image<Bgr, Byte> img) {


            //Image<Bgr,Byte>cut = img.Copy(new Rectangle(x1, y1, width, height));



            return null;
        }


        public static Image<Bgr, Byte> t(Image<Bgr,Byte> _img) {
            Image<Bgr, Byte> img = _img.Copy();
            ImageClass.OtsuBinarization(img);
            System.Drawing.Rectangle r = getYLocationImage(img);
            if (r != NULLR)
                return _img.Copy(r);
            else
                return _img;
        }

        public static System.Drawing.Rectangle getYLocationImage(Image<Bgr, Byte> _img) {
            Image<Bgr, Byte> img = _img.Copy();
            ImageClass.EdgeDetectionSobel3x3X(img);
            
            ImageClass.Projection p = ImageClass.VProjection(img);

            Utils.applyMovingAverage(p, .02);

            //new GraphXY(new int[1], p.values);
            double stepdiv = .01;
            int step = (int)((p.peak*stepdiv > 1) ? (p.peak * stepdiv) : 1);
            for (int i = (int)(p.peak * 0.99); i > 0; i-=step)
            {
                Console.WriteLine("trying for {0}", i);
                List<Region> rl = LPRecognition.regionsListFromThreshold(p, i);
                foreach (Region r in rl) {
                    Image<Bgr, Byte> cut = _img.Copy(new System.Drawing.Rectangle(0, r.startPoint, img.Width, r.delta));
                    iu.updateImage(cut);
                    System.Drawing.Rectangle fr = locateHorizontally(cut);
                    if (fr != NULLR) return fr;
                }
            }

            /*
            List<Region> rs = LPRecognition.regionsListFromThreshold(p, 80);
            Region r = rs[1];
            System.Drawing.Rectangle rekt = getXLocationImage(_img.Copy(new System.Drawing.Rectangle(0, r.startPoint, img.Width, r.delta)));
            rekt.Offset(0, r.startPoint);
            */
            return NULLR;
        }

        public static MainForm.ImageUpdateble iu = null;

        public static System.Drawing.Rectangle locateHorizontally(Image<Bgr, Byte> _img)
        {
            Image<Bgr, Byte> img = _img.Copy();
            ImageClass.EdgeDetectionSobel3x3Y(img);

            ImageClass.Projection p = ImageClass.HProjection(img);
            if (p.peak < 3) return NULLR;
            Utils.applyMovingAverage(p, .02);

            new GraphXY(p.values, new int[1]);
            double stepdiv = 0.1;
            int step = (int)((p.peak * stepdiv > 1) ? (p.peak * stepdiv) : 1);
            for (int i = 0; i < p.peak; i += step)
            {
                //Console.WriteLine("trying for {0}", i);
                List<Region> rl = LPRecognition.regionsListFromThreshold(p, i);
                foreach (Region r in rl)
                {
                    //Image<Bgr, Byte> lp = _img.Copy();
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(r.startPoint, 0, r.delta, img.Height);
                    if (acceptLP(rect)) return rect;
                }
            }
            return NULLR;
            //return new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
        }



        public static bool acceptLP(System.Drawing.Rectangle img) {
            double ratio = img.Width / (img.Height*1.0);
            return ratio > 3.5 && ratio < 5.5 && img.Height > 10;
        }

        public static System.Drawing.Rectangle NULLR = new System.Drawing.Rectangle(0, 0, 0, 0);
    }
}
