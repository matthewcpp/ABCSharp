namespace ABC
{
    public class Rest : Duration
    {
        public bool isVisible { get; set; }

        public Rest(bool isVisible = true) : base(Item.Type.Rest)
        {
        }
    }

    public class MultiMeasureRest : Item
    {
        public bool isVisible { get; set; }
        public int count { get; set; }
        
        public MultiMeasureRest() : base(Type.MultiMeasureRest)
        {
        }
    }
}