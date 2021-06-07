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

        public async Task<IActionResult> Index()
        {
            return View(await _context.ArduinoData.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("ID,Moisture,Humidity,Temperature,Illumination")] ArduinoData arduinoData)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Moisture,Humidity,Temperature,Illumination")] ArduinoData arduinoData)
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
