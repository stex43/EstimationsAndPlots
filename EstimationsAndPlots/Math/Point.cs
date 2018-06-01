namespace EstimationsAndPlots
{
    public class Point
    {
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Point() : this(0, 0)
        {

        }

        private double x;
        private double y;

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
    }
}
