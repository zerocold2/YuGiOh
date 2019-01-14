using System.Collections.Generic;
using System.Drawing;
using YGO.Domain.Entities;

namespace YGO.Cards.Classification
{
    public interface IMatchTemplate
    {
        List<Rectangle> Match(byte[] sourceImage, int srcWidth, int srcHeight,
            byte[] templateImage, int tmpWidth, int tmpHeight, double threshold, bool enableGpu);
        List<Rectangle> MatchRegion(byte[] sourceImage, Rectangle ROI,
            byte[] templateImage, int tmpWidth, int tmpHeight, double threshold, bool enableGpu);
        List<Rectangle> MatchRegion(byte[] sourceImage, Rectangle ROI, RegionType type,
            byte[] templateImage, int tmpWidth, int tmpHeight, double threshold, bool enableGpu);
    }
}