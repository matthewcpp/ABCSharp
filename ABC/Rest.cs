namespace ABC
{
    public class Rest : Duration
    {
        public bool isVisible { get; set; }

        public Rest(bool isVisible = true) : base(Item.Type.Rest)
        {
        }
    }

    public class MultiMeasureRest
    {
        public bool isVisible { get; set; }
        public int count { get; set; }
    }
}