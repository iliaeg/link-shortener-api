namespace LinkShortenerAPI.Helpers
{
    public static class PasswordWithSaltHasher
    {
        public static string HashWithSaltGenerate(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
        }

        public static bool HashWithSaltVerify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
