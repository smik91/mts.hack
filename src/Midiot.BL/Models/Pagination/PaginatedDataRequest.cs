using System.ComponentModel.DataAnnotations;

namespace Midiot.BL.Models.Pagination;

public class PaginatedDataRequest
{
    [Range(1, 1000, ErrorMessage = "Page index must be in range [1, 1000]")]
    public int PageIndex { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be in range [1, 100]")]
    public int PageSize { get; set; } = 10;
    public string SearchedTitle { get; set; } = "";
}
