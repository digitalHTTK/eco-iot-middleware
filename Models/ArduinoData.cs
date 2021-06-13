using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Plan_io_T.Models {

    // Для сбора данных вполне подойдет анемичная модель, поведение
    // модели в рамках задачи определять не нужно
    public class ArduinoData {
        public int ID { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public int NodeID { get; set; }

        public int Humidity { get; set; }

        public int Temperature { get; set; }

        public int co2Concentration { get; set; }

        public int coConcentration { get; set; }

        public int lpgConcentration { get; set; }

        public int smokeConcentration { get; set; }
    }

}
