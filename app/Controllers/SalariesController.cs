using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;
using System.Threading.Tasks;
using System.Linq;

namespace app.Controllers
{
    public class SalariesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly app.Services.IAuditService _auditService;
        public SalariesController(ApplicationDbContext context, app.Services.IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Salaries
        public async Task<IActionResult> Index()
        {
            var salaries = await _context.Salaries.Include(s => s.Employee).ToListAsync();
            return View(salaries);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View();
        }

        // POST: Salaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Microsoft.AspNetCore.Http.IFormCollection form)
        {
            var salary = new Salary();
            if (form.ContainsKey("EmpNo") && int.TryParse(form["EmpNo"], out var empNo))
                salary.EmpNo = empNo;

            // SalaryAmount comes as decimal string, convert to stored long (centavos)
            if (form.ContainsKey("SalaryAmount"))
            {
                var raw = form["SalaryAmount"].ToString();
                decimal salDec = 0;
                // Try invariant then current culture
                if (!decimal.TryParse(raw, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out salDec))
                {
                    decimal.TryParse(raw, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out salDec);
                }
                salary.SalaryAmount = (long)(salDec * 100);
            }

            salary.FromDate = form.ContainsKey("FromDate") ? form["FromDate"].ToString() : string.Empty;
            salary.ToDate = form.ContainsKey("ToDate") ? (string.IsNullOrWhiteSpace(form["ToDate"].ToString()) ? "9999-12-31" : form["ToDate"].ToString()) : "9999-12-31";

            // Validaciones
            if (salary.EmpNo == 0)
                ModelState.AddModelError("EmpNo", "Seleccione un empleado.");
            if (salary.SalaryAmount <= 0)
                ModelState.AddModelError("SalaryAmount", "El salario debe ser mayor a 0.");
            if (string.IsNullOrWhiteSpace(salary.FromDate))
                ModelState.AddModelError("FromDate", "La fecha de inicio es requerida.");

            if (ModelState.IsValid)
            {
                // Obtener salario previo para auditoría
                var prev = await _context.Salaries.Where(s => s.EmpNo == salary.EmpNo)
                    .OrderByDescending(s => s.FromDate)
                    .FirstOrDefaultAsync();

                _context.Salaries.Add(salary);
                await _context.SaveChangesAsync();

                // Log de auditoría (usar nombre de usuario si está disponible)
                try
                {
                    var userName = User?.Identity?.Name ?? "Sistema";
                    System.Diagnostics.Debug.WriteLine($"Logging salary creation for EmpNo: {salary.EmpNo}, Prev: {prev?.SalaryAmount ?? 0L}, New: {salary.SalaryAmount}");
                    await _auditService.LogSalaryChangeAsync(salary.EmpNo, prev?.SalaryAmount ?? 0L, salary.SalaryAmount, userName, "Registro de salario");
                    
                    // Debug: Verificar que se creó la entrada
                    var auditCount = await _context.AuditLogs.CountAsync();
                    System.Diagnostics.Debug.WriteLine($"Total audit logs after create: {auditCount}");
                }
                catch (Exception ex)
                {
                    // No detener el flujo si falla el logging
                    System.Diagnostics.Debug.WriteLine($"Error en auditoría create: {ex.Message}");
                    TempData["InfoMessage"] = $"Salario creado, pero no se pudo registrar en auditoría: {ex.Message}";
                }

                TempData["SuccessMessage"] = "Salario guardado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Employees = _context.Employees.ToList();
            return View(salary);
        }

        // GET: Salaries/Edit?empNo=1&fromDate=2020-01-01
        public async Task<IActionResult> Edit(int empNo, string fromDate)
        {
            var salary = await _context.Salaries.Include(s => s.Employee).FirstOrDefaultAsync(s => s.EmpNo == empNo && s.FromDate == fromDate);
            if (salary == null)
                return NotFound();

            ViewBag.Employees = _context.Employees.ToList();
            return View(salary);
        }

        // POST: Salaries/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int empNo, string fromDate, Microsoft.AspNetCore.Http.IFormCollection form)
        {
            try
            {
                var salary = await _context.Salaries.FirstOrDefaultAsync(s => s.EmpNo == empNo && s.FromDate == fromDate);
                if (salary == null)
                {
                    TempData["ErrorMessage"] = "Salario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var oldAmount = salary.SalaryAmount;

                // parse new amount
                if (form.ContainsKey("SalaryAmount"))
                {
                    var raw = form["SalaryAmount"].ToString();
                    decimal salDec = 0;
                    if (!decimal.TryParse(raw, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out salDec))
                    {
                        decimal.TryParse(raw, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out salDec);
                    }
                    salary.SalaryAmount = (long)(salDec * 100);
                }

                salary.ToDate = form.ContainsKey("ToDate") ? (string.IsNullOrWhiteSpace(form["ToDate"].ToString()) ? "9999-12-31" : form["ToDate"].ToString()) : salary.ToDate;

                _context.Salaries.Update(salary);
                await _context.SaveChangesAsync();

                try
                {
                    var userName = User?.Identity?.Name ?? "Sistema";
                    System.Diagnostics.Debug.WriteLine($"Logging salary change for EmpNo: {salary.EmpNo}, Old: {oldAmount}, New: {salary.SalaryAmount}");
                    await _auditService.LogSalaryChangeAsync(salary.EmpNo, oldAmount, salary.SalaryAmount, userName, "Edición de salario");
                    
                    // Debug: Verificar que se creó la entrada
                    var auditCount = await _context.AuditLogs.CountAsync();
                    System.Diagnostics.Debug.WriteLine($"Total audit logs after edit: {auditCount}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en auditoría edit: {ex.Message}");
                    TempData["InfoMessage"] = $"Salario actualizado, pero no se pudo registrar en auditoría: {ex.Message}";
                }

                TempData["SuccessMessage"] = "Salario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al actualizar el salario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Salaries/Delete?empNo=1&fromDate=2020-01-01
        public async Task<IActionResult> Delete(int empNo, string fromDate)
        {
            var salary = await _context.Salaries.Include(s => s.Employee).FirstOrDefaultAsync(s => s.EmpNo == empNo && s.FromDate == fromDate);
            if (salary == null)
                return NotFound();

            return View(salary);
        }

        // POST: Salaries/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int EmpNo, string FromDate)
        {
            var salary = await _context.Salaries.FirstOrDefaultAsync(s => s.EmpNo == EmpNo && s.FromDate == FromDate);
            if (salary != null)
            {
                var oldAmount = salary.SalaryAmount;
                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
                try
                {
                    var userName = User?.Identity?.Name ?? "Sistema";
                    await _auditService.LogGenericChangeAsync(salary.EmpNo, userName, $"Eliminación de salario (antes: {oldAmount / 100m:C})", 0);
                }
                catch { }
            }
            TempData["SuccessMessage"] = "Salario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
