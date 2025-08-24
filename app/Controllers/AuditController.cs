using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Data;
using app.Models;
using System.Threading.Tasks;
using System.Linq;

namespace app.Controllers
{
    public class AuditController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly app.Services.IAuditService _auditService;
        
        public AuditController(ApplicationDbContext context, app.Services.IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Audit - Mostrar TODOS los cambios en la base de datos
        public async Task<IActionResult> Index()
        {
            try
            {
                // Obtener logs de auditoría general (todos los cambios)
                var generalLogs = await _auditService.GetAllAuditLogsAsync(100);
                
                // También obtener logs específicos de salarios para compatibilidad
                var salaryLogs = await _context.AuditLogs.Include(a => a.Employee).OrderByDescending(a => a.FechaActualizacion).Take(50).ToListAsync();

                // Combinar logs para vista unificada
                var combinedLogs = new List<dynamic>();
                
                // Agregar logs generales
                foreach (var log in generalLogs)
                {
                    combinedLogs.Add(new
                    {
                        Id = log.Id,
                        FechaActualizacion = log.FechaActualizacion,
                        Usuario = log.Usuario,
                        TipoOperacion = log.TipoOperacionDescripcion,
                        Tabla = log.TablaDescripcion,
                        DetalleCambio = log.DetalleCambio,
                        EmpleadoNombre = log.Employee?.FullName ?? (log.EmpNo.HasValue ? $"Empleado #{log.EmpNo}" : "N/A"),
                        EmpNo = log.EmpNo,
                        Tipo = "General"
                    });
                }

                // Agregar logs de salarios legacy (solo si no están ya incluidos en generales)
                foreach (var log in salaryLogs)
                {
                    // Evitar duplicados - solo agregar si no hay un log general reciente para el mismo empleado
                    var hasRecentGeneralLog = generalLogs.Any(gl => 
                        gl.EmpNo == log.EmpNo && 
                        gl.Tabla == "salaries" && 
                        Math.Abs((gl.FechaActualizacion - log.FechaActualizacion).TotalMinutes) < 5);
                    
                    if (!hasRecentGeneralLog)
                    {
                        combinedLogs.Add(new
                        {
                            Id = log.Id,
                            FechaActualizacion = log.FechaActualizacion,
                            Usuario = log.Usuario,
                            TipoOperacion = "Modificación",
                            Tabla = "Salarios",
                            DetalleCambio = log.DetalleCambio,
                            EmpleadoNombre = log.Employee?.FullName ?? $"Empleado #{log.EmpNo}",
                            EmpNo = (int?)log.EmpNo,
                            Tipo = "Salary"
                        });
                    }
                }

                // Ordenar por fecha descendente
                combinedLogs = combinedLogs.OrderByDescending(l => l.FechaActualizacion).ToList();
                
                ViewBag.CombinedLogs = combinedLogs;
                
                return View(generalLogs);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar logs de auditoría: {ex.Message}";
                return View(new List<GeneralAuditLog>());
            }
        }

        // GET: Audit/CreateGeneralTable - Crear tabla de auditoría general
        public async Task<IActionResult> CreateGeneralTable()
        {
            try
            {
                var sql = @"
                    -- Crear tabla de auditoría general si no existe
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Log_Auditoria_General')
                    BEGIN
                        CREATE TABLE Log_Auditoria_General (
                            id int IDENTITY(1,1) PRIMARY KEY,
                            usuario nvarchar(50) NOT NULL,
                            fechaActualizacion datetime2 NOT NULL DEFAULT GETDATE(),
                            tipoOperacion nvarchar(20) NOT NULL,
                            tabla nvarchar(50) NOT NULL,
                            detalleCambio nvarchar(500) NOT NULL,
                            registroId nvarchar(100) NULL,
                            emp_no int NULL,
                            valorAnterior nvarchar(500) NULL,
                            valorNuevo nvarchar(500) NULL,
                            FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
                        );
                        
                        SELECT 'Tabla de auditoría general creada exitosamente' as resultado;
                    END
                    ELSE
                    BEGIN
                        SELECT 'La tabla de auditoría general ya existe' as resultado;
                    END";

                await _context.Database.ExecuteSqlRawAsync(sql);
                TempData["SuccessMessage"] = "Tabla de auditoría general verificada/creada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear tabla general: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Audit/Test - Crear entradas de prueba
        public async Task<IActionResult> Test()
        {
            try
            {
                // Obtener el primer empleado que exista
                var firstEmployee = await _context.Employees.FirstOrDefaultAsync();
                if (firstEmployee == null)
                {
                    TempData["ErrorMessage"] = "No hay empleados en la base de datos para crear una entrada de auditoría.";
                    return RedirectToAction(nameof(Index));
                }

                // Crear entrada de auditoría general de prueba
                await _auditService.LogGeneralChangeAsync("employees", "UPDATE", 
                    $"Prueba de auditoría para {firstEmployee.FullName}", 
                    "Sistema de Prueba", firstEmployee.EmpNo, $"emp_{firstEmployee.EmpNo}");

                // Crear entrada de auditoría de salario de prueba
                await _auditService.LogSalaryChangeAsync(firstEmployee.EmpNo, 5000000, 5500000, "Sistema de Prueba", "Prueba de cambio de salario");
                
                var totalGeneralLogs = await _context.GeneralAuditLogs.CountAsync();
                var totalSalaryLogs = await _context.AuditLogs.CountAsync();
                
                TempData["SuccessMessage"] = $"Entradas de auditoría de prueba creadas. Total general: {totalGeneralLogs}, Total salarios: {totalSalaryLogs}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear entradas de prueba: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Audit/FixTable - Acción para corregir la tabla de auditoría de salarios
        public async Task<IActionResult> FixTable()
        {
            try
            {
                var sql = @"
                    -- Verificar si la tabla necesita ser corregida
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns c 
                        JOIN sys.tables t ON c.object_id = t.object_id 
                        WHERE t.name = 'Log_AuditoriaSalarios' 
                        AND c.name = 'id' 
                        AND c.is_identity = 1
                    )
                    BEGIN
                        -- Crear tabla temporal con estructura correcta
                        CREATE TABLE Log_AuditoriaSalarios_temp (
                            id int IDENTITY(1,1) PRIMARY KEY,
                            usuario nvarchar(50) NOT NULL,
                            fechaActualizacion datetime2 NOT NULL,
                            DetalleCambio nvarchar(250) NOT NULL,
                            salario bigint NOT NULL,
                            emp_no int NOT NULL,
                            FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
                        );

                        -- Copiar datos existentes (si los hay)
                        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Log_AuditoriaSalarios')
                        BEGIN
                            SET IDENTITY_INSERT Log_AuditoriaSalarios_temp ON;
                            INSERT INTO Log_AuditoriaSalarios_temp (id, usuario, fechaActualizacion, DetalleCambio, salario, emp_no)
                            SELECT id, usuario, fechaActualizacion, DetalleCambio, salario, emp_no 
                            FROM Log_AuditoriaSalarios
                            WHERE id IS NOT NULL;
                            SET IDENTITY_INSERT Log_AuditoriaSalarios_temp OFF;
                            
                            -- Eliminar tabla original
                            DROP TABLE Log_AuditoriaSalarios;
                        END

                        -- Renombrar tabla temporal
                        EXEC sp_rename 'Log_AuditoriaSalarios_temp', 'Log_AuditoriaSalarios';
                        
                        SELECT 'Tabla corregida exitosamente' as resultado;
                    END
                    ELSE
                    BEGIN
                        SELECT 'La tabla ya está correctamente configurada' as resultado;
                    END";

                await _context.Database.ExecuteSqlRawAsync(sql);
                TempData["SuccessMessage"] = "Tabla de auditoría de salarios corregida exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al corregir la tabla: {ex.Message}";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
