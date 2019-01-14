using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YGO.Cards.Classification
{
    public interface ITextRecognition
    {
        string Recognize(byte[] srcImage);
        string Recongnize(byte[] srcImage, Rectangle ROI);
    }
    
}
