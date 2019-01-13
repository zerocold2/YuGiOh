using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using YGO.Cards.Classification;

namespace ComponentExecuter
{
    class Program
    {
        static void Main(string[] args)
        {
            MatchTemplate templateMatch = new MatchTemplate();
            var cardList = Cards(@"D:\Entertetment\YuGiOh\YuGi");
            for (int i = 0; i < cardList.Length; i++)
            {
                Image img = Image.FromFile(cardList[i].FullName);
                int width = img.Width, height = img.Height;
                //templateMatch.Match(ImageToBytes(img),width,height,)
            }

        }

        static System.IO.FileInfo[] Cards(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            FileInfo[] ret = new FileInfo[files.Length];
            int i = 0;
            foreach (var file in files)
            {
                ret[i++] = new FileInfo(file);
            }
            return ret;
        }

        static byte[] ImageToBytes(Image img)
        {
            byte[] retBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                retBytes = ms.ToArray();
            }
            return retBytes;
        }
    }
}
