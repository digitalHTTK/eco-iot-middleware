using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plan_io_T.Models {
    public class ArduinoDataWithPOCOs {
        public int NodeNum { get; set; }
        public int PublishData { get; set; }
        public int temp { get; set; }
        public int hum { get; set; }
        public int co { get; set; }
        public int co2 { get; set; }
        public int lpg { get; set; }
        public int smk { get; set; }
    }

    public class NodesThatContainsData {
        public int nodeId { get; set; }
        public string time { get; set; }
        public ArduinoDataWithPOCOs nodeData { get; set; }
    }

    public class NodesThatContainsDataLists {
        public int nodeId { get; set; }
        public string time { get; set; }
        public List<ArduinoDataWithPOCOs> nodeData { get; set; }
    }
}
