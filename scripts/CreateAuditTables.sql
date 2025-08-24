-- Script para crear tablas de auditoría en SQL Server
-- Ejecutar este script en SQL Server Management Studio o cualquier cliente SQL

USE [programa]; -- Cambia por el nombre de tu base de datos
GO

-- 1. Crear tabla de auditoría general para todos los cambios
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
    
    PRINT 'Tabla Log_Auditoria_General creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Log_Auditoria_General ya existe';
END
GO

-- 2. Verificar/Corregir tabla de auditoría de salarios
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
            SET IDENTITY_INSERT [dbo].[Log_AuditoriaSalarios_temp] ON;
            
            INSERT INTO [dbo].[Log_AuditoriaSalarios_temp] ([id], [usuario], [fechaActualizacion], [DetalleCambio], [salario], [emp_no])
            SELECT [id], [usuario], [fechaActualizacion], [DetalleCambio], [salario], [emp_no] 
            FROM [dbo].[Log_AuditoriaSalarios]
            WHERE [id] IS NOT NULL;
            
            SET IDENTITY_INSERT [dbo].[Log_AuditoriaSalarios_temp] OFF;
        END

        -- Eliminar tabla original
        DROP TABLE [dbo].[Log_AuditoriaSalarios];

        -- Renombrar tabla temporal
        EXEC sp_rename '[dbo].[Log_AuditoriaSalarios_temp]', 'Log_AuditoriaSalarios';
        EXEC sp_rename 'PK_Log_AuditoriaSalarios_temp', 'PK_Log_AuditoriaSalarios';
        EXEC sp_rename 'FK_Log_AuditoriaSalarios_temp_Employees', 'FK_Log_AuditoriaSalarios_Employees';
        
        PRINT 'Tabla Log_AuditoriaSalarios corregida exitosamente';
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
        
        PRINT 'Tabla Log_AuditoriaSalarios creada exitosamente';
    END
END
ELSE
BEGIN
    PRINT 'La tabla Log_AuditoriaSalarios ya está correctamente configurada';
END
GO

-- 3. Insertar algunos datos de prueba para verificar que funciona
INSERT INTO [dbo].[Log_Auditoria_General] 
([usuario], [fechaActualizacion], [tipoOperacion], [tabla], [detalleCambio], [registroId], [emp_no])
VALUES 
('Sistema', GETDATE(), 'CREATE', 'employees', 'Tabla de auditoría general inicializada', 'system_init', NULL),
('Sistema', GETDATE(), 'CREATE', 'system', 'Sistema de auditoría configurado correctamente', 'audit_setup', NULL);

PRINT 'Datos de prueba insertados';

-- 4. Verificar que las tablas se crearon correctamente
SELECT 'Log_Auditoria_General' as Tabla, COUNT(*) as Registros FROM [dbo].[Log_Auditoria_General]
UNION ALL
SELECT 'Log_AuditoriaSalarios' as Tabla, COUNT(*) as Registros FROM [dbo].[Log_AuditoriaSalarios];

PRINT 'Script de auditoría completado exitosamente';
