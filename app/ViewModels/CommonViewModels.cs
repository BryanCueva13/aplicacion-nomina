using System.ComponentModel.DataAnnotations;

namespace app.ViewModels
{
    /// <summary>
    /// ViewModel para login de usuarios
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    /// <summary>
    /// ViewModel para registro de nuevos usuarios
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "La CI es requerida")]
        [Display(Name = "Cédula de Identidad")]
        public string CI { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Apellido")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es requerido")]
        [Display(Name = "Género")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de contratación es requerida")]
        [Display(Name = "Fecha de Contratación")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Nombre de Usuario")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para creación/edición de empleados
    /// </summary>
    public class EmployeeViewModel
    {
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "La CI es requerida")]
        [Display(Name = "Cédula de Identidad")]
        public string CI { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Apellido")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es requerido")]
        [Display(Name = "Género")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de contratación es requerida")]
        [Display(Name = "Fecha de Contratación")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // Propiedades para el usuario (opcional)
        [Display(Name = "Crear Usuario")]
        public bool CreateUser { get; set; }

        [Display(Name = "Nombre de Usuario")]
        [StringLength(100)]
        public string? Username { get; set; }

        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Rol")]
        public string Role { get; set; } = "RRHH";
    }

    /// <summary>
    /// ViewModel para listado de empleados con paginación
    /// </summary>
    public class EmployeeListViewModel
    {
        public List<EmployeeItemViewModel> Employees { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    }

    /// <summary>
    /// ViewModel para item de empleado en listado
    /// </summary>
    public class EmployeeItemViewModel
    {
        public int EmpNo { get; set; }
        public string CI { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? HireDate { get; set; }
        public string? CurrentDepartment { get; set; }
        public string? CurrentTitle { get; set; }
        public decimal? CurrentSalary { get; set; }
        public bool HasUser { get; set; }
    }

    /// <summary>
    /// ViewModel para departamentos
    /// </summary>
    public class DepartmentViewModel
    {
        public int DeptNo { get; set; }

        [Required(ErrorMessage = "El nombre del departamento es requerido")]
        [Display(Name = "Nombre del Departamento")]
        [StringLength(50)]
        public string DeptName { get; set; } = string.Empty;

        public int TotalEmployees { get; set; }
        public string? CurrentManager { get; set; }
    }

    /// <summary>
    /// ViewModel para asignación de empleado a departamento
    /// </summary>
    public class DepartmentAssignmentViewModel
    {
        public int EmpNo { get; set; }
        public int DeptNo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
    }

    /// <summary>
    /// ViewModel para títulos/cargos
    /// </summary>
    public class TitleViewModel
    {
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [Display(Name = "Título/Cargo")]
        [StringLength(50)]
        public string TitleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public string? EmployeeName { get; set; }
    }

    /// <summary>
    /// ViewModel para salarios
    /// </summary>
    public class SalaryViewModel
    {
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "El salario es requerido")]
        [Display(Name = "Salario")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor a 0")]
        public decimal SalaryAmount { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public string? EmployeeName { get; set; }
    }

    /// <summary>
    /// ViewModel para reporte de nómina
    /// </summary>
    public class PayrollReportViewModel
    {
        public int EmpNo { get; set; }
        public string CI { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal CurrentSalary { get; set; }
        public string HireDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para reporte de estructura organizacional
    /// </summary>
    public class OrganizationalReportViewModel
    {
        public int DeptNo { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public ManagerInfo? Manager { get; set; }
        public List<EmployeeInfo> Employees { get; set; } = new();
        public int EmployeeCount { get; set; }
    }

    /// <summary>
    /// Información del gerente para el reporte organizacional
    /// </summary>
    public class ManagerInfo
    {
        public int EmpNo { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// Información del empleado para el reporte organizacional
    /// </summary>
    public class EmployeeInfo
    {
        public int EmpNo { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
    }
}
