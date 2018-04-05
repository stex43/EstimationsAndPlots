namespace EstimationsAndPlots
{
    interface IFunction
    {
        double FunctionValue(double x);
        double[] FunctionValue(double[] x);    
        
        int NumberOfParameters { get; }
        double[] Parameters { set; get; }
    }
}
