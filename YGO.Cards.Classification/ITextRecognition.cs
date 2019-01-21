using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Text;
using Emgu.CV.Util;

namespace YGO.Cards.Classification
{
    public interface ITextRecognition
    {
        string Recognize(byte[] srcImage);
        string Recongnize(byte[] srcImage, Rectangle ROI);
    }

    enum OCRMode
    {
        /// <summary>
        /// Perform a full page OCR
        /// </summary>
        FullPage,

        /// <summary>
        /// Detect the text region before applying OCR.
        /// </summary>
        TextDetection
    }
    public class TesseractTextRecognition : ITextRecognition
    {
        private Mat _imageMat;
        public string Recognize(byte[] srcImage)
        {
            Tesseract ocr = new Tesseract();
            Mat outImage = new Mat();
            return OcrImage(ocr, ConvertToMat(srcImage), OCRMode.FullPage, outImage);
        }

        private Mat ConvertToMat(byte[] imageData)
        {
            Bitmap bitmap=new Bitmap(new MemoryStream(imageData));
            Image<Gray, byte> depthImage = new Image<Gray, byte>(bitmap);
            bitmap.Dispose();
            return depthImage.Mat;
        }
        public string Recongnize(byte[] srcImage, Rectangle ROI)
        {
            throw new NotImplementedException();
        }
        private string OcrImage(Tesseract ocr, Mat image, OCRMode mode, Mat imageColor)
        {
            Bgr drawCharColor = new Bgr(Color.Red);
            
            if (image.NumberOfChannels == 1)
                CvInvoke.CvtColor(image, imageColor, ColorConversion.Gray2Bgr);
            else
                image.CopyTo(imageColor);

            if (mode == OCRMode.FullPage)
            {
                ocr.Init("","eng",OcrEngineMode.TesseractLstmCombined);
                ocr.SetImage(imageColor);
                
                if (ocr.Recognize() != 0)
                    throw new Exception("Failed to recognizer image");
                Tesseract.Character[] characters = ocr.GetCharacters();
                if (characters.Length == 0)
                {
                    Mat imgGrey = new Mat();
                    CvInvoke.CvtColor(image, imgGrey, ColorConversion.Bgr2Gray);
                    Mat imgThresholded = new Mat();
                    CvInvoke.Threshold(imgGrey, imgThresholded, 65, 255, ThresholdType.Binary);
                    ocr.SetImage(imgThresholded);
                    characters = ocr.GetCharacters();
                    imageColor = imgThresholded;
                    if (characters.Length == 0)
                    {
                        CvInvoke.Threshold(image, imgThresholded, 190, 255, ThresholdType.Binary);
                        ocr.SetImage(imgThresholded);
                        characters = ocr.GetCharacters();
                        imageColor = imgThresholded;
                    }
                }
                foreach (Tesseract.Character c in characters)
                {
                    CvInvoke.Rectangle(imageColor, c.Region, drawCharColor.MCvScalar);
                }

                return ocr.GetUTF8Text();

            }
            else
            {
                bool checkInvert = true;
                Rectangle[] regions;
                using (ERFilterNM1 er1 = new ERFilterNM1("trained_classifierNM1.xml", 8, 0.00025f, 0.13f, 0.4f, true, 0.1f))
                using (ERFilterNM2 er2 = new ERFilterNM2("trained_classifierNM2.xml", 0.3f))
                {
                    int channelCount = image.NumberOfChannels;
                    UMat[] channels = new UMat[checkInvert ? channelCount * 2 : channelCount];

                    for (int i = 0; i < channelCount; i++)
                    {
                        UMat c = new UMat();
                        CvInvoke.ExtractChannel(image, c, i);
                        channels[i] = c;
                    }

                    if (checkInvert)
                    {
                        for (int i = 0; i < channelCount; i++)
                        {
                            UMat c = new UMat();
                            CvInvoke.BitwiseNot(channels[i], c);
                            channels[i + channelCount] = c;
                        }
                    }

                    VectorOfERStat[] regionVecs = new VectorOfERStat[channels.Length];
                    for (int i = 0; i < regionVecs.Length; i++)
                        regionVecs[i] = new VectorOfERStat();

                    try
                    {
                        for (int i = 0; i < channels.Length; i++)
                        {
                            er1.Run(channels[i], regionVecs[i]);
                            er2.Run(channels[i], regionVecs[i]);
                        }
                        using (VectorOfUMat vm = new VectorOfUMat(channels))
                        {
                            regions = ERFilter.ERGrouping(image, vm, regionVecs, ERFilter.GroupingMethod.OrientationHoriz,
                               "trained_classifier_erGrouping.xml", 0.5f);
                        }
                    }
                    finally
                    {
                        foreach (UMat tmp in channels)
                            if (tmp != null)
                                tmp.Dispose();
                        foreach (VectorOfERStat tmp in regionVecs)
                            if (tmp != null)
                                tmp.Dispose();
                    }

                    Rectangle imageRegion = new Rectangle(Point.Empty, imageColor.Size);
                    for (int i = 0; i < regions.Length; i++)
                    {
                        Rectangle r = ScaleRectangle(regions[i], 1.1);

                        r.Intersect(imageRegion);
                        regions[i] = r;
                    }

                }
                
                List<Tesseract.Character> allChars = new List<Tesseract.Character>();
                String allText = String.Empty;
                foreach (Rectangle rect in regions)
                {
                    using (Mat region = new Mat(image, rect))
                    {
                        ocr.SetImage(region);
                        if (ocr.Recognize() != 0)
                            throw new Exception("Failed to recognize image");
                        Tesseract.Character[] characters = ocr.GetCharacters();

                        //convert the coordinates from the local region to global
                        for (int i = 0; i < characters.Length; i++)
                        {
                            Rectangle charRegion = characters[i].Region;
                            charRegion.Offset(rect.Location);
                            characters[i].Region = charRegion;

                        }
                        allChars.AddRange(characters);

                        allText += ocr.GetUTF8Text() + Environment.NewLine;

                    }
                }

                Bgr drawRegionColor = new Bgr(Color.Red);
                foreach (Rectangle rect in regions)
                {
                    CvInvoke.Rectangle(imageColor, rect, drawRegionColor.MCvScalar);
                }
                foreach (Tesseract.Character c in allChars)
                {
                    CvInvoke.Rectangle(imageColor, c.Region, drawCharColor.MCvScalar);
                }

                return allText;
            }

        }

        private static Rectangle ScaleRectangle(Rectangle r, double scale)
        {
            double centerX = r.Location.X + r.Width / 2.0;
            double centerY = r.Location.Y + r.Height / 2.0;
            double newWidth = Math.Round(r.Width * scale);
            double newHeight = Math.Round(r.Height * scale);
            return new Rectangle((int)Math.Round(centerX - newWidth / 2.0), (int)Math.Round(centerY - newHeight / 2.0),
               (int)newWidth, (int)newHeight);
        }
    }
}
