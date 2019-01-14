using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Stitching;
using Emgu.CV.Structure;

namespace YGO.Cards.Classification
{
    public class MatchTemplate : IMatchTemplate
    {
        public List<Rectangle> Match(byte[] sourceImage, int srcWidth, int srcHeight, byte[] templateImage, int tmpWidth, int tmpHeight, double threshold, bool enableGpu)
        {
            List<Rectangle> ret = new List<Rectangle>();
            var templateMatch = TemplateMatch(sourceImage, srcWidth, srcHeight, templateImage, tmpWidth, tmpHeight, threshold);
            if (templateMatch != null)
                ret.Add(templateMatch.Value);
            return ret;
        }

        public List<Rectangle> MatchRegion(byte[] sourceImage, Rectangle ROI, byte[] templateImage, int tmpWidth,
            int tmpHeight, double threshold, bool enableGpu)
        {
            List<Rectangle> ret = new List<Rectangle>();
            var regionBytes = EllipseOfInterest(sourceImage, ROI);
            var tmatch = TemplateMatch(regionBytes, ROI.Width, ROI.Height, templateImage, tmpWidth, tmpHeight, threshold);
            if (tmatch != null)
                ret.Add(tmatch.Value);
            return ret;
        }

        private byte[] RegionOfInterest(byte[] srcImage, Rectangle regionOfInterest)
        {
            using (MemoryStream mem = new MemoryStream(srcImage))
            {
                Bitmap srcBitmap = new Bitmap(mem);
                Bitmap croppedImage = new Bitmap(regionOfInterest.Width, regionOfInterest.Height, srcBitmap.PixelFormat);
                using (croppedImage)
                {
                    using (Graphics croppingGr = Graphics.FromImage(croppedImage))
                    {
                        croppingGr.DrawImage(srcBitmap,
                            new Rectangle(0, 0, regionOfInterest.Width, regionOfInterest.Height), regionOfInterest,
                            GraphicsUnit.Pixel);
                    }
                    srcBitmap.Dispose();
                    return BitmapToBytes(croppedImage);
                }
            }
        }

        private byte[] EllipseOfInterest(byte[] srcImage, Rectangle regionOfInterest)
        {
            using (MemoryStream mem = new MemoryStream(srcImage))
            {
                Bitmap srcBitmap = new Bitmap(mem);
                Bitmap croppedImage = new Bitmap(regionOfInterest.Width, regionOfInterest.Height, srcBitmap.PixelFormat);
                using (croppedImage)
                {
                    using (Graphics croppingGr = Graphics.FromImage(croppedImage))
                    {
                        Brush tBrush = new TextureBrush(srcBitmap, new Rectangle(regionOfInterest.X, regionOfInterest.Y, regionOfInterest.Width, regionOfInterest.Height));

                        croppingGr.FillEllipse(tBrush, new Rectangle(0, 0, regionOfInterest.Width, regionOfInterest.Height));
                        tBrush.Dispose();
                    }
                    srcBitmap.Dispose();
                    return BitmapToBytes(croppedImage);
                }
            }
        }

        private byte[] BitmapToBytes(Bitmap srcBitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                srcBitmap.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
        private Rectangle? TemplateMatch(byte[] sourceImage, int srcWidth, int srcHeight, byte[] templateImage, int tmpWidth, int tmpHeight, double threshold)
        {
            Bitmap b = new Bitmap(new MemoryStream(sourceImage));
            Bitmap b1 = new Bitmap(new MemoryStream(templateImage));
            Image<Bgr, byte> source = new Image<Bgr, byte>(b); // srcImage
            Image<Bgr, byte> template = new Image<Bgr, byte>(b1); // template
            //Resizing source image if it smaller than the template
            if (srcWidth < tmpWidth || srcHeight < tmpHeight)
            {
                source = source.Resize(tmpWidth, tmpHeight, Inter.Linear);
                //template = template.Resize(0.5, Inter.Linear);
                //source.Save("s.png");
            }
            //template.Save("t.png");
            b.Dispose(); b1.Dispose();
            Rectangle ret = new Rectangle();
            using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
                if (maxValues[0] > threshold)
                {
                    ret = new Rectangle(maxLocations[0], template.Size);
                    return ret;
                }
            }
            return null;
        }
    }
}
