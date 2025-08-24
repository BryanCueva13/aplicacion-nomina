using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para la tabla employees
    /// </summary>
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required]
        [Column("ci")]
        [StringLength(50)]
        public string CI { get; set; } = string.Empty;

        [Required]
        [Column("birth_date")]
        [StringLength(50)]
        public string BirthDate { get; set; } = string.Empty;

        [Required]
        [Column("first_name")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column("last_name")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column("gender")]
        [StringLength(1)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Column("hire_date")]
        [StringLength(50)]
        public string HireDate { get; set; } = string.Empty;

        [Required]
        [Column("correo")]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual ICollection<DepartmentEmployee> DepartmentEmployees { get; set; } = new List<DepartmentEmployee>();
        public virtual ICollection<DepartmentManager> DepartmentManagers { get; set; } = new List<DepartmentManager>();
        public virtual ICollection<Title> Titles { get; set; } = new List<Title>();
        public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
        public virtual User? User { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        // Propiedades calculadas
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public DateTime? BirthDateTime 
        { 
            get 
            { 
                if (DateTime.TryParse(BirthDate, out DateTime result))
                    return result;
                return null;
            } 
        }

        [NotMapped]
        public DateTime? HireDatetime 
        { 
            get 
            { 
                if (DateTime.TryParse(HireDate, out DateTime result))
                    return result;
                return null;
            } 
        }
    }
}
