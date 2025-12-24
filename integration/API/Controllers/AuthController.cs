using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using integration.Services;
using integration.Services.Employers.Storage;
using Microsoft.Extensions.Configuration;

namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmployersStorageService _employersStorage;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IEmployersStorageService employersStorage,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _employersStorage = employersStorage;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Аутентификация пользователя по email
        /// </summary>
        /// <param name="request">Email пользователя</param>
        /// <returns>JWT токен</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Email))
                {
                    _logger.LogWarning("Попытка входа без email");
                    return BadRequest(new { Error = "Email обязателен" });
                }

                _logger.LogInformation("Попытка входа для email: {Email}", request.Email);

                // Получаем JWT ключ из конфигурации
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT ключ не настроен в конфигурации");
                    return StatusCode(500, new { Error = "Ошибка конфигурации сервера" });
                }

                // Используем extension метод для генерации токена
                var token = await _employersStorage.GenerateUserTokenAsync(request.Email, jwtKey);

                _logger.LogInformation("Успешный вход для email: {Email}", request.Email);

                return Ok(new
                {
                    Token = token,
                    Message = "Успешная авторизация",
                    ExpiresIn = _configuration.GetValue<int>("Jwt:ExpireMinutes", 480)
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Ошибка авторизации для {Email}: {Message}", request?.Email, ex.Message);
                return Unauthorized(new { Error = "Неверный email или пользователь не найден" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе для {Email}", request?.Email);
                return StatusCode(500, new { Error = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Валидация токена (тестовый endpoint)
        /// </summary>
        [HttpPost("validate")]
        [Authorize]
        public IActionResult Validate()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Error = "Токен невалиден" });
                }

                _logger.LogInformation("Валидация токена для userId: {UserId}", userId);

                return Ok(new
                {
                    UserId = userId,
                    Email = email,
                    Name = name,
                    IsValid = true,
                    ValidatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка валидации токена");
                return Unauthorized(new { Error = "Токен невалиден" });
            }
        }

        /// <summary>
        /// Получение информации о текущем пользователе
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                var position = User.FindFirst("Position")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Получаем полную информацию о сотруднике
                var staff = _employersStorage.Get();
                var employee = staff?.FirstOrDefault(e =>
                    e.user?.id.ToString() == userId);

                return Ok(new
                {
                    UserId = userId,
                    Email = email,
                    Name = name,
                    Position = position,
                    EmployeeId = employee?.id,
                    Department = employee?.department,
                    Phone = employee?.phone,
                    IsActive = employee?.is_active ?? false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения информации о пользователе");
                return StatusCode(500, new { Error = "Внутренняя ошибка" });
            }
        }

        /// <summary>
        /// Проверка доступности email (можно ли залогиниться)
        /// </summary>
        [HttpPost("check-email")]
        public IActionResult CheckEmail([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Email))
                {
                    return BadRequest(new { Error = "Email обязателен" });
                }

                var staff = _employersStorage.Get();
                var exists = staff?.Any(e =>
                    e.email?.Equals(request.Email, StringComparison.OrdinalIgnoreCase) == true) ?? false;

                return Ok(new
                {
                    Email = request.Email,
                    Exists = exists,
                    Message = exists ? "Email найден в системе" : "Email не найден"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка проверки email");
                return StatusCode(500, new { Error = "Внутренняя ошибка" });
            }
        }
    }

    /// <summary>
    /// Модель запроса для входа
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Модель ответа с токеном
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}