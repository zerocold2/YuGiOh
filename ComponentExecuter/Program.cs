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
using YGO.Domain.Entities;

namespace ComponentExecuter
{
    class Program
    {
        static Rectangle _title = new Rectangle(19, 20, 264 - 19, 48 - 20),
            _icon = new Rectangle(265, 19, 300 - 265, 48 - 20),
            _level_type_iconType = new Rectangle(34, 35, 292 - 34, 72 - 53),
            _num = new Rectangle(231, 311, 302 - 231, 318 - 311),
            _effect = new Rectangle(22, 323, 283 - 22, 336 - 323),
            _desc = new Rectangle(22, 337, 297 - 22, 391 - 337),
            _atk_def = new Rectangle(164, 394, 299 - 164, 408 - 394),
            _pendulum_left = new Rectangle(19, 273, 44 - 19, 321 - 273),
            _pendulum_right = new Rectangle(275, 273, 300 - 275, 320 - 273),
            _pendulum_mid = new Rectangle(46, 273, 272 - 46, 320 - 273);

        private static MatchTemplate templateMatch;
        static void Main(string[] args)
        {
            templateMatch = new MatchTemplate();




            var cardList = Cards(@"D:\Entertetment\YuGiOh\YuGi");
            for (int i = 0; i < cardList.Length; i++)
            {
                Image img = Image.FromFile(cardList[i].FullName);
                int width = img.Width, height = img.Height;
                MonsterCards(img);
            }

        }

        static Card GetCardDetails(Image cardImage)
        {
            Card card = new Card();
            
            return card;
        }
        static bool MonsterCards(Image cardImage)
        {
            var iconTypesDirectory = @"D:\Entertetment\YuGiOh\YuGi\Attributes\";
            var iconTypesDirList = Cards(iconTypesDirectory);
            for (int i = 0; i < iconTypesDirList.Length; ++i)
            {
                var tmpImg = Image.FromFile(iconTypesDirList[i].FullName);
                var res = templateMatch.MatchRegion(ImageToBytes(cardImage), _icon, ImageToBytes(tmpImg), tmpImg.Width, tmpImg.Height, 0.8, false);
                if (res.Count > 0)
                {

                }
            }
            return true;
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
