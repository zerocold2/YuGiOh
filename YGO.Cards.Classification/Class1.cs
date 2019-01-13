using System;
using System.Collections.Generic;
using System.Drawing;
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
    public interface IMatchTemplate
    {
        List<Rectangle> Match(byte[] sourceImage, int srcWidth, int srcHeight,
            byte[] templateImage, int tmpWidth, int tmpHeight, double threshold, bool enableGpu);
    }
    public class MatchTemplate : IMatchTemplate
    {
        public List<Rectangle> Match(byte[] sourceImage, int srcWidth, int srcHeight, byte[] templateImage, int tmpWidth, int tmpHeight, double threshold, bool enableGpu)
        {
            List<Rectangle> ret = new List<Rectangle>();
            ret.Add(TemplateMatch(sourceImage, srcWidth, srcHeight, templateImage, srcWidth, srcHeight, threshold));
            return ret;
        }

        private Rectangle TemplateMatch(byte[] sourceImage, int srcWidth, int srcHeight, byte[] templateImage, int tmpWidth, int tmpHeight, double threshold)
        {

            Image<Bgr, byte> source = new Image<Bgr, byte>(srcWidth, srcHeight) { Bytes = sourceImage }; // srcImage
            Image<Bgr, byte> template = new Image<Bgr, byte>(tmpWidth, tmpHeight) { Bytes = templateImage }; // template
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
                }
            }
            return ret;
        }
    }
}
