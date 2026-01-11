namespace Identity.Requets
{
    public class RefreshTokenRequest
    {
        public string? Token { get; set; }
        public bool RememberMe { get; set; }
    }
}
