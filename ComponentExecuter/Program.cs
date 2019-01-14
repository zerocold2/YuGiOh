using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            _icon = new Rectangle(269, 19, 299 - 269, 49 - 19),
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




            var cardList = Cards(@"D:\_ZC\JDownload\misc\YuGi");//D:\Entertetment\YuGiOh\YuGi
            var monsterDir = @"D:\_ZC\JDownload\misc\YuGi\Monsters";
            for (int i = 0; i < cardList.Length; i++)
            {
                Image img = Image.FromFile(cardList[i].FullName);
                int width = img.Width, height = img.Height;
                var ret = MonsterCards(img);
                Console.WriteLine(i);
                if (!string.IsNullOrEmpty(ret))
                {
                    switch (ret)
                    {
                        case "WATER":
                            cardList[i].CopyTo($"{monsterDir}\\{ret}\\{cardList[i].Name}");
                            break;
                        case "DARK":
                            cardList[i].CopyTo($"{monsterDir}\\{ret}\\{cardList[i].Name}");
                            break;
                        case "FIRE":
                            cardList[i].CopyTo($"{monsterDir}\\{ret}\\{cardList[i].Name}");
                            break;
                        case "LIGHT":
                            cardList[i].CopyTo($"{monsterDir}\\{ret}\\{cardList[i].Name}");
                            break;
                        case "WIND":
                            cardList[i].CopyTo($"{monsterDir}\\{ret}\\{cardList[i].Name}");
                            break;
                        case "EARTH":
                            cardList[i].CopyTo($"{monsterDir}\\{ret}\\{cardList[i].Name}");
                            break;
                    }
                }
            }

        }

        static Card GetCardDetails(Image cardImage)
        {
            Card card = new Card();

            return card;
        }
        static Dictionary<string, int> dic = new Dictionary<string, int>();
        static string MonsterCards(Image cardImage)
        {
            var iconTypesDirectory = @"D:\_ZC\JDownload\misc\YuGi\Attributes";
            var iconTypesDirList = Cards(iconTypesDirectory);


            for (int i = 0; i < iconTypesDirList.Length; ++i)
            {
                var tmpImg = Image.FromFile(iconTypesDirList[i].FullName);
                var res = templateMatch.MatchRegion(ImageToBytes(cardImage), _icon, ImageToBytes(tmpImg), tmpImg.Width, tmpImg.Height, 0.5, false);
                if (res.Count > 0)
                {
                    //Console.WriteLine(iconTypesDirList[i].Name);
                    return iconTypesDirList[i].Name.Split('.')[0];
                }
            }
            return "";
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
