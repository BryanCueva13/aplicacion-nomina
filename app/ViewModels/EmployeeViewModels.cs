using System.ComponentModel.DataAnnotations;

namespace app.ViewModels
{
    /// <summary>
    /// ViewModel para formularios de empleado (crear/editar)
    /// </summary>
    public class EmployeeViewModel
    {
        public int EmpNo { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(20, ErrorMessage = "La cédula no puede tener más de 20 caracteres")]
        [Display(Name = "Cédula de Identidad")]
        public string CI { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        [Display(Name = "Nombres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres")]
        [Display(Name = "Apellidos")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es obligatorio")]
        [Display(Name = "Género")]
        public string Gender { get; set; } = "M";

        [Required(ErrorMessage = "La fecha de contratación es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Contratación")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        // Datos de usuario (opcional)
        [Display(Name = "Crear Usuario del Sistema")]
        public bool CreateUser { get; set; }

        [StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string? Username { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Rol")]
        public string Role { get; set; } = "RRHH";

        // Propiedades calculadas para mostrar
        public string FullName => $"{FirstName} {LastName}";
        
        public int Age 
        { 
            get 
            { 
                var age = DateTime.Now.Year - BirthDate.Year;
                if (DateTime.Now.DayOfYear < BirthDate.DayOfYear)
                    age--;
                return age;
            } 
        }

        public int YearsOfService
        {
            get
            {
                var years = DateTime.Now.Year - HireDate.Year;
                if (DateTime.Now.DayOfYear < HireDate.DayOfYear)
                    years--;
                return years;
            }
        }
    }

    /// <summary>
    /// ViewModel para elemento de lista de empleados
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

        public string HireDateFormatted => HireDate?.ToString("dd/MM/yyyy") ?? "N/A";
        public string SalaryFormatted => CurrentSalary?.ToString("C") ?? "N/A";
        public int YearsOfService 
        { 
            get 
            {
                if (!HireDate.HasValue) return 0;
                var years = DateTime.Now.Year - HireDate.Value.Year;
                if (DateTime.Now.DayOfYear < HireDate.Value.DayOfYear)
                    years--;
                return years;
            } 
        }
    }

    /// <summary>
    /// ViewModel para lista paginada de empleados
    /// </summary>
    public class EmployeeListViewModel
    {
        public List<EmployeeItemViewModel> Employees { get; set; } = new List<EmployeeItemViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartItem => ((CurrentPage - 1) * PageSize) + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);

        public string GetSortDirection(string column)
        {
            if (SortBy == column)
                return SortDirection == "asc" ? "desc" : "asc";
            return "asc";
        }

        public string GetSortIcon(string column)
        {
            if (SortBy != column)
                return "bi-chevron-expand";
            
            return SortDirection == "asc" ? "bi-chevron-up" : "bi-chevron-down";
        }
    }
}
