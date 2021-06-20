using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plan_io_T.Data;
using Plan_io_T.Models;
using Plan_io_T.ViewModels;

// Не очень правильный с точки зрения логики CRUD, но в целом удобный коллекционер
// данных мониторинга
namespace Plan_io_T.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class ArduinoDatasController : Controller
    {
        private readonly MvcDataContext _context;

        public ArduinoDatasController(MvcDataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int nodeId, string date, int page = 1,
            DataSortState sortOrder = DataSortState.idAsc)
        {
            int pageSize = 30;

            IQueryable<ArduinoData> arduinoDatas = _context.ArduinoData;
            if (nodeId != 0) {
                arduinoDatas = arduinoDatas.Where(p => p.NodeID == nodeId);
            }
            if (!String.IsNullOrEmpty(date)) {
                arduinoDatas = arduinoDatas.Where(p => p.Date.Contains(date));
            }

            switch (sortOrder) {
                case DataSortState.idDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.ID);
                    break;
                case DataSortState.dateAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.Date);
                    break;
                case DataSortState.dateDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.Date);
                    break;
                case DataSortState.timeAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.Time);
                    break;
                case DataSortState.timeDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.Time);
                    break;
                case DataSortState.nodeIdAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.NodeID);
                    break;
                case DataSortState.nodeIdDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.NodeID);
                    break;
                case DataSortState.coAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.coConcentration);
                    break;
                case DataSortState.coDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.coConcentration);
                    break;
                case DataSortState.co2Asc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.co2Concentration);
                    break;
                case DataSortState.co2Desc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.co2Concentration);
                    break;
                case DataSortState.tempAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.Temperature);
                    break;
                case DataSortState.tempDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.Temperature);
                    break;
                case DataSortState.humAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.Humidity);
                    break;
                case DataSortState.humDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.Humidity);
                    break;
                case DataSortState.lpgAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.lpgConcentration);
                    break;
                case DataSortState.lpgDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.lpgConcentration);
                    break;
                case DataSortState.smkAsc:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.smokeConcentration);
                    break;
                case DataSortState.smkDesc:
                    arduinoDatas = arduinoDatas.OrderByDescending(s => s.smokeConcentration);
                    break;
                default:
                    arduinoDatas = arduinoDatas.OrderBy(s => s.ID);
                    break;
            }

            var count = await arduinoDatas.CountAsync();
            var items = await arduinoDatas.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            DataIndexViewModel viewModel = new DataIndexViewModel {
                DataPageViewModel = new DataPageViewModel(count, page, pageSize),
                DataSortViewModel = new DataSortViewModel(sortOrder),
                DataFilterViewModel = new DataFilterViewModel(nodeId, date),
                ArduinoDatas = items
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arduinoData = await _context.ArduinoData
                .FirstOrDefaultAsync(m => m.ID == id);
            if (arduinoData == null)
            {
                return NotFound();
            }

            return View(arduinoData);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Temperature,Humidity,co2Concentration,coConcentration,lpgConcentration,smokeConcentration")] ArduinoData arduinoData)
        {
            if (ModelState.IsValid)
            {
                DateTime dateTime = DateTime.Now;
                arduinoData.Date = dateTime.ToString("dd.MM.yyyy");
                arduinoData.Time = dateTime.ToString("HH:mm:ss");
                arduinoData.NodeID = 0;
                _context.Add(arduinoData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(arduinoData);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arduinoData = await _context.ArduinoData.FindAsync(id);
            if (arduinoData == null)
            {
                return NotFound();
            }
            return View(arduinoData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Temperature,Humidity,co2Concentration,coConcentration,lpgConcentration,smokeConcentration")] ArduinoData arduinoData)
        {
            if (id != arduinoData.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arduinoData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArduinoDataExists(arduinoData.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(arduinoData);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arduinoData = await _context.ArduinoData
                .FirstOrDefaultAsync(m => m.ID == id);
            if (arduinoData == null)
            {
                return NotFound();
            }

            return View(arduinoData);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arduinoData = await _context.ArduinoData.FindAsync(id);
            _context.ArduinoData.Remove(arduinoData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArduinoDataExists(int id)
        {
            return _context.ArduinoData.Any(e => e.ID == id);
        }
    }
}
