using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using app.Data;
using app.Models;
using app.Services;
using app.ViewModels;

namespace app.Controllers
{
    /// <summary>
    /// Controlador para manejo de autenticación y cuentas de usuario
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AccountController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Página de login
        /// </summary>
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// Procesar login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Buscar usuario en la base de datos
            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !_authService.VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(model);
            }

            // Crear claims para el usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.EmpNo.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("EmployeeName", user.Employee.FullName),
                new Claim("Role", user.Role),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirigir a la URL de retorno o al dashboard
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Logout
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Página de acceso denegado
        /// </summary>
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Cambiar contraseña (para usuarios autenticados)
        /// </summary>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        /// <summary>
        /// Página de registro de nuevos usuarios
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        /// <summary>
        /// Procesar registro de nuevo usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verificar si el email ya existe
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == model.Email);

            if (existingEmployee != null)
            {
                ModelState.AddModelError("Email", "Ya existe un empleado con este email.");
                return View(model);
            }

            // Verificar si la CI ya existe
            var existingCI = await _context.Employees
                .FirstOrDefaultAsync(e => e.CI == model.CI);

            if (existingCI != null)
            {
                ModelState.AddModelError("CI", "Ya existe un empleado con esta cédula.");
                return View(model);
            }

            // Verificar si el nombre de usuario ya existe
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Este nombre de usuario ya está en uso.");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generar nuevo número de empleado
                var maxEmpNo = await _context.Employees.MaxAsync(e => (int?)e.EmpNo) ?? 1000;
                var newEmpNo = maxEmpNo + 1;

                // Crear empleado
                var employee = new Employee
                {
                    EmpNo = newEmpNo,
                    CI = model.CI,
                    BirthDate = model.BirthDate.ToString("yyyy-MM-dd"),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    HireDate = model.HireDate.ToString("yyyy-MM-dd"),
                    Email = model.Email
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Crear usuario
                var user = new User
                {
                    EmpNo = newEmpNo,
                    Username = model.Username,
                    PasswordHash = _authService.HashPassword(model.Password),
                    Role = "RRHH" // Rol por defecto
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Usuario {model.Username} registrado exitosamente. Ya puedes iniciar sesión.";
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Error al crear el usuario. Inténtalo nuevamente.");
                return View(model);
            }
        }

        /// <summary>
        /// Procesar cambio de contraseña
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var empNo = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FindAsync(empNo);

            if (user == null)
            {
                return NotFound();
            }

            // Verificar contraseña actual
            if (!_authService.VerifyPassword(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("CurrentPassword", "La contraseña actual es incorrecta.");
                return View(model);
            }

            // Actualizar contraseña
            user.PasswordHash = _authService.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }

    /// <summary>
    /// ViewModel para cambio de contraseña
    /// </summary>
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
