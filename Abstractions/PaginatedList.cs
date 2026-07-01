namespace SurveyBasket.Abstractions;

public class PaginatedList<T>
{
    public PaginatedList(List<T> items,int countRecords,int pageSize,int pageNumber)
    {
        PageSize = pageSize;
        Items= items;
        TotolPages = (int) Math.Ceiling((double)countRecords/PageSize);
        PageNumber= pageNumber;
    }
    public List<T> Items { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int TotolPages { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage=> PageNumber < Items.Count;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var items=  query.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();
        var countRecords = query.Count();
        return new PaginatedList<T>(items, countRecords, pageSize, pageNumber);

    }


}
