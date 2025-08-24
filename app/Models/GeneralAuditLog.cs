using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para auditoría general de todos los cambios en la base de datos
    /// </summary>
    [Table("Log_Auditoria_General")]
    public class GeneralAuditLog
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("usuario")]
        [StringLength(50)]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        [Column("fechaActualizacion")]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        [Required]
        [Column("tipoOperacion")]
        [StringLength(20)]
        public string TipoOperacion { get; set; } = string.Empty; // CREATE, UPDATE, DELETE

        [Required]
        [Column("tabla")]
        [StringLength(50)]
        public string Tabla { get; set; } = string.Empty; // employees, salaries, titles, departments, etc.

        [Required]
        [Column("detalleCambio")]
        [StringLength(500)]
        public string DetalleCambio { get; set; } = string.Empty;

        [Column("registroId")]
        [StringLength(100)]
        public string? RegistroId { get; set; } // ID o clave del registro afectado

        [Column("emp_no")]
        public int? EmpNo { get; set; } // Solo si está relacionado con un empleado

        [Column("valorAnterior")]
        [StringLength(500)]
        public string? ValorAnterior { get; set; }

        [Column("valorNuevo")]
        [StringLength(500)]
        public string? ValorNuevo { get; set; }

        // Propiedades de navegación
        [ForeignKey("EmpNo")]
        public virtual Employee? Employee { get; set; }

        // Propiedades calculadas para mostrar información más clara
        [NotMapped]
        public string TipoOperacionDescripcion => TipoOperacion switch
        {
            "CREATE" => "Creación",
            "UPDATE" => "Modificación", 
            "DELETE" => "Eliminación",
            _ => TipoOperacion
        };

        [NotMapped]
        public string TablaDescripcion => Tabla switch
        {
            "employees" => "Empleados",
            "salaries" => "Salarios",
            "titles" => "Títulos",
            "departments" => "Departamentos",
            "dept_emp" => "Asignación Departamentos",
            "dept_manager" => "Gerentes Departamento",
            _ => Tabla
        };
    }
}
