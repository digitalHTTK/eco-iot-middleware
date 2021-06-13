using System;
using System.Collections.Generic;
using Plan_io_T.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Plan_io_T.Models {
    public class DataFilterViewModel {
        public DataFilterViewModel(int nodeId, string date) {
            SelectedNodeId = nodeId;
            SelectedDate = date;
        }
        public int SelectedNodeId { get; private set; }   
        public string SelectedDate { get; private set; }    
    }
}

