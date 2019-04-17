using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataTransferObject
{
    public interface ISearchResult<TResult>
        where TResult : class
    {
        IList<TResult> SearchResultItems { get; set; }
        int Count { get; set; }
    }
}
