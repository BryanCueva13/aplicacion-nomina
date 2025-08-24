-- Crear la base de datos
CREATE DATABASE programa;


-- Usar la base de datos
USE programa;


-- ===============================
-- TABLA: employees
-- ===============================
CREATE TABLE employees (
    emp_no INT PRIMARY KEY,
    ci VARCHAR(50) NOT NULL,
    birth_date VARCHAR(50) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    gender CHAR(1) NOT NULL,
    hire_date VARCHAR(50) NOT NULL,
    correo VARCHAR(100) NOT NULL
);

-- ===============================
-- TABLA: departments
-- ===============================
CREATE TABLE departments (
    dept_no INT PRIMARY KEY,
    dept_name VARCHAR(50) NOT NULL
);

-- ===============================
-- TABLA: dept_emp
-- ===============================
CREATE TABLE dept_emp (
    emp_no INT NOT NULL,
    dept_no INT NOT NULL,
    from_date VARCHAR(50) NOT NULL,
    to_date VARCHAR(50) NOT NULL,
    PRIMARY KEY (emp_no, dept_no),
    FOREIGN KEY (emp_no) REFERENCES employees(emp_no),
    FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);

-- ===============================
-- TABLA: dept_manager
-- ===============================
CREATE TABLE dept_manager (
    emp_no INT NOT NULL,
    dept_no INT NOT NULL,
    from_date VARCHAR(50) NOT NULL,
    to_date VARCHAR(50) NOT NULL,
    PRIMARY KEY (emp_no, dept_no),
    FOREIGN KEY (emp_no) REFERENCES employees(emp_no),
    FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);

-- ===============================
-- TABLA: titles
-- ===============================
CREATE TABLE titles (
    emp_no INT NOT NULL,
    title VARCHAR(50) NOT NULL,
    from_date VARCHAR(50) NOT NULL,
    to_date VARCHAR(50) NULL,
    PRIMARY KEY (emp_no, title, from_date),
    FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- ===============================
-- TABLA: salaries
-- ===============================
CREATE TABLE salaries (
    emp_no INT NOT NULL,
    salary BIGINT NOT NULL,
    from_date VARCHAR(50) NOT NULL,
    to_date VARCHAR(50) NOT NULL,
    PRIMARY KEY (emp_no, from_date),
    FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- ===============================
-- TABLA: users
-- ===============================
CREATE TABLE users (
    emp_no INT PRIMARY KEY,
    usuario VARCHAR(100) NOT NULL,
    clave VARCHAR(100) NOT NULL,
    FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- ===============================
-- TABLA: Log_AuditoriaSalarios
-- ===============================
CREATE TABLE Log_AuditoriaSalarios (
    id INT PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL,
    fechaActualizacion DATE NOT NULL,
    DetalleCambio VARCHAR(250) NOT NULL,
    salario BIGINT NOT NULL,
    emp_no INT NOT NULL,
    FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);