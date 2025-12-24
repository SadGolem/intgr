// В любое место в проекте (например, новый файл AuthExtensions.cs):
namespace integration.Services
{
    public static class AuthExtensions
    {
        public static async Task<string> GenerateUserTokenAsync(
            this IEmployersStorageService employersStorage,
            string email,
            string jwtSecretKey)
        {
            var staff = employersStorage.Get();
            var employee = staff?.FirstOrDefault(e =>
                e.email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true);

            if (employee?.user?.id == null)
                throw new UnauthorizedAccessException("Пользователь не найден");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.user.id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, employee.user.name ?? "Unknown")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}