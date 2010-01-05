namespace SimpleExpressionEvaluator.Evaluation
{
    public interface IVariableResolutionService
    {
        object ResolveVariableValue(string variableName);
    }
}