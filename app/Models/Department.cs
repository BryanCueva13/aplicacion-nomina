using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para la tabla departments
    /// </summary>
    [Table("departments")]
    public class Department
    {
        [Key]
        [Column("dept_no")]
        public int DeptNo { get; set; }

        [Required]
        [Column("dept_name")]
        [StringLength(50)]
        public string DeptName { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual ICollection<DepartmentEmployee> DepartmentEmployees { get; set; } = new List<DepartmentEmployee>();
        public virtual ICollection<DepartmentManager> DepartmentManagers { get; set; } = new List<DepartmentManager>();
    }
}
