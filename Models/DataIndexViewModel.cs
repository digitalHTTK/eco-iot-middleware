using System;
using System.Collections.Generic;
using Plan_io_T.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Plan_io_T.Models {
    public class DataIndexViewModel {
        public IEnumerable<ArduinoData> ArduinoDatas { get; set; }
        public DataPageViewModel DataPageViewModel { get; set; }
        public DataFilterViewModel DataFilterViewModel { get; set; }
        public DataSortViewModel DataSortViewModel { get; set; }
    }
}
