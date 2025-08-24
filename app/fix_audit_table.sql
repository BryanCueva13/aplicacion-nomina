-- Script para corregir la tabla Log_AuditoriaSalarios
-- Hacer que la columna id sea IDENTITY

-- Paso 1: Crear tabla temporal con estructura correcta
CREATE TABLE Log_AuditoriaSalarios_temp (
    id int IDENTITY(1,1) PRIMARY KEY,
    usuario nvarchar(50) NOT NULL,
    fechaActualizacion datetime2 NOT NULL,
    DetalleCambio nvarchar(250) NOT NULL,
    salario bigint NOT NULL,
    emp_no int NOT NULL,
    FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Paso 2: Copiar datos existentes (si los hay)
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Log_AuditoriaSalarios')
BEGIN
    INSERT INTO Log_AuditoriaSalarios_temp (usuario, fechaActualizacion, DetalleCambio, salario, emp_no)
    SELECT usuario, fechaActualizacion, DetalleCambio, salario, emp_no 
    FROM Log_AuditoriaSalarios;
    
    -- Paso 3: Eliminar tabla original
    DROP TABLE Log_AuditoriaSalarios;
END

-- Paso 4: Renombrar tabla temporal
EXEC sp_rename 'Log_AuditoriaSalarios_temp', 'Log_AuditoriaSalarios';
