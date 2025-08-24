using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using app.Data;
using app.Services;
using Microsoft.EntityFrameworkCore;

namespace app.Controllers
{
    /// <summary>
    /// Controlador principal para el dashboard
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public HomeController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        /// <summary>
        /// Dashboard principal
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardViewModel();

            // Obtener estadísticas generales
            dashboardData.TotalEmployees = await _context.Employees.CountAsync();
            dashboardData.TotalDepartments = await _context.Departments.CountAsync();
            dashboardData.ActiveUsers = await _context.Users.CountAsync();

            // Empleados contratados este mes
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            
            // Obtener todos los empleados y filtrar en memoria
            var allEmployees = await _context.Employees.ToListAsync();
            dashboardData.NewEmployeesThisMonth = allEmployees
                .Where(e => e.HireDatetime.HasValue && 
                           e.HireDatetime.Value.Month == currentMonth && 
                           e.HireDatetime.Value.Year == currentYear)
                .Count();

            // Últimos cambios de salario (auditoría)
            dashboardData.RecentSalaryChanges = await _auditService.GetRecentAuditLogsAsync(10);

            // Empleados por departamento (usando ToDateTime para poder consultar)
            dashboardData.EmployeesByDepartment = await _context.DepartmentEmployees
                .Include(de => de.Department)
                .Where(de => de.ToDate == "9999-12-31") // Asignaciones activas
                .GroupBy(de => de.Department.DeptName)
                .Select(g => new EmployeesByDepartmentItem
                {
                    DepartmentName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Próximos empleados con aniversario (obtener todos y filtrar en memoria)
            var thirtyDaysFromNow = DateTime.Now.AddDays(30);
            var employeesForAnniversary = await _context.Employees.ToListAsync();
            dashboardData.UpcomingAnniversaries = employeesForAnniversary
                .Where(e => e.HireDatetime.HasValue &&
                           e.HireDatetime.Value.Month >= DateTime.Now.Month &&
                           e.HireDatetime.Value.Month <= thirtyDaysFromNow.Month)
                .Select(e => new UpcomingAnniversaryItem
                {
                    EmployeeName = e.FullName,
                    HireDate = e.HireDatetime!.Value,
                    YearsOfService = DateTime.Now.Year - e.HireDatetime.Value.Year
                })
                .OrderBy(e => e.HireDate.Month)
                .ThenBy(e => e.HireDate.Day)
                .Take(10)
                .ToList();

            return View(dashboardData);
        }

        /// <summary>
        /// Página de error
        /// </summary>
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Información sobre la aplicación
        /// </summary>
        public IActionResult About()
        {
            return View();
        }
    }

    /// <summary>
    /// ViewModel para el dashboard
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int ActiveUsers { get; set; }
        public int NewEmployeesThisMonth { get; set; }
        public List<app.Models.AuditLog> RecentSalaryChanges { get; set; } = new();
        public List<EmployeesByDepartmentItem> EmployeesByDepartment { get; set; } = new();
        public List<UpcomingAnniversaryItem> UpcomingAnniversaries { get; set; } = new();
    }

    public class EmployeesByDepartmentItem
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class UpcomingAnniversaryItem
    {
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public int YearsOfService { get; set; }
    }
}
