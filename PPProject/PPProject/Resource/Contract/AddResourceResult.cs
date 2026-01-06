namespace PPProject.Resource.Contract
{
    public enum AddResourceError
    {
        None,
        InavlidAddAmount,
        MaxCountExceed,
        UpdateFailed
    }
    public record AddResourceResult(
        bool Success,
        int Amount,
        AddResourceError errorCode);
}
