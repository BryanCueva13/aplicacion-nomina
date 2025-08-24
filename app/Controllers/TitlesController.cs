using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;
using System.Threading.Tasks;
using System.Linq;

namespace app.Controllers
{
    public class TitlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly app.Services.IAuditService _auditService;
        public TitlesController(ApplicationDbContext context, app.Services.IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Titles
        public async Task<IActionResult> Index()
        {
            var titles = await _context.Titles.Include(t => t.Employee).ToListAsync();
            return View(titles);
        }

        // GET: Titles/Create
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }

        // POST: Titles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Microsoft.AspNetCore.Http.IFormCollection form)
        {
            // Leer desde el formulario para evitar problemas de binding
            var title = new Title();
            if (form.ContainsKey("EmpNo") && int.TryParse(form["EmpNo"], out var empNo))
                title.EmpNo = empNo;
            title.TitleName = form.ContainsKey("TitleName") ? form["TitleName"].ToString() : string.Empty;
            title.FromDate = form.ContainsKey("FromDate") ? form["FromDate"].ToString() : string.Empty;
            title.ToDate = form.ContainsKey("ToDate") ? (string.IsNullOrWhiteSpace(form["ToDate"].ToString()) ? null : form["ToDate"].ToString()) : null;

            // Validaciones simples
            if (title.EmpNo == 0)
                ModelState.AddModelError("EmpNo", "Seleccione un empleado.");
            if (string.IsNullOrWhiteSpace(title.TitleName))
                ModelState.AddModelError("TitleName", "El título es requerido.");
            if (string.IsNullOrWhiteSpace(title.FromDate))
                ModelState.AddModelError("FromDate", "La fecha de inicio es requerida.");

            if (ModelState.IsValid)
            {
                _context.Titles.Add(title);
                await _context.SaveChangesAsync();
                try
                {
                    var userName = User?.Identity?.Name ?? "Sistema";
                    await _auditService.LogGenericChangeAsync(title.EmpNo, userName, $"Creación de título: {title.TitleName}");
                }
                catch { }
                TempData["SuccessMessage"] = "Título/Cargo guardado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Employees = _context.Employees.ToList();
            return View(title);
        }

        // GET: Titles/Edit?empNo=1&fromDate=2020-01-01&titleName=Developer
        public async Task<IActionResult> Edit(int empNo, string fromDate, string titleName)
        {
            var title = await _context.Titles.FirstOrDefaultAsync(t => t.EmpNo == empNo && t.FromDate == fromDate && t.TitleName == titleName);
            if (title == null)
                return NotFound();

            ViewBag.Employees = _context.Employees.ToList();
            return View(title);
        }

        // POST: Titles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int empNo, string fromDate, string titleName, Microsoft.AspNetCore.Http.IFormCollection form)
        {
            try
            {
                var title = await _context.Titles.FirstOrDefaultAsync(t => t.EmpNo == empNo && t.FromDate == fromDate && t.TitleName == titleName);
                if (title == null)
                {
                    TempData["ErrorMessage"] = "Título no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Solo podemos actualizar ToDate ya que TitleName y FromDate son parte de la clave
                var newToDate = form.ContainsKey("ToDate") ? form["ToDate"].ToString() : string.Empty;
                title.ToDate = string.IsNullOrWhiteSpace(newToDate) ? null : newToDate;

                _context.Titles.Update(title);
                await _context.SaveChangesAsync();
                try
                {
                    var userName = User?.Identity?.Name ?? "Sistema";
                    await _auditService.LogGenericChangeAsync(title.EmpNo, userName, $"Edición de título: {title.TitleName} (fecha hasta: {title.ToDate})");
                }
                catch { }
                TempData["SuccessMessage"] = "Título/Cargo actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar el título: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Titles/Delete
        public async Task<IActionResult> Delete(int? empNo, string? fromDate, string? titleName)
        {
            if (empNo == null)
                return NotFound();

            var title = await _context.Titles.Include(t => t.Employee).FirstOrDefaultAsync(t => t.EmpNo == empNo && t.FromDate == fromDate && t.TitleName == titleName);
            if (title == null)
                return NotFound();

            return View(title);
        }

        // POST: Titles/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int EmpNo, string FromDate, string TitleName)
        {
            var title = await _context.Titles.FirstOrDefaultAsync(t => t.EmpNo == EmpNo && t.FromDate == FromDate && t.TitleName == TitleName);
            if (title != null)
            {
                _context.Titles.Remove(title);
                await _context.SaveChangesAsync();
                try
                {
                    var userName = User?.Identity?.Name ?? "Sistema";
                    await _auditService.LogGenericChangeAsync(title.EmpNo, userName, $"Eliminación de título: {title.TitleName}");
                }
                catch { }
            }
            TempData["SuccessMessage"] = "Título/Cargo eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
