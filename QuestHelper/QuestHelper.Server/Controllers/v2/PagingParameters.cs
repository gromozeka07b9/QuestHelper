using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Controllers.v2
{
    public class PagingParameters
    {
        const int maxPageSize = 50;
        private int _pageNumber = 1;
        private int _pageSize = 3;
        private string _range = string.Empty;

        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
        }

        public string Filter { get; set; } = string.Empty;

        public string Range
        {
            get
            {
                return _range;
            }
            set
            {
                _range = value;
            }
        }

        public int IndexesRangeToPageNumber(string range, int pageSize)
        {
            //[0,9]=1,[10,19]=2,PageSize=10
            int idxFrom, idxTo = 0;
            var arrIdx = range.Replace("[", "").Replace("]", "").Split(",");
            if (arrIdx.Length == 2)
            {
                idxFrom = Convert.ToInt32(arrIdx[0]);
                idxTo = Convert.ToInt32(arrIdx[1]);
                if (idxFrom == 0)
                {
                    return 1;
                }
                else
                {
                    return (idxFrom / pageSize) + 1;
                }
            }
            else
            {
                return 1;
            }
        }

        public string Sort { get; set; } = string.Empty;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
