# 📋 Sistema de Gestión de Empleados y Nómina

## 🏢 Descripción General

Sistema integral de gestión de recursos humanos desarrollado en **ASP.NET Core 8.0** con **Entity Framework Core**, diseñado para administrar empleados, departamentos, salarios, títulos y generar reportes de nómina y estructura organizacional.

## ✨ Características Principales

### 👥 Gestión de Empleados
- **Registro completo de empleados** con información personal y laboral
- **Gestión de usuarios** vinculados a empleados
- **Historial de cambios** y auditoría completa
- **Búsqueda y filtrado** avanzado
- **Validación de datos** (CI, email, fechas)

### 🏢 Gestión de Departamentos
- **Creación y administración** de departamentos
- **Asignación de gerentes** por departamento
- **Asignación de empleados** a departamentos
- **Historial de asignaciones** con fechas de inicio y fin
- **Visualización de estructura** organizacional

### 💰 Gestión de Salarios
- **Registro de salarios** por empleado
- **Historial salarial** completo
- **Auditoría de cambios** salariales
- **Moneda en dólares** ($USD)
- **Validación de rangos** salariales

### 🎯 Gestión de Títulos/Cargos
- **Asignación de títulos** y cargos
- **Historial de promociones** y cambios
- **Fechas de vigencia** de cada título
- **Múltiples títulos** por empleado

### 📊 Sistema de Reportes
- **Reporte de Nómina** completo con totales
- **Estructura Organizacional** por departamentos
- **Exportación a CSV** de datos de nómina
- **Estadísticas generales** y métricas

### 🔐 Seguridad y Autenticación
- **Sistema de autenticación** personalizado
- **Roles de usuario** (Administrador, RRHH)
- **Encriptación de contraseñas** con BCrypt
- **Sesiones seguras** y control de acceso

### 📋 Auditoría Completa
- **Log general** de todos los cambios
- **Auditoría específica** de salarios
- **Trazabilidad completa** de modificaciones
- **Información de usuario** y fecha de cambios

## 🛠️ Tecnologías Utilizadas

### Backend
- **ASP.NET Core 8.0** - Framework web principal
- **Entity Framework Core 8.0** - ORM para base de datos
- **SQL Server** - Base de datos principal
- **C#** - Lenguaje de programación

### Frontend
- **Razor Pages** - Motor de vistas
- **Bootstrap 5** - Framework CSS
- **jQuery** - Biblioteca JavaScript
- **Bootstrap Icons** - Iconografía

### Seguridad
- **BCrypt.Net-Next** - Encriptación de contraseñas
- **ASP.NET Core Identity** - Gestión de identidades

### Utilidades
- **AutoMapper** - Mapeo de objetos
- **EPPlus** - Exportación Excel
- **iTextSharp** - Generación PDF

## 📁 Estructura del Proyecto

```
app/
├── Controllers/          # Controladores MVC
│   ├── AccountController.cs
│   ├── AuditController.cs
│   ├── DepartmentController.cs
│   ├── EmployeesController.cs
│   ├── HomeController.cs
│   ├── ReportsController.cs
│   ├── SalariesController.cs
│   ├── SetupController.cs
│   └── TitlesController.cs
├── Data/                 # Contexto y configuración DB
│   ├── ApplicationDbContext.cs
│   └── Scripts/
├── Models/               # Modelos de entidades
│   ├── Employee.cs
│   ├── Department.cs
│   ├── Salary.cs
│   ├── Title.cs
│   ├── User.cs
│   ├── AuditLog.cs
│   └── GeneralAuditLog.cs
├── ViewModels/           # ViewModels para vistas
│   └── CommonViewModels.cs
├── Services/             # Servicios de negocio
│   ├── AuditService.cs
│   ├── AuthService.cs
│   ├── DataSeederService.cs
│   └── ValidationService.cs
├── Views/                # Vistas Razor
│   ├── Account/
│   ├── Audit/
│   ├── Department/
│   ├── Employees/
│   ├── Home/
│   ├── Reports/
│   ├── Salaries/
│   ├── Setup/
│   ├── Titles/
│   └── Shared/
├── wwwroot/              # Archivos estáticos
│   ├── css/
│   ├── js/
│   └── lib/
└── Migrations/           # Migraciones EF Core
```

## 🗄️ Estructura de Base de Datos

### Tablas Principales
- **employees** - Información de empleados
- **departments** - Departamentos de la empresa
- **dept_emp** - Relación empleado-departamento
- **dept_manager** - Gerentes de departamento
- **salaries** - Historial de salarios
- **titles** - Títulos y cargos
- **users** - Usuarios del sistema

### Tablas de Auditoría
- **Log_Auditoria_General** - Auditoría general del sistema
- **Log_AuditoriaSalarios** - Auditoría específica de salarios

## 🚀 Instalación y Configuración

### Prerrequisitos
- **.NET 8.0 SDK**
- **SQL Server** (Local o remoto)
- **Visual Studio 2022** o **VS Code**

### Pasos de Instalación

1. **Clonar o descargar** el proyecto
```bash
git clone [repositorio]
cd aplicacion-nomina/app
```

2. **Configurar conexión** a base de datos
```json
// En appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=programa;User Id=sa;Password=tu_password;TrustServerCertificate=true;MultipleActiveResultSets=true;"
  }
}
```

3. **Restaurar paquetes** NuGet
```bash
dotnet restore
```

4. **Ejecutar migraciones** (si existen)
```bash
dotnet ef database update
```

5. **Ejecutar la aplicación**
```bash
dotnet run
```

6. **Acceder al sistema**
- URL: `http://localhost:5046`
- Crear primer usuario administrador desde el sistema

## 📱 Funcionalidades Detalladas

### 🏠 Dashboard Principal
- **Estadísticas generales** del sistema
- **Empleados recientes** contratados
- **Cambios de auditoría** más recientes
- **Empleados por departamento**
- **Próximos aniversarios** laborales

### 👤 Gestión de Empleados
- **Listado paginado** con búsqueda
- **Creación/edición** de empleados
- **Información completa**: CI, nombres, fechas, email
- **Creación automática** de usuarios
- **Validaciones** de datos únicos

### 🏢 Gestión de Departamentos
- **CRUD completo** de departamentos
- **Asignación de empleados** con fechas
- **Designación de gerentes**
- **Vista de asignaciones** actuales e históricas

### 💸 Gestión de Salarios
- **Registro de salarios** por empleado
- **Historial completo** de cambios
- **Auditoría automática** de modificaciones
- **Validaciones** de montos y fechas

### 🎖️ Gestión de Títulos
- **Asignación de cargos** y títulos
- **Fechas de vigencia** flexibles
- **Historial de promociones**
- **Múltiples títulos** simultáneos

### 📊 Reportes Avanzados

#### Reporte de Nómina
- **Listado completo** de empleados activos
- **Salarios actuales** por departamento
- **Totales y subtotales** automáticos
- **Exportación CSV** integrada

#### Estructura Organizacional
- **Vista jerárquica** por departamentos
- **Gerentes y empleados** claramente identificados
- **Estadísticas** organizacionales
- **Funcionalidad de impresión**

### 🔍 Sistema de Auditoría
- **Registro automático** de todos los cambios
- **Información detallada**: usuario, fecha, cambios
- **Auditoría específica** para salarios
- **Consulta y filtrado** de logs

### 🔐 Seguridad
- **Autenticación obligatoria** en todo el sistema
- **Encriptación BCrypt** para contraseñas
- **Roles diferenciados** (Admin/RRHH)
- **Sesiones seguras** con timeout automático

## 🎯 Casos de Uso Principales

### Para Administradores
1. **Configuración inicial** del sistema
2. **Gestión de usuarios** y permisos
3. **Supervisión general** del sistema
4. **Acceso a todas** las funcionalidades

### Para Personal RRHH
1. **Gestión diaria** de empleados
2. **Actualización** de salarios y títulos
3. **Asignación** a departamentos
4. **Generación** de reportes

### Para Gerentes
1. **Consulta** de información de empleados
2. **Visualización** de estructura organizacional
3. **Acceso** a reportes departamentales

## 📈 Métricas y Estadísticas

El sistema proporciona:
- **Total de empleados** activos
- **Número de departamentos**
- **Usuarios** del sistema
- **Empleados nuevos** por período
- **Distribución** por departamento
- **Nómina total** en dólares

## 🔄 Flujos de Trabajo

### Incorporación de Empleado
1. Registro en módulo de empleados
2. Creación opcional de usuario
3. Asignación a departamento
4. Definición de título/cargo
5. Establecimiento de salario
6. Auditoría automática

### Cambio Organizacional
1. Modificación en departamentos
2. Reasignación de empleados
3. Actualización de gerencias
4. Registro en auditoría
5. Actualización de reportes

## 🛡️ Consideraciones de Seguridad

- **Validación** de entrada en todos los formularios
- **Sanitización** de datos antes de almacenar
- **Encriptación** de contraseñas
- **Prevención** de inyección SQL mediante EF Core
- **Validación** de autorización en controladores

## 🚀 Próximas Mejoras

- [ ] **API REST** para integración externa
- [ ] **Exportación** a PDF de reportes
- [ ] **Gráficos** y visualizaciones avanzadas
- [ ] **Notificaciones** automáticas
- [ ] **Módulo de vacaciones** y permisos
- [ ] **Calculadora** de nómina automática

## 📞 Soporte y Contacto

Para soporte técnico o consultas sobre el sistema:
- Revisar la documentación en código
- Verificar logs de aplicación
- Consultar auditoría del sistema

## 📄 Licencia

Proyecto desarrollado para gestión interna de recursos humanos.

---

**Versión:** 1.0  
**Última actualización:** Agosto 2025  
**Framework:** ASP.NET Core 8.0  
**Base de datos:** SQL Server
