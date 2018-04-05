namespace EstimationsAndPlots
{
    interface IMinimize
    {
        double[] Minimize(IFunction function, IDistance distance);
    }
}
