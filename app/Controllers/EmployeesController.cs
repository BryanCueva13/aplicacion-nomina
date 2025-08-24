using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;

namespace app.Controllers
{
    /// <summary>
    /// Controlador para gestión de empleados
    /// </summary>
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de empleados
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.User)
                .OrderBy(e => e.EmpNo)
                .ToListAsync();

            return View(employees);
        }

        /// <summary>
        /// Ver detalles de un empleado
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmpNo == id);

            if (employee == null)
            {
                return NotFound();
            }


            // Departamento actual (asignación activa)
            var deptEmp = _context.DepartmentEmployees
                .Include(de => de.Department)
                .Where(de => de.EmpNo == id)
                .AsEnumerable() // Traer a memoria para filtrar por fecha
                .Where(de => de.ToDate == "9999-12-31" || string.IsNullOrEmpty(de.ToDate) || DateTime.Parse(de.ToDate) > DateTime.Now)
                .OrderByDescending(de => de.FromDate)
                .FirstOrDefault();
            ViewBag.CurrentDepartment = deptEmp?.Department?.DeptName;

            // ¿Es gerente?
            var isManager = _context.DepartmentManagers
                .Where(dm => dm.EmpNo == id)
                .AsEnumerable()
                .Any(dm => dm.ToDate == "9999-12-31" || string.IsNullOrEmpty(dm.ToDate) || DateTime.Parse(dm.ToDate) > DateTime.Now);
            ViewBag.IsManager = isManager;

            return View(employee);
        }

        /// <summary>
        /// Formulario para crear nuevo empleado
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments.ToList();
            var employee = new Employee
            {
                HireDate = DateTime.Now.ToString("yyyy-MM-dd"),
                BirthDate = DateTime.Now.AddYears(-25).ToString("yyyy-MM-dd"),
                Gender = "M"
            };
            return View(employee);
        }

        /// <summary>
        /// Procesar creación de empleado
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            int selectedDeptNo = 0;
            if (Request.Form.ContainsKey("SelectedDeptNo"))
            {
                int.TryParse(Request.Form["SelectedDeptNo"], out selectedDeptNo);
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                ViewBag.SelectedDeptNo = selectedDeptNo;
                return View(employee);
            }

            try
            {
                // Generar nuevo número de empleado
                var maxEmpNo = await _context.Employees.MaxAsync(e => (int?)e.EmpNo) ?? 1000;
                employee.EmpNo = maxEmpNo + 1;

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Si se seleccionó un departamento, crear la asignación
                if (selectedDeptNo > 0)
                {
                    var today = DateTime.Now.ToString("yyyy-MM-dd");
                    var deptEmp = new DepartmentEmployee
                    {
                        EmpNo = employee.EmpNo,
                        DeptNo = selectedDeptNo,
                        FromDate = today,
                        ToDate = "9999-12-31"
                    };
                    _context.DepartmentEmployees.Add(deptEmp);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Empleado {employee.FirstName} {employee.LastName} creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.Departments = _context.Departments.ToList();
                ModelState.AddModelError(string.Empty, "Error al crear el empleado. Inténtalo nuevamente.");
                return View(employee);
            }
        }

        /// <summary>
        /// Formulario para editar empleado
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmpNo == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        /// <summary>
        /// Procesar edición de empleado
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmpNo)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Empleado {employee.FirstName} {employee.LastName} actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar el empleado. Inténtalo nuevamente.");
                return View(employee);
            }
        }

        /// <summary>
        /// Confirmación para eliminar empleado
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmpNo == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        /// <summary>
        /// Procesar eliminación de empleado
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmpNo == id);

            if (employee == null)
            {
                return NotFound();
            }

            try
            {
                // Eliminar usuario asociado si existe
                if (employee.User != null)
                {
                    _context.Users.Remove(employee.User);
                }

                // Eliminar empleado
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Empleado {employee.FirstName} {employee.LastName} eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al eliminar el empleado. Puede que tenga registros relacionados.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
