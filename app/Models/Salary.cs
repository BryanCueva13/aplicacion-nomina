using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para la tabla salaries (salarios de empleados)
    /// </summary>
    [Table("salaries")]
    public class Salary
    {
        [Key, Column("emp_no", Order = 0)]
        public int EmpNo { get; set; }

        [Key, Column("from_date", Order = 1)]
        [StringLength(50)]
        public string FromDate { get; set; } = string.Empty;

        [Required]
        [Column("salary")]
        public long SalaryAmount { get; set; }

        [Required]
        [Column("to_date")]
        [StringLength(50)]
        public string ToDate { get; set; } = string.Empty;

        // Propiedades de navegación
        [ForeignKey("EmpNo")]
        public virtual Employee Employee { get; set; } = null!;

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

        [NotMapped]
        public decimal SalaryDecimal => SalaryAmount / 100m; // Asumiendo que se almacena en centavos
    }
}
