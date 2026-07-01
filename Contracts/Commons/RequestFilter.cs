namespace SurveyBasket.Contracts.Commons;

public record RequestFilter
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public string ?SearchValue { get; init; }
    public string? SortColumn { get; init; }

    public string? SortDirection { get; init; } = "ASC";

}
