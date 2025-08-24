using app.Data;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Services
{
    /// <summary>
    /// Servicio para auditoría de cambios de salarios
    /// </summary>
    public interface IAuditService
    {
        Task LogSalaryChangeAsync(int empNo, long oldSalary, long newSalary, string usuario, string detalle);
        Task<List<AuditLog>> GetAuditLogsByEmployeeAsync(int empNo);
        Task<List<AuditLog>> GetRecentAuditLogsAsync(int count = 50);
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
            var auditLog = new AuditLog
            {
                EmpNo = empNo,
                Usuario = usuario,
                FechaActualizacion = DateTime.Now,
                Salario = newSalary,
                DetalleCambio = $"{detalle}. Salario anterior: {oldSalary:C}, Nuevo salario: {newSalary:C}"
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
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
        /// Obtiene los registros de auditoría más recientes
        /// </summary>
        public async Task<List<AuditLog>> GetRecentAuditLogsAsync(int count = 50)
        {
            return await _context.AuditLogs
                .Include(a => a.Employee)
                .OrderByDescending(a => a.FechaActualizacion)
                .Take(count)
                .ToListAsync();
        }
    }
}
