using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para la tabla titles (títulos/cargos de empleados)
    /// </summary>
    [Table("titles")]
    public class Title
    {
        [Key, Column("emp_no", Order = 0)]
        public int EmpNo { get; set; }

        [Key, Column("title", Order = 1)]
        [StringLength(50)]
        public string TitleName { get; set; } = string.Empty;

        [Key, Column("from_date", Order = 2)]
        [StringLength(50)]
        public string FromDate { get; set; } = string.Empty;

        [Column("to_date")]
        [StringLength(50)]
        public string? ToDate { get; set; }

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
                if (!string.IsNullOrEmpty(ToDate) && DateTime.TryParse(ToDate, out DateTime result))
                    return result;
                return null;
            } 
        }

        [NotMapped]
        public bool IsActive => string.IsNullOrEmpty(ToDate) || ToDateTime > DateTime.Now;
    }
}
