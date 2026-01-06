namespace PPProject.Resource.Contract
{
    public enum ConsumeResourceError
    {
        None,
        InvalidConsumeCost,
        NotEnoughResource,
        UpdateFailed
    }
    public record ConsumeResourceResult(
        bool Success,
        int Amount,
        ConsumeResourceError errorCode);
}
