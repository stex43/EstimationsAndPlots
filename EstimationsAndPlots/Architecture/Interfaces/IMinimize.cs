namespace EstimationsAndPlots
{
    interface IMinimize
    {
        double[] Minimize(Function function, IDistance distance);
    }
}
