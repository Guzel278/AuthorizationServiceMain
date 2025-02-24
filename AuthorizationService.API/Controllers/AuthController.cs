using AuthorizationService.API.Requests;
using AuthorizationService.Application.Interfaces;
using AuthorizationService.Domain.Models;
using AuthorizationService.Infrastructure.MessageBroker;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.API.Controllers
{
    [ApiController]
    [EnableCors("AllowReactApp")]
    [Route("api/[controller]")]
    public class AuthController(IUserService userService, IRabbitMqService rabbitMqService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {

            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                return NotFound(new { error = $"Role '{request.Role}' does not exist." });

            // Преобразуем enum в ID роли
            int roleId = (int)role;

            // Проверяем, существует ли роль в базе данных
            var roleExist = await userService.GetRoleByIdAsync(roleId);
            if (roleExist == null)
                return NotFound(new { error = $"Role '{request.Role}' does not exist." });

            // Создаём нового пользователя
            var user = new User(request.Username, request.Email, request.Email, roleId);

            var result = await userService.RegisterUserAsync(user, request.Password);

            if (!result)
                return Conflict(new { error = "User already exists." });

            // Создаём объект для отправки в RabbitMQ
            var notification = new
            {
                UserId = user.Id,
                Email = user.Email,
                Message = "Welcome! Your registration was successful.",
                Type = "Registration"
            };

             rabbitMqService.Send(notification, "notification_queue"); // Отправляем сообщение

            return CreatedAtAction(nameof(Register), new { userId = user.Id }, new { userId = user.Id });
        }

        [HttpPost("login")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Проверка пользователя
            var user = await userService.ValidateUserAsync(request.Username, request.Password);
            if (user == null)
                return Unauthorized(new { error = "Invalid username or password." });

            // Генерация токена
            var token = userService.GenerateJwtToken(user);

            // Сохранение токена в базе
            await userService.SaveUserTokenAsync(user.Id, token);

            // Логирование в auditLogs
            await userService.LogUserActionAsync(user.Id, "User logged in");

            return Ok(new
            {
                userId = user.Id,
                username = user.Username,
                role = user.Role.Name,
                token
            });
        }

        [HttpPost("logout")]
        [EnableCors("AllowReactApp")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var success = await userService.RevokeUserTokenAsync(request.Token);
            if (!success)
                return BadRequest(new { error = "Invalid or expired token." });

            await userService.LogUserActionAsync(request.UserId, "User logged out");
            return Ok(new
            {
                userId = request.UserId,
                token = request.Token,
                message = "Successfully logged out."
            });
        }
    }
}
