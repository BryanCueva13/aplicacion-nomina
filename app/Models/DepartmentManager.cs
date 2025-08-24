using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para la tabla dept_manager (gerentes de departamentos)
    /// </summary>
    [Table("dept_manager")]
    public class DepartmentManager
    {
        [Key, Column("emp_no", Order = 0)]
        public int EmpNo { get; set; }

        [Key, Column("dept_no", Order = 1)]
        public int DeptNo { get; set; }

        [Required]
        [Column("from_date")]
        [StringLength(50)]
        public string FromDate { get; set; } = string.Empty;

        [Required]
        [Column("to_date")]
        [StringLength(50)]
        public string ToDate { get; set; } = string.Empty;

        // Propiedades de navegación
        [ForeignKey("EmpNo")]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey("DeptNo")]
        public virtual Department Department { get; set; } = null!;

        // Propiedades calculadas
        [NotMapped]
        public DateTime? FromDateTime 
        { 
            get 
            { 
                if (DateTime.TryParse(FromDate, out DateTime result))
                    return result;
                return null;
            } 
        }

        [NotMapped]
        public DateTime? ToDateTime 
        { 
            get 
            { 
                if (DateTime.TryParse(ToDate, out DateTime result))
                    return result;
                return null;
            } 
        }

        [NotMapped]
        public bool IsActive => ToDateTime == null || ToDateTime > DateTime.Now;
    }
}
