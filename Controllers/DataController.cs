using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plan_io_T.Data;
using Plan_io_T.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Plan_io_T.Library;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Plan_io_T.Controllers {
    [Authorize(Roles = "admin, user")]
    public class DataController : Controller {

        private readonly MvcDataContext _context;
        private SerialPortConnector _serialPortConnector;
        static private string jsonString = "init";

        public DataController(MvcDataContext context) {
            _context = context;
            _serialPortConnector = new SerialPortConnector();
        }

        public IActionResult Send(string command) {
            try {
                _serialPortConnector.Send(command);
                return Ok("success");
            } catch (Exception err) {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        public IActionResult SendPost(string command) {
            try {
                _serialPortConnector.Send(command);
                return Ok("success");
            } catch (Exception err) {
                return BadRequest(err.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewRecord() {
            try {
                if (ModelState.IsValid) {

                    int[] mas = new int[4];
                    mas = await Task.Run(() => GetDataFromEmulator());

                    ArduinoData aData = new ArduinoData {
                        Humidity     = mas[0],
                        Moisture     = mas[1],
                        Temperature  = mas[2],
                        Illumination = mas[3]
                    };
                    _context.ArduinoData.Add(aData);
                    jsonString = JsonSerializer.Serialize(aData);
                    await _context.SaveChangesAsync();
                }
                return Ok("success");
            }
            catch (Exception) {
                return BadRequest("failed");
            }
        }

        [HttpGet("Data/SendDataToFront")]
        public string SendDataToFront() {
            return jsonString;
        }

        [HttpPost]
        public IActionResult ConectDevice() {
            _serialPortConnector.Send("EZ");
            return RedirectToAction("System");
        }

        [HttpPost]
        public async Task<IActionResult> System(string _device) {
            try {
                if (_device == "arduino") {
                    ViewData["ArduinoEcho"] = await Task.Run(() => GetEchoAnswer("A"));
                }
                else if (_device == "sensors") {
                    ViewData["SensorsEcho"]  = await Task.Run(() => GetEchoAnswer("M"));
                    ViewData["SensorsEcho"] += await Task.Run(() => GetEchoAnswer("H"));
                    ViewData["SensorsEcho"] += await Task.Run(() => GetEchoAnswer("T"));
                    ViewData["SensorsEcho"] += await Task.Run(() => GetEchoAnswer("L"));
                } 
                else if (_device == "relay") {
                    ViewData["RelayEcho"]  = await Task.Run(() => GetEchoAnswer("R"));
                    ViewData["RelayEcho"] += await Task.Run(() => GetEchoAnswer("Z"));
                }
                return View();
            } catch (Exception err) {
                return BadRequest("failed");
            }
        }

        private int[] GetDataFromEmulator() {

            // Запрос-ответ порта
            _serialPortConnector.Send("RH");
            string _h = _serialPortConnector.Get();
            _serialPortConnector.Send("RM");
            string _m = _serialPortConnector.Get();
            _serialPortConnector.Send("RT");
            string _t = _serialPortConnector.Get();
            _serialPortConnector.Send("RL");
            string _l = _serialPortConnector.Get();

            int[] _mas = new int[4];
            if (_h == "CLOSED" || _h == "BAD") {
                throw new Exception("Error: Port closed or something going wrong during the process.");
            }
            else {
                _mas[0] = int.Parse(_h);
                _mas[1] = int.Parse(_m);
                _mas[2] = int.Parse(_t);
                _mas[3] = int.Parse(_l);
            }
            
            return _mas;
        }

        private string GetEchoAnswer(string _deviceID) {
            _serialPortConnector.Send("E" + _deviceID);
            string temp = _serialPortConnector.Get();
            if (temp == "CLOSED" || temp == "BAD") {
                throw new Exception("Error: Port closed or something going wrong during the process.");
            }
            return temp;
        }

        public async Task<IActionResult> Dashboard() {
            var temp = await _context.ArduinoData.OrderByDescending(p => p.ID).FirstOrDefaultAsync();
            ViewData["Time"] = DateTime.Now.ToString("HH:mm tt");
            ViewData["Humidity"] = temp.Humidity;
            ViewData["Moisture"] = temp.Moisture;
            ViewData["Temperature"] = temp.Temperature;
            if (temp.Temperature < 10 || temp.Humidity < 20 || temp.Moisture < 20 || temp.Illumination < 20) ViewData["Score"] = "1";
            else if (temp.Temperature < 14 || temp.Humidity < 40 || temp.Moisture < 40 || temp.Illumination < 40) ViewData["Score"] = "2";
            else if (temp.Temperature < 16 || temp.Humidity < 60 || temp.Moisture < 60 || temp.Illumination < 60) ViewData["Score"] = "3";
            else if (temp.Temperature < 20 || temp.Humidity < 80 || temp.Moisture < 80 || temp.Illumination < 80) ViewData["Score"] = "4";
            else ViewData["Score"] = "5";
            return View();
        }

        public IActionResult System() {
            return View();
        }
    }
}
