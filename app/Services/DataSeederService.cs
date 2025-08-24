using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;
using app.Services;

namespace app.Services
{
    /// <summary>
    /// Servicio para inicializar datos de prueba en la base de datos
    /// </summary>
    public interface IDataSeederService
    {
        Task SeedAsync();
        Task<bool> HasDataAsync();
    }

    public class DataSeederService : IDataSeederService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public DataSeederService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Verifica si ya existen datos en la base de datos
        /// </summary>
        public async Task<bool> HasDataAsync()
        {
            return await _context.Employees.AnyAsync();
        }

        /// <summary>
        /// Crea datos iniciales de prueba
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                // Verificar si ya existen datos
                if (await HasDataAsync())
                {
                    return;
                }

                // Crear empleados de prueba
                var employees = new List<Employee>
                {
                    new Employee
                    {
                        EmpNo = 1001,
                        CI = "12345678",
                        BirthDate = "1985-05-15",
                        FirstName = "Juan",
                        LastName = "Pérez",
                        Gender = "M",
                        HireDate = "2020-01-15",
                        Email = "juan.perez@empresa.com"
                    },
                    new Employee
                    {
                        EmpNo = 1002,
                        CI = "87654321",
                        BirthDate = "1990-08-22",
                        FirstName = "María",
                        LastName = "González",
                        Gender = "F",
                        HireDate = "2019-03-10",
                        Email = "maria.gonzalez@empresa.com"
                    },
                    new Employee
                    {
                        EmpNo = 1003,
                        CI = "11223344",
                        BirthDate = "1988-12-03",
                        FirstName = "Carlos",
                        LastName = "Rodríguez",
                        Gender = "M",
                        HireDate = "2021-06-01",
                        Email = "carlos.rodriguez@empresa.com"
                    }
                };

                _context.Employees.AddRange(employees);
                await _context.SaveChangesAsync();

                // Crear departamentos
                var departments = new List<Department>
                {
                    new Department { DeptNo = 1, DeptName = "Recursos Humanos" },
                    new Department { DeptNo = 2, DeptName = "Tecnología" },
                    new Department { DeptNo = 3, DeptName = "Ventas" },
                    new Department { DeptNo = 4, DeptName = "Marketing" },
                    new Department { DeptNo = 5, DeptName = "Administración" }
                };

                _context.Departments.AddRange(departments);
                await _context.SaveChangesAsync();

                // Crear asignaciones de empleados a departamentos
                var deptEmployees = new List<DepartmentEmployee>
                {
                    new DepartmentEmployee { EmpNo = 1001, DeptNo = 1, FromDate = "2020-01-15", ToDate = "9999-12-31" },
                    new DepartmentEmployee { EmpNo = 1002, DeptNo = 2, FromDate = "2019-03-10", ToDate = "9999-12-31" },
                    new DepartmentEmployee { EmpNo = 1003, DeptNo = 2, FromDate = "2021-06-01", ToDate = "9999-12-31" }
                };

                _context.DepartmentEmployees.AddRange(deptEmployees);
                await _context.SaveChangesAsync();

                // Crear gerentes
                var managers = new List<DepartmentManager>
                {
                    new DepartmentManager { EmpNo = 1001, DeptNo = 1, FromDate = "2020-01-15", ToDate = "9999-12-31" },
                    new DepartmentManager { EmpNo = 1002, DeptNo = 2, FromDate = "2019-03-10", ToDate = "9999-12-31" }
                };

                _context.DepartmentManagers.AddRange(managers);
                await _context.SaveChangesAsync();

                // Crear títulos
                var titles = new List<Title>
                {
                    new Title { EmpNo = 1001, TitleName = "Gerente de RRHH", FromDate = "2020-01-15", ToDate = null },
                    new Title { EmpNo = 1002, TitleName = "Jefe de Tecnología", FromDate = "2019-03-10", ToDate = null },
                    new Title { EmpNo = 1003, TitleName = "Desarrollador Senior", FromDate = "2021-06-01", ToDate = null }
                };

                _context.Titles.AddRange(titles);
                await _context.SaveChangesAsync();

                // Crear salarios
                var salaries = new List<Salary>
                {
                    new Salary { EmpNo = 1001, SalaryAmount = 800000, FromDate = "2020-01-15", ToDate = "9999-12-31" },
                    new Salary { EmpNo = 1002, SalaryAmount = 1200000, FromDate = "2019-03-10", ToDate = "9999-12-31" },
                    new Salary { EmpNo = 1003, SalaryAmount = 900000, FromDate = "2021-06-01", ToDate = "9999-12-31" }
                };

                _context.Salaries.AddRange(salaries);
                await _context.SaveChangesAsync();

                // Crear usuarios con contraseñas cifradas
                var users = new List<User>
                {
                    new User 
                    { 
                        EmpNo = 1001, 
                        Username = "admin", 
                        PasswordHash = _authService.HashPassword("123456"),
                        Role = "Administrador"
                    },
                    new User 
                    { 
                        EmpNo = 1002, 
                        Username = "maria.gonzalez", 
                        PasswordHash = _authService.HashPassword("123456"),
                        Role = "RRHH"
                    },
                    new User 
                    { 
                        EmpNo = 1003, 
                        Username = "carlos.rodriguez", 
                        PasswordHash = _authService.HashPassword("123456"),
                        Role = "RRHH"
                    }
                };

                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();

                Console.WriteLine("Datos de prueba creados exitosamente:");
                Console.WriteLine("Usuarios disponibles:");
                Console.WriteLine("- admin / 123456 (Administrador)");
                Console.WriteLine("- maria.gonzalez / 123456 (RRHH)");
                Console.WriteLine("- carlos.rodriguez / 123456 (RRHH)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear datos de prueba: {ex.Message}");
                throw;
            }
        }
    }
}
