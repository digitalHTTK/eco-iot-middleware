using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plan_io_T.Models {

    // Для сбора данных вполне подойдет анемичная модель, поведение
    // модели в рамках задачи определять не нужно
    public class ArduinoData {
        public int ID { get; set; }

        // Влажность ПОЧВЫ
        public int Moisture { get; set; }
        // Влажность ВОЗДУХА
        public int Humidity { get; set; }

        public int Temperature { get; set; }
        public int Illumination { get; set; }
    }

}
