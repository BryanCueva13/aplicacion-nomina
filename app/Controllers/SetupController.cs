using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Data;

namespace app.Controllers
{
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SetupController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Setup/CreateAuditTables
        public async Task<IActionResult> CreateAuditTables()
        {
            try
            {
                var sql = @"
                    -- Crear tabla de auditoría general si no existe
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Log_Auditoria_General')
                    BEGIN
                        CREATE TABLE [dbo].[Log_Auditoria_General] (
                            [id] int IDENTITY(1,1) NOT NULL,
                            [usuario] nvarchar(50) NOT NULL,
                            [fechaActualizacion] datetime2(7) NOT NULL DEFAULT GETDATE(),
                            [tipoOperacion] nvarchar(20) NOT NULL,
                            [tabla] nvarchar(50) NOT NULL,
                            [detalleCambio] nvarchar(500) NOT NULL,
                            [registroId] nvarchar(100) NULL,
                            [emp_no] int NULL,
                            [valorAnterior] nvarchar(500) NULL,
                            [valorNuevo] nvarchar(500) NULL,
                            CONSTRAINT [PK_Log_Auditoria_General] PRIMARY KEY CLUSTERED ([id] ASC),
                            CONSTRAINT [FK_Log_Auditoria_General_Employees] FOREIGN KEY([emp_no]) REFERENCES [dbo].[employees]([emp_no])
                        );
                    END

                    -- Verificar/Corregir tabla de auditoría de salarios
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.columns c 
                        JOIN sys.tables t ON c.object_id = t.object_id 
                        WHERE t.name = 'Log_AuditoriaSalarios' 
                        AND c.name = 'id' 
                        AND c.is_identity = 1
                    )
                    BEGIN
                        -- Si la tabla existe pero no tiene IDENTITY, recrearla
                        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Log_AuditoriaSalarios')
                        BEGIN
                            -- Crear tabla temporal con estructura correcta
                            CREATE TABLE [dbo].[Log_AuditoriaSalarios_temp] (
                                [id] int IDENTITY(1,1) NOT NULL,
                                [usuario] nvarchar(50) NOT NULL,
                                [fechaActualizacion] datetime2(7) NOT NULL,
                                [DetalleCambio] nvarchar(250) NOT NULL,
                                [salario] bigint NOT NULL,
                                [emp_no] int NOT NULL,
                                CONSTRAINT [PK_Log_AuditoriaSalarios_temp] PRIMARY KEY CLUSTERED ([id] ASC),
                                CONSTRAINT [FK_Log_AuditoriaSalarios_temp_Employees] FOREIGN KEY([emp_no]) REFERENCES [dbo].[employees]([emp_no])
                            );

                            -- Copiar datos existentes (si los hay)
                            IF EXISTS (SELECT 1 FROM [dbo].[Log_AuditoriaSalarios])
                            BEGIN
                                INSERT INTO [dbo].[Log_AuditoriaSalarios_temp] ([usuario], [fechaActualizacion], [DetalleCambio], [salario], [emp_no])
                                SELECT [usuario], [fechaActualizacion], [DetalleCambio], [salario], [emp_no] 
                                FROM [dbo].[Log_AuditoriaSalarios]
                                WHERE [emp_no] IS NOT NULL;
                            END

                            -- Eliminar tabla original
                            DROP TABLE [dbo].[Log_AuditoriaSalarios];

                            -- Renombrar tabla temporal
                            EXEC sp_rename '[dbo].[Log_AuditoriaSalarios_temp]', 'Log_AuditoriaSalarios';
                        END
                        ELSE
                        BEGIN
                            -- Crear tabla desde cero
                            CREATE TABLE [dbo].[Log_AuditoriaSalarios] (
                                [id] int IDENTITY(1,1) NOT NULL,
                                [usuario] nvarchar(50) NOT NULL,
                                [fechaActualizacion] datetime2(7) NOT NULL,
                                [DetalleCambio] nvarchar(250) NOT NULL,
                                [salario] bigint NOT NULL,
                                [emp_no] int NOT NULL,
                                CONSTRAINT [PK_Log_AuditoriaSalarios] PRIMARY KEY CLUSTERED ([id] ASC),
                                CONSTRAINT [FK_Log_AuditoriaSalarios_Employees] FOREIGN KEY([emp_no]) REFERENCES [dbo].[employees]([emp_no])
                            );
                        END
                    END

                    -- Insertar datos de prueba
                    IF NOT EXISTS (SELECT 1 FROM [dbo].[Log_Auditoria_General] WHERE [tabla] = 'system')
                    BEGIN
                        INSERT INTO [dbo].[Log_Auditoria_General] 
                        ([usuario], [tipoOperacion], [tabla], [detalleCambio], [registroId])
                        VALUES 
                        ('Sistema', 'CREATE', 'system', 'Tablas de auditoría creadas desde la aplicación', 'setup_' + CONVERT(NVARCHAR, GETDATE(), 112));
                    END
                ";

                await _context.Database.ExecuteSqlRawAsync(sql);
                
                TempData["SuccessMessage"] = "Tablas de auditoría creadas/verificadas exitosamente desde la aplicación.";
                ViewBag.Message = "✅ Tablas de auditoría configuradas correctamente";
                ViewBag.Details = "Se crearon/verificaron las tablas Log_Auditoria_General y Log_AuditoriaSalarios";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear tablas: {ex.Message}";
                ViewBag.Message = "❌ Error al configurar auditoría";
                ViewBag.Details = ex.Message;
            }

            return View();
        }

        // GET: Setup - Página principal de configuración
        public IActionResult Index()
        {
            return View();
        }
    }
}
