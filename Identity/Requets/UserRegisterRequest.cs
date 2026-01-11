namespace Identity.Requets
{
    public class UserRegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public int Age { get; set; }
        public bool RememberMe { get; set; }
    }
}
