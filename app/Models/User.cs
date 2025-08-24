using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    /// <summary>
    /// Modelo para la tabla users (usuarios del sistema)
    /// </summary>
    [Table("users")]
    public class User
    {
        [Key]
        [Column("emp_no")]
        public int EmpNo { get; set; }

        [Required]
        [Column("usuario")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("clave")]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;

        // Propiedades de navegación
        [ForeignKey("EmpNo")]
        public virtual Employee Employee { get; set; } = null!;

        // Propiedades adicionales para roles (se puede expandir)
        [NotMapped]
        public string Role { get; set; } = "RRHH"; // Por defecto RRHH, puede ser "Administrador"

        [NotMapped]
        public DateTime LastLogin { get; set; }

        [NotMapped]
        public bool IsActive { get; set; } = true;
    }
}
