namespace YGO.Domain.Entities
{
    public enum RegionType
    {
        Undefined,
        Rectangle,
        Circle
    }
    public class Card
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public byte[] ImageBytes { get; set; }
        public byte[] IconBytes { get; set; }
        public CardType CardType { get; set; }
        public CardAttribute CardAttribute { get; set; }
    }
}