-- Script de inicialización para la base de datos del sistema de gestión de empleados
-- Ejecutar este script después de crear la base de datos con tu script original

USE programa;
GO

-- Insertar datos de prueba

-- Insertar empleados de prueba
INSERT INTO employees (emp_no, ci, birth_date, first_name, last_name, gender, hire_date, correo) VALUES
(1001, '12345678', '1985-05-15', 'Juan', 'Pérez', 'M', '2020-01-15', 'juan.perez@empresa.com'),
(1002, '87654321', '1990-08-22', 'María', 'González', 'F', '2019-03-10', 'maria.gonzalez@empresa.com'),
(1003, '11223344', '1988-12-03', 'Carlos', 'Rodríguez', 'M', '2021-06-01', 'carlos.rodriguez@empresa.com'),
(1004, '44332211', '1992-04-18', 'Ana', 'Martínez', 'F', '2020-09-15', 'ana.martinez@empresa.com'),
(1005, '55667788', '1987-11-27', 'Roberto', 'López', 'M', '2018-02-20', 'roberto.lopez@empresa.com');

-- Insertar departamentos
INSERT INTO departments (dept_no, dept_name) VALUES
(1, 'Recursos Humanos'),
(2, 'Tecnología'),
(3, 'Ventas'),
(4, 'Marketing'),
(5, 'Administración');

-- Insertar asignaciones de empleados a departamentos
INSERT INTO dept_emp (emp_no, dept_no, from_date, to_date) VALUES
(1001, 1, '2020-01-15', '9999-12-31'),
(1002, 2, '2019-03-10', '9999-12-31'),
(1003, 2, '2021-06-01', '9999-12-31'),
(1004, 3, '2020-09-15', '9999-12-31'),
(1005, 5, '2018-02-20', '9999-12-31');

-- Insertar gerentes de departamentos
INSERT INTO dept_manager (emp_no, dept_no, from_date, to_date) VALUES
(1001, 1, '2020-01-15', '9999-12-31'),
(1002, 2, '2019-03-10', '9999-12-31'),
(1005, 5, '2018-02-20', '9999-12-31');

-- Insertar títulos/cargos
INSERT INTO titles (emp_no, title, from_date, to_date) VALUES
(1001, 'Gerente de RRHH', '2020-01-15', NULL),
(1002, 'Jefe de Tecnología', '2019-03-10', NULL),
(1003, 'Desarrollador Senior', '2021-06-01', NULL),
(1004, 'Ejecutiva de Ventas', '2020-09-15', NULL),
(1005, 'Gerente Administrativo', '2018-02-20', NULL);

-- Insertar salarios
INSERT INTO salaries (emp_no, salary, from_date, to_date) VALUES
(1001, 8000000, '2020-01-15', '9999-12-31'),
(1002, 12000000, '2019-03-10', '9999-12-31'),
(1003, 9000000, '2021-06-01', '9999-12-31'),
(1004, 6500000, '2020-09-15', '9999-12-31'),
(1005, 7500000, '2018-02-20', '9999-12-31');

-- Insertar usuarios del sistema (contraseñas hasheadas con BCrypt)
-- Contraseña por defecto para todos: "123456"
INSERT INTO users (emp_no, usuario, clave) VALUES
(1001, 'admin', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi'), -- 123456
(1002, 'maria.gonzalez', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi'), -- 123456
(1005, 'roberto.lopez', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi'); -- 123456

-- Insertar algunos registros de auditoría de ejemplo
INSERT INTO Log_AuditoriaSalarios (id, usuario, fechaActualizacion, DetalleCambio, salario, emp_no) VALUES
(1, 'admin', '2025-01-15', 'Ajuste salarial anual', 8000000, 1001),
(2, 'admin', '2025-02-01', 'Promoción a Jefe de Tecnología', 12000000, 1002),
(3, 'maria.gonzalez', '2025-02-15', 'Aumento por desempeño', 9000000, 1003);

PRINT 'Datos de prueba insertados correctamente';
PRINT 'Usuarios creados:';
PRINT '  admin / 123456 (Administrador)';
PRINT '  maria.gonzalez / 123456 (RRHH)';
PRINT '  roberto.lopez / 123456 (RRHH)';
GO
