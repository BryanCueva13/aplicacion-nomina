using app.Data;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Services
{
    /// <summary>
    /// Servicio para auditoría completa de cambios en la base de datos
    /// </summary>
    public interface IAuditService
    {
        Task LogSalaryChangeAsync(int empNo, long oldSalary, long newSalary, string usuario, string detalle);
        Task<List<AuditLog>> GetAuditLogsByEmployeeAsync(int empNo);
        Task<List<AuditLog>> GetRecentAuditLogsAsync(int count = 50);
        Task LogGenericChangeAsync(int empNo, string usuario, string detalle, long salario = 0);
        
        // Nuevos métodos para auditoría general
        Task LogGeneralChangeAsync(string tabla, string tipoOperacion, string detalle, string usuario, int? empNo = null, string? registroId = null, string? valorAnterior = null, string? valorNuevo = null);
        Task<List<GeneralAuditLog>> GetRecentGeneralAuditLogsAsync(int count = 50);
        Task<List<GeneralAuditLog>> GetAllAuditLogsAsync(int count = 100);
    }

    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra un cambio de salario en la auditoría
        /// </summary>
        public async Task LogSalaryChangeAsync(int empNo, long oldSalary, long newSalary, string usuario, string detalle)
        {
            try
            {
                // Verificar que el empleado existe
                var employeeExists = await _context.Employees.AnyAsync(e => e.EmpNo == empNo);
                if (!employeeExists)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: Employee {empNo} does not exist");
                    return;
                }

                // Log específico de salarios
                var auditLog = new AuditLog
                {
                    EmpNo = empNo,
                    Usuario = usuario ?? "Sistema",
                    FechaActualizacion = DateTime.Now,
                    Salario = newSalary,
                    DetalleCambio = $"{detalle}. Salario anterior: {(oldSalary / 100m):C}, Nuevo salario: {(newSalary / 100m):C}"
                };

                _context.AuditLogs.Add(auditLog);

                // Log general también
                await LogGeneralChangeAsync("salaries", "UPDATE", 
                    $"Cambio de salario: {(oldSalary / 100m):C} → {(newSalary / 100m):C}. {detalle}", 
                    usuario ?? "Sistema", empNo, $"emp_{empNo}", (oldSalary / 100m).ToString("C"), (newSalary / 100m).ToString("C"));

                await _context.SaveChangesAsync();
                
                System.Diagnostics.Debug.WriteLine($"Audit log created: ID={auditLog.Id}, EmpNo={empNo}, Salario={newSalary}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating audit log: {ex.Message}");
            }
        }

        public async Task LogGenericChangeAsync(int empNo, string usuario, string detalle, long salario = 0)
        {
            try
            {
                var employeeExists = await _context.Employees.AnyAsync(e => e.EmpNo == empNo);
                if (!employeeExists)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: Employee {empNo} does not exist");
                    return;
                }

                // Para cambios genéricos, usar solo auditoría general
                await LogGeneralChangeAsync("titles", "UPDATE", detalle, usuario ?? "Sistema", empNo, $"emp_{empNo}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating generic audit log: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra cualquier cambio en la base de datos
        /// </summary>
        public async Task LogGeneralChangeAsync(string tabla, string tipoOperacion, string detalle, string usuario, 
            int? empNo = null, string? registroId = null, string? valorAnterior = null, string? valorNuevo = null)
        {
            try
            {
                var generalAuditLog = new GeneralAuditLog
                {
                    Usuario = usuario ?? "Sistema",
                    FechaActualizacion = DateTime.Now,
                    TipoOperacion = tipoOperacion,
                    Tabla = tabla,
                    DetalleCambio = detalle,
                    RegistroId = registroId,
                    EmpNo = empNo,
                    ValorAnterior = valorAnterior,
                    ValorNuevo = valorNuevo
                };

                _context.GeneralAuditLogs.Add(generalAuditLog);
                await _context.SaveChangesAsync();
                
                System.Diagnostics.Debug.WriteLine($"General audit log created: ID={generalAuditLog.Id}, Tabla={tabla}, Operacion={tipoOperacion}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating general audit log: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el historial de auditoría de un empleado específico
        /// </summary>
        public async Task<List<AuditLog>> GetAuditLogsByEmployeeAsync(int empNo)
        {
            return await _context.AuditLogs
                .Where(a => a.EmpNo == empNo)
                .Include(a => a.Employee)
                .OrderByDescending(a => a.FechaActualizacion)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los registros de auditoría de salarios más recientes
        /// </summary>
        public async Task<List<AuditLog>> GetRecentAuditLogsAsync(int count = 50)
        {
            return await _context.AuditLogs
                .Include(a => a.Employee)
                .OrderByDescending(a => a.FechaActualizacion)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los registros de auditoría general más recientes
        /// </summary>
        public async Task<List<GeneralAuditLog>> GetRecentGeneralAuditLogsAsync(int count = 50)
        {
            return await _context.GeneralAuditLogs
                .Include(a => a.Employee)
                .OrderByDescending(a => a.FechaActualizacion)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los logs combinados (salarios + general)
        /// </summary>
        public async Task<List<GeneralAuditLog>> GetAllAuditLogsAsync(int count = 100)
        {
            return await _context.GeneralAuditLogs
                .Include(a => a.Employee)
                .OrderByDescending(a => a.FechaActualizacion)
                .Take(count)
                .ToListAsync();
        }
    }
}
