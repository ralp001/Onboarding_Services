// Application/Common/Models/Pagination.cs
namespace Onboarding.Application.Common.Models;

public class Pagination
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public Pagination(int pageNumber, int pageSize, int totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public Pagination Pagination { get; }

    public PaginatedList(List<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        Pagination = new Pagination(pageNumber, pageSize, totalCount);
    }
}