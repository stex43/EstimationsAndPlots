namespace EstimationsAndPlots
{
    public interface IDistance
    {
        double Distance(Function function);

        double[] DistanceDerivative(Function function);
    }
}
