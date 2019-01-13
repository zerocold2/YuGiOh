namespace YGO.Domain.Entities
{
    public class MonsterCard : Card
    {
        public int Level { get; set; }
        public int AttakPoints { get; set; }
        public int DefencePoints { get; set; }
        public MonsterEffect MonsterEffect { get; set; }
    }
}