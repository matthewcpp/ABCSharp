namespace ABC
{
    public struct Rest
    {
        public Length length { get; set; }
        public bool isVisible { get; set; }

        public Rest(Length length = Length.Eighth, bool isVisible = true)
        {
            this.length = length;
            this.isVisible = isVisible;
        }
    }
}