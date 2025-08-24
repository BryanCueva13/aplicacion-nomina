using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;
using app.ViewModels;

namespace app.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Payroll()
        {
            ViewData["Title"] = "Reporte de Nómina";
            
            // Obtener todos los empleados
            var employees = await _context.Employees.ToListAsync();
            
            var payrollData = new List<PayrollReportViewModel>();
            
            foreach (var employee in employees)
            {
                // Obtener departamento actual
                var currentDept = _context.DepartmentEmployees
                    .Include(de => de.Department)
                    .Where(de => de.EmpNo == employee.EmpNo)
                    .AsEnumerable()
                    .Where(de => de.ToDate == "9999-01-01" || de.ToDate == "9999-12-31" || string.IsNullOrEmpty(de.ToDate))
                    .OrderByDescending(de => de.FromDate)
                    .FirstOrDefault();

                // Obtener título actual
                var currentTitle = _context.Titles
                    .Where(t => t.EmpNo == employee.EmpNo)
                    .AsEnumerable()
                    .Where(t => t.ToDate == null || t.ToDate == "9999-01-01" || t.ToDate == "9999-12-31" || string.IsNullOrEmpty(t.ToDate))
                    .OrderByDescending(t => t.FromDate)
                    .FirstOrDefault();

                // Obtener salario actual
                var currentSalary = _context.Salaries
                    .Where(s => s.EmpNo == employee.EmpNo)
                    .AsEnumerable()
                    .Where(s => s.ToDate == "9999-01-01" || s.ToDate == "9999-12-31" || string.IsNullOrEmpty(s.ToDate))
                    .OrderByDescending(s => s.FromDate)
                    .FirstOrDefault();

                payrollData.Add(new PayrollReportViewModel
                {
                    EmpNo = employee.EmpNo,
                    CI = employee.CI,
                    FullName = $"{employee.FirstName} {employee.LastName}",
                    Department = currentDept?.Department?.DeptName ?? "Sin Departamento",
                    Title = currentTitle?.TitleName ?? "Sin Título",
                    CurrentSalary = currentSalary?.SalaryAmount ?? 0,
                    HireDate = employee.HireDate
                });
            }

            // Ordenar por departamento y luego por nombre
            payrollData = payrollData
                .OrderBy(p => p.Department)
                .ThenBy(p => p.FullName)
                .ToList();

            var totalPayroll = payrollData.Sum(p => p.CurrentSalary);
            ViewBag.TotalPayroll = totalPayroll;
            ViewBag.EmployeeCount = payrollData.Count;

            return View(payrollData);
        }

        public async Task<IActionResult> Organizational()
        {
            ViewData["Title"] = "Estructura Organizacional";

            // Obtener todos los departamentos
            var departments = await _context.Departments.ToListAsync();
            
            var organizationalData = new List<OrganizationalReportViewModel>();

            foreach (var department in departments)
            {
                // Obtener gerente actual
                var currentManager = _context.DepartmentManagers
                    .Include(dm => dm.Employee)
                    .Where(dm => dm.DeptNo == department.DeptNo)
                    .AsEnumerable()
                    .Where(dm => dm.ToDate == "9999-01-01" || dm.ToDate == "9999-12-31" || string.IsNullOrEmpty(dm.ToDate))
                    .OrderByDescending(dm => dm.FromDate)
                    .FirstOrDefault();

                // Obtener empleados actuales del departamento
                var currentEmployees = _context.DepartmentEmployees
                    .Include(de => de.Employee)
                    .Where(de => de.DeptNo == department.DeptNo)
                    .AsEnumerable()
                    .Where(de => de.ToDate == "9999-01-01" || de.ToDate == "9999-12-31" || string.IsNullOrEmpty(de.ToDate))
                    .ToList();

                var employeeInfos = new List<EmployeeInfo>();
                foreach (var empAssignment in currentEmployees)
                {
                    // Obtener título actual del empleado
                    var currentTitle = _context.Titles
                        .Where(t => t.EmpNo == empAssignment.EmpNo)
                        .AsEnumerable()
                        .Where(t => t.ToDate == null || t.ToDate == "9999-01-01" || t.ToDate == "9999-12-31" || string.IsNullOrEmpty(t.ToDate))
                        .OrderByDescending(t => t.FromDate)
                        .FirstOrDefault();

                    employeeInfos.Add(new EmployeeInfo
                    {
                        EmpNo = empAssignment.Employee.EmpNo,
                        FullName = $"{empAssignment.Employee.FirstName} {empAssignment.Employee.LastName}",
                        Title = currentTitle?.TitleName ?? "Sin Título",
                        FromDate = empAssignment.FromDate
                    });
                }

                organizationalData.Add(new OrganizationalReportViewModel
                {
                    DeptNo = department.DeptNo,
                    DepartmentName = department.DeptName,
                    Manager = currentManager != null ? new ManagerInfo
                    {
                        EmpNo = currentManager.Employee.EmpNo,
                        FullName = $"{currentManager.Employee.FirstName} {currentManager.Employee.LastName}",
                        FromDate = currentManager.FromDate
                    } : null,
                    Employees = employeeInfos.OrderBy(e => e.FullName).ToList(),
                    EmployeeCount = employeeInfos.Count
                });
            }

            // Ordenar por nombre de departamento
            organizationalData = organizationalData
                .OrderBy(d => d.DepartmentName)
                .ToList();

            var totalEmployees = organizationalData.Sum(d => d.EmployeeCount);
            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.DepartmentCount = organizationalData.Count;

            return View(organizationalData);
        }

        [HttpPost]
        public async Task<IActionResult> ExportPayrollToCsv()
        {
            // Obtener todos los empleados
            var employees = await _context.Employees.ToListAsync();
            
            var payrollData = new List<object>();
            
            foreach (var employee in employees)
            {
                // Obtener departamento actual
                var currentDept = _context.DepartmentEmployees
                    .Include(de => de.Department)
                    .Where(de => de.EmpNo == employee.EmpNo)
                    .AsEnumerable()
                    .Where(de => de.ToDate == "9999-01-01" || de.ToDate == "9999-12-31" || string.IsNullOrEmpty(de.ToDate))
                    .OrderByDescending(de => de.FromDate)
                    .FirstOrDefault();

                // Obtener título actual
                var currentTitle = _context.Titles
                    .Where(t => t.EmpNo == employee.EmpNo)
                    .AsEnumerable()
                    .Where(t => t.ToDate == null || t.ToDate == "9999-01-01" || t.ToDate == "9999-12-31" || string.IsNullOrEmpty(t.ToDate))
                    .OrderByDescending(t => t.FromDate)
                    .FirstOrDefault();

                // Obtener salario actual
                var currentSalary = _context.Salaries
                    .Where(s => s.EmpNo == employee.EmpNo)
                    .AsEnumerable()
                    .Where(s => s.ToDate == "9999-01-01" || s.ToDate == "9999-12-31" || string.IsNullOrEmpty(s.ToDate))
                    .OrderByDescending(s => s.FromDate)
                    .FirstOrDefault();

                payrollData.Add(new
                {
                    NumeroEmpleado = employee.EmpNo,
                    CI = employee.CI,
                    NombreCompleto = $"{employee.FirstName} {employee.LastName}",
                    Departamento = currentDept?.Department?.DeptName ?? "Sin Departamento",
                    Titulo = currentTitle?.TitleName ?? "Sin Título",
                    SalarioActual = (currentSalary?.SalaryAmount ?? 0) / 100m, // Convertir a dólares
                    FechaContratacion = employee.HireDate
                });
            }

            // Ordenar por departamento y nombre
            payrollData = payrollData
                .OrderBy(p => ((dynamic)p).Departamento)
                .ThenBy(p => ((dynamic)p).NombreCompleto)
                .ToList();

            var csv = "Numero Empleado,CI,Nombre Completo,Departamento,Titulo,Salario Actual (USD),Fecha Contratacion\n";
            foreach (dynamic item in payrollData)
            {
                csv += $"{item.NumeroEmpleado},{item.CI},\"{item.NombreCompleto}\",\"{item.Departamento}\",\"{item.Titulo}\",{item.SalarioActual:F2},{item.FechaContratacion}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"nomina_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
