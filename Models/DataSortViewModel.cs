using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plan_io_T.Models;

namespace Plan_io_T.Models {
    public class DataSortViewModel {
        public DataSortState IdSort { get; private set; } 
        public DataSortState DateSort { get; private set; }
        public DataSortState TimeSort { get; private set; }
        public DataSortState NodeIdSort { get; private set; }
        public DataSortState CoSort { get; private set; }
        public DataSortState Co2Sort { get; private set; }
        public DataSortState TempSort { get; private set; }
        public DataSortState HumSort { get; private set; }
        public DataSortState LpgSort { get; private set; }
        public DataSortState SmkSort { get; private set; }
        public DataSortState Current { get; private set; }

        public DataSortViewModel(DataSortState sortOrder) {
            IdSort = sortOrder == DataSortState.idAsc ? DataSortState.idDesc : DataSortState.idAsc;
            DateSort = sortOrder == DataSortState.dateAsc ? DataSortState.dateDesc : DataSortState.dateAsc;
            TimeSort = sortOrder == DataSortState.timeAsc ? DataSortState.timeDesc : DataSortState.timeAsc;
            NodeIdSort = sortOrder == DataSortState.nodeIdAsc ? DataSortState.nodeIdDesc : DataSortState.nodeIdAsc;
            CoSort = sortOrder == DataSortState.coAsc ? DataSortState.coDesc : DataSortState.coAsc;
            Co2Sort = sortOrder == DataSortState.co2Asc ? DataSortState.co2Desc : DataSortState.co2Asc;
            TempSort = sortOrder == DataSortState.tempAsc ? DataSortState.tempDesc : DataSortState.tempAsc;
            HumSort = sortOrder == DataSortState.humAsc ? DataSortState.humDesc : DataSortState.humAsc;
            LpgSort = sortOrder == DataSortState.lpgAsc ? DataSortState.lpgDesc : DataSortState.lpgAsc;
            SmkSort = sortOrder == DataSortState.smkAsc ? DataSortState.smkDesc : DataSortState.smkAsc;
            Current = sortOrder;
        }
    }
}
