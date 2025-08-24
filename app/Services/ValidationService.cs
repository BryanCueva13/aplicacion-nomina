using app.Data;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Services
{
    /// <summary>
    /// Servicio para validaciones de negocio
    /// </summary>
    public interface IValidationService
    {
        Task<bool> ValidateNoOverlappingDepartmentAssignmentAsync(int empNo, DateTime fromDate, DateTime toDate, int? excludeDeptNo = null);
        Task<bool> ValidateNoOverlappingTitleAsync(int empNo, DateTime fromDate, DateTime? toDate, string? excludeTitle = null, DateTime? excludeFromDate = null);
        Task<bool> ValidateNoOverlappingSalaryAsync(int empNo, DateTime fromDate, DateTime toDate, DateTime? excludeFromDate = null);
        Task<bool> ValidateOnlyOneActiveManagerPerDepartmentAsync(int deptNo, DateTime fromDate, DateTime toDate, int? excludeEmpNo = null);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeEmpNo = null);
        Task<bool> IsCIUniqueAsync(string ci, int? excludeEmpNo = null);
        Task<bool> IsUsernameUniqueAsync(string username, int? excludeEmpNo = null);
    }

    public class ValidationService : IValidationService
    {
        private readonly ApplicationDbContext _context;

        public ValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Valida que no haya solapamiento en asignaciones de departamento
        /// </summary>
        public async Task<bool> ValidateNoOverlappingDepartmentAssignmentAsync(int empNo, DateTime fromDate, DateTime toDate, int? excludeDeptNo = null)
        {
            var query = _context.DepartmentEmployees
                .Where(de => de.EmpNo == empNo);

            if (excludeDeptNo.HasValue)
            {
                query = query.Where(de => de.DeptNo != excludeDeptNo.Value);
            }

            var existingAssignments = await query.ToListAsync();

            foreach (var assignment in existingAssignments)
            {
                if (assignment.FromDateTime.HasValue && assignment.ToDateTime.HasValue)
                {
                    // Verificar solapamiento
                    if (fromDate < assignment.ToDateTime && toDate > assignment.FromDateTime)
                    {
                        return false; // Hay solapamiento
                    }
                }
            }

            return true; // No hay solapamiento
        }

        /// <summary>
        /// Valida que no haya solapamiento en títulos
        /// </summary>
        public async Task<bool> ValidateNoOverlappingTitleAsync(int empNo, DateTime fromDate, DateTime? toDate, string? excludeTitle = null, DateTime? excludeFromDate = null)
        {
            var query = _context.Titles
                .Where(t => t.EmpNo == empNo);

            if (!string.IsNullOrEmpty(excludeTitle) && excludeFromDate.HasValue)
            {
                query = query.Where(t => !(t.TitleName == excludeTitle && t.FromDateTime == excludeFromDate));
            }

            var existingTitles = await query.ToListAsync();

            foreach (var title in existingTitles)
            {
                if (title.FromDateTime.HasValue)
                {
                    var titleToDate = title.ToDateTime ?? DateTime.MaxValue;
                    var newToDate = toDate ?? DateTime.MaxValue;

                    // Verificar solapamiento
                    if (fromDate < titleToDate && newToDate > title.FromDateTime)
                    {
                        return false; // Hay solapamiento
                    }
                }
            }

            return true; // No hay solapamiento
        }

        /// <summary>
        /// Valida que no haya solapamiento en salarios
        /// </summary>
        public async Task<bool> ValidateNoOverlappingSalaryAsync(int empNo, DateTime fromDate, DateTime toDate, DateTime? excludeFromDate = null)
        {
            var query = _context.Salaries
                .Where(s => s.EmpNo == empNo);

            if (excludeFromDate.HasValue)
            {
                query = query.Where(s => s.FromDateTime != excludeFromDate);
            }

            var existingSalaries = await query.ToListAsync();

            foreach (var salary in existingSalaries)
            {
                if (salary.FromDateTime.HasValue && salary.ToDateTime.HasValue)
                {
                    // Verificar solapamiento
                    if (fromDate < salary.ToDateTime && toDate > salary.FromDateTime)
                    {
                        return false; // Hay solapamiento
                    }
                }
            }

            return true; // No hay solapamiento
        }

        /// <summary>
        /// Valida que solo haya un gerente activo por departamento
        /// </summary>
        public async Task<bool> ValidateOnlyOneActiveManagerPerDepartmentAsync(int deptNo, DateTime fromDate, DateTime toDate, int? excludeEmpNo = null)
        {
            var query = _context.DepartmentManagers
                .Where(dm => dm.DeptNo == deptNo);

            if (excludeEmpNo.HasValue)
            {
                query = query.Where(dm => dm.EmpNo != excludeEmpNo.Value);
            }

            var existingManagers = await query.ToListAsync();

            foreach (var manager in existingManagers)
            {
                if (manager.FromDateTime.HasValue && manager.ToDateTime.HasValue)
                {
                    // Verificar solapamiento
                    if (fromDate < manager.ToDateTime && toDate > manager.FromDateTime)
                    {
                        return false; // Hay solapamiento
                    }
                }
            }

            return true; // No hay solapamiento
        }

        /// <summary>
        /// Valida que el email sea único
        /// </summary>
        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeEmpNo = null)
        {
            var query = _context.Employees.Where(e => e.Email == email);

            if (excludeEmpNo.HasValue)
            {
                query = query.Where(e => e.EmpNo != excludeEmpNo.Value);
            }

            return !await query.AnyAsync();
        }

        /// <summary>
        /// Valida que la CI sea única
        /// </summary>
        public async Task<bool> IsCIUniqueAsync(string ci, int? excludeEmpNo = null)
        {
            var query = _context.Employees.Where(e => e.CI == ci);

            if (excludeEmpNo.HasValue)
            {
                query = query.Where(e => e.EmpNo != excludeEmpNo.Value);
            }

            return !await query.AnyAsync();
        }

        /// <summary>
        /// Valida que el nombre de usuario sea único
        /// </summary>
        public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeEmpNo = null)
        {
            var query = _context.Users.Where(u => u.Username == username);

            if (excludeEmpNo.HasValue)
            {
                query = query.Where(u => u.EmpNo != excludeEmpNo.Value);
            }

            return !await query.AnyAsync();
        }
    }
}
