using Microsoft.EntityFrameworkCore;
using TpePrmcyWms.Models.Service;

namespace TpePrmcyWms.Models.Unit.Back
{
    //ref: https://learn.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-8.0
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public string PageTagHtml { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            int pagetag = Convert.ToInt16(SysBaseServ.JsonConf("SystemBase:PagesTagNumberCount"));
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageIndex = pageIndex;

            PageTagHtml = $"<div {(!HasPreviousPage ? "" : $"onclick = \"goListQuery('pageNum={PageIndex - 1}')\"")}  class=\"CtrlBtn {(!HasPreviousPage ? "disabled" : "")}\"> ☚ </div>";
            for (int i = (PageIndex - pagetag < 1 ? 1 : PageIndex - pagetag); i <= (PageIndex + pagetag < TotalPages ? PageIndex + pagetag : TotalPages); i++)
            {
                
                PageTagHtml += $"<div {(PageIndex == i ? "" : $"onclick = \"goListQuery('pageNum={i}')\"")} class=\"CtrlBtn {(PageIndex==i ? "disabled" : "")}\">{i}</div> ";
            }
            PageTagHtml += $"<div {(!HasNextPage ? "" : $"onclick = \"goListQuery('pageNum={PageIndex + 1}')\"")}  class=\"CtrlBtn {(!HasNextPage ? "disabled" : "")}\">☛</div> ";

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            pageIndex = TotalPages >= pageIndex ? pageIndex : 1;
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
