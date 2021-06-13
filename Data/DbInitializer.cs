using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plan_io_T.Models;

namespace Plan_io_T.Data {
    public class DbInitializer {
        public static void Initialize(MvcDataContext context) {
            context.Database.EnsureCreated();

            if (context.ArduinoData.Any()) {
                return;   
            }

            DateTime dateTime = DateTime.Now;
            var aData = new ArduinoData[]
            {
            new ArduinoData {
                Date = dateTime.ToString("dd.MM.yyyy"),
                Time = dateTime.ToString("HH:mm:ss"),
                NodeID = 0,
                Temperature = 0,
                Humidity = 0,
                co2Concentration = 0,
                coConcentration = 0,
                smokeConcentration = 0,
                lpgConcentration = 0
            },
            new ArduinoData {
                Date = dateTime.ToString("dd.MM.yyyy"),
                Time = dateTime.ToString("HH:mm:ss"),
                NodeID = 0,
                Temperature = 1,
                Humidity = 1,
                co2Concentration = 1,
                coConcentration = 1,
                smokeConcentration = 1,
                lpgConcentration = 1
            },
            new ArduinoData {
                Date = dateTime.ToString("dd.MM.yyyy"),
                Time = dateTime.ToString("HH:mm:ss"),
                NodeID = 0,
                Temperature = 2,
                Humidity = 2,
                co2Concentration = 2,
                coConcentration = 2,
                smokeConcentration = 2,
                lpgConcentration = 2
            }
            };

            foreach (ArduinoData a in aData) {
                context.ArduinoData.Add(a);
            }
            context.SaveChanges();
        }
    }
}
