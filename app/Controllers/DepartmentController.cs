using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;

namespace app.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeptNo,DeptName")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Department/Assignments/{id}
        public async Task<IActionResult> Assignments(int id)
        {
            var assignments = await _context.DepartmentEmployees
                .Include(de => de.Employee)
                .Where(de => de.DeptNo == id)
                .ToListAsync();
            ViewBag.Department = await _context.Departments.FindAsync(id);
            return View(assignments);
        }
        
            // GET: Department/AddEmployee/{deptNo}
            public IActionResult AddEmployee(int deptNo)
            {
                var department = _context.Departments.Find(deptNo);
                if (department == null) return NotFound();
                var empleadosNoAsignados = _context.Employees
                    .Where(e => !_context.DepartmentEmployees.Any(de => de.EmpNo == e.EmpNo && de.DeptNo == deptNo && (de.ToDate == "9999-12-31" || string.IsNullOrEmpty(de.ToDate))))
                    .ToList();
                ViewBag.Department = department;
                ViewBag.Employees = empleadosNoAsignados;
                return View();
            }

            // POST: Department/AddEmployee/{deptNo}
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddEmployee(int deptNo, int empNo, string fromDate, string toDate)
            {
                var department = await _context.Departments.FindAsync(deptNo);
                var employee = await _context.Employees.FindAsync(empNo);
                if (department == null || employee == null)
                {
                    return NotFound();
                }
                var assignment = new DepartmentEmployee
                {
                    EmpNo = empNo,
                    DeptNo = deptNo,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                _context.DepartmentEmployees.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Assignments", new { id = deptNo });
            }

        // GET: Department/Managers/{id}
        public async Task<IActionResult> Managers(int id)
        {
            var managers = await _context.DepartmentManagers
                .Include(dm => dm.Employee)
                .Where(dm => dm.DeptNo == id)
                .ToListAsync();
            ViewBag.Department = await _context.Departments.FindAsync(id);
            return View(managers);
        }
        
            // GET: Department/AddManager/{deptNo}
            public IActionResult AddManager(int deptNo)
            {
                var department = _context.Departments.Find(deptNo);
                if (department == null) return NotFound();
                var empleadosNoGerentes = _context.Employees
                    .Where(e => !_context.DepartmentManagers.Any(dm => dm.EmpNo == e.EmpNo && dm.DeptNo == deptNo && (dm.ToDate == "9999-12-31" || string.IsNullOrEmpty(dm.ToDate))))
                    .ToList();
                ViewBag.Department = department;
                ViewBag.Employees = empleadosNoGerentes;
                return View();
            }
        
            // POST: Department/AddManager/{deptNo}
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddManager(int deptNo, int empNo, string fromDate, string toDate)
            {
                var department = await _context.Departments.FindAsync(deptNo);
                var employee = await _context.Employees.FindAsync(empNo);
                if (department == null || employee == null)
                {
                    return NotFound();
                }
                var manager = new DepartmentManager
                {
                    EmpNo = empNo,
                    DeptNo = deptNo,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                _context.DepartmentManagers.Add(manager);
                await _context.SaveChangesAsync();
                return RedirectToAction("Managers", new { id = deptNo });
            }
    }
}
