using System;
using System.Collections.Generic;
using WebApiCore.Common;

namespace WebApiCore.DataTransferObject
{
    public class PagingModel
    {
        public int Skip { get; set; }
        private int _take;
        public int Take
        {
            get
            {
                return _take == 0 || _take > Constant.MaximumItem ? Constant.MaximumItem : _take;
            }
            set => _take = value;
        }

        public List<SortModel> Sort { get; set; }

        public string SortField
        {
            get
            {
                return Sort != null && Sort.Count > 0 ? Sort[0].Field : string.Empty;
            }
        }

        public string SortType
        {
            get
            {
                return Sort != null && Sort.Count > 0 ? Sort[0].Dir : string.Empty;
            }
        }

        public string FilterOperator { get; set; }
        public string FilterField { get; set; }
        public string FilterValue { get; set; }

        private int _page;
        public int Page
        {
            get
            {
                return _page == 0 ? 1 : _page;
            }
            set => _page = value;
        }

        private int _pageSize;
        public int PageSize
        {
            get
            {
                return _pageSize == 0 || _pageSize > Constant.MaximumItem ? Constant.MaximumItem : _pageSize;
            }
            set => _pageSize = value;
        }

        public class SortModel
        {
            public string Dir { get; set; }
            public string Field { get; set; }
        }
    }
}
