namespace Identity.Responses
{
    public class AdminUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
