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
using MQTTnet;
using MQTTnet.Client.Options;
using System.Threading;
using MQTTnet.Client;
using System.Text;
using MQTTnet.Protocol;

namespace Plan_io_T.Controllers {
    [Authorize(Roles = "admin, user")]
    public class DataController : Controller {

        private readonly MvcDataContext _context;
        private SerialPortConnector _serialPortConnector;
        static private string jsonString = "init";
        static private MqttFactory factory;
        static private IMqttClient mqttClient;
        static private bool isMqttConnected = false;
        static private ArduinoData aData;

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
                    _context.ArduinoData.Add(aData);
                    await _context.SaveChangesAsync();
                }
                return Ok("success");
            } catch (Exception) {
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
                    ViewData["ArduinoEcho"] = await Task.Run(() => GetEchoAnswer("M"));
                } else if (_device == "sensors") {
                    ViewData["SensorsEcho"] = await Task.Run(() => GetEchoAnswer("M"));
                    ViewData["SensorsEcho"] += await Task.Run(() => GetEchoAnswer("H"));
                    ViewData["SensorsEcho"] += await Task.Run(() => GetEchoAnswer("T"));
                    ViewData["SensorsEcho"] += await Task.Run(() => GetEchoAnswer("L"));
                } else if (_device == "relay") {
                    ViewData["RelayEcho"] = await Task.Run(() => GetEchoAnswer("R"));
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
            } else {
                _mas[0] = int.Parse(_h);
                _mas[1] = int.Parse(_m);
                _mas[2] = int.Parse(_t);
                _mas[3] = int.Parse(_l);
            }

            return _mas;
        }

        private string GetEchoAnswer(string _deviceID) {
            string temp = "Ok";
            SendMQTT("OK?");
            return temp;
        }

        public static async Task SubscribeAsync(string topic, int qos = 1) =>
            await mqttClient.SubscribeAsync(new TopicFilterBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .Build());

        private async void SetMQTT() {
            factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1", 1883)
                .Build();

            mqttClient.UseConnectedHandler(e => {
                isMqttConnected = true;
                Console.WriteLine("Connected successfully with MQTT Broker.");
            });

            mqttClient.UseDisconnectedHandler(e => {
                isMqttConnected = false;
                Console.WriteLine("Disconnected from MQTT Broker.");
            });

            mqttClient.UseApplicationMessageReceivedHandler(e => {
                try {
                    string topic = e.ApplicationMessage.Topic;

                    if (string.IsNullOrWhiteSpace(topic) == false) {
                        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                        PushDataFromNodes(payload);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message, ex);
                }
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None);

            await SubscribeAsync("ecoiot/from/+");

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("ecoiot/to")
                .WithPayload("READY")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);
        }

        [HttpPost]
        private async void PushDataFromNodes(string msg) {
            var optionsBuilder = new DbContextOptionsBuilder<MvcDataContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MvcDataContext;Trusted_Connection=True;MultipleActiveResultSets=true");

            using (var dataContext = new MvcDataContext(optionsBuilder.Options)) {
                ArduinoDataWithPOCOs data = JsonSerializer.Deserialize<ArduinoDataWithPOCOs>(msg);
                DateTime dateTime = DateTime.Now;
                ArduinoData arduinoData = new ArduinoData {
                    Date = dateTime.ToString("dd.MM.yyyy"),
                    Time = dateTime.ToString("HH:mm:ss"),
                    NodeID = data.NodeNum,
                    Temperature = data.temp,
                    Humidity = data.hum,
                    co2Concentration = data.co2,
                    coConcentration = data.co,
                    smokeConcentration = data.smk,
                    lpgConcentration = data.lpg
                };
                aData = arduinoData;
                try {
                    if (ModelState.IsValid) {
                        dataContext.ArduinoData.Add(arduinoData);
                        await dataContext.SaveChangesAsync();
                    }
                } catch (Exception err) {
                    Console.WriteLine(err.Message);
                }
            }
        }

        [HttpPost]
        private async void SendMQTT(string msg) {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("ecoiot/to")
                .WithPayload(msg)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);
        }

        public async Task<IActionResult> Dashboard() {
            if (!isMqttConnected) SetMQTT();
            var temp = await _context.ArduinoData.OrderByDescending(p => p.ID).FirstOrDefaultAsync();
            ViewData["Time"] = DateTime.Now.ToString("HH:mm tt");
            ViewData["Humidity"] = temp.Humidity;
            ViewData["co2Concentration"] = temp.co2Concentration;
            ViewData["Temperature"] = temp.Temperature;
            if (temp.Temperature < 10 || temp.Humidity < 20) ViewData["Score"] = "1";
            else if (temp.Temperature < 14 || temp.Humidity < 40) ViewData["Score"] = "2";
            else if (temp.Temperature < 16 || temp.Humidity < 60) ViewData["Score"] = "3";
            else if (temp.Temperature < 20 || temp.Humidity < 80) ViewData["Score"] = "4";
            else ViewData["Score"] = "5";
            return View();
        }

        public IActionResult System() {
            return View();
        }


    }
}
