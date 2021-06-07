using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace Plan_io_T.Library {
    public class SerialPortConnector {
        // Поэтому в админ-панели и прописывается номер порта и скорость
        private readonly int _baudRate = 115200;
        private readonly string _portName = "COM6";

        // Хардварный аналог сеттера 
        public void Send(string command) {
            try {
                using (var serialPort = new SerialPort(_portName, _baudRate)) {
                    serialPort.Open();
                    serialPort.Write(command);
                }
            }
            catch (Exception e) {
                
            }
        }

        public string Get() {
            try {
                using (var serialPort = new SerialPort(_portName, _baudRate)) {
                    serialPort.Open();
                    return serialPort.ReadLine();
            }
            }
            catch (Exception) {
                return "BAD";
            }
            
        }
    }
}
