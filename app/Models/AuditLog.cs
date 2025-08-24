using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para la tabla Log_AuditoriaSalarios (auditoría de cambios de salarios)
    /// </summary>
    [Table("Log_AuditoriaSalarios")]
    public class AuditLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("usuario")]
        [StringLength(50)]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        [Column("fechaActualizacion")]
        public DateTime FechaActualizacion { get; set; }

        [Required]
        [Column("DetalleCambio")]
        [StringLength(250)]
        public string DetalleCambio { get; set; } = string.Empty;

        [Required]
        [Column("salario")]
        public long Salario { get; set; }

        [Required]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        // Propiedades de navegación
        [ForeignKey("EmpNo")]
        public virtual Employee Employee { get; set; } = null!;

        // Propiedades calculadas
        [NotMapped]
        public decimal SalarioDecimal => Salario / 100m; // Asumiendo que se almacena en centavos
    }
}
