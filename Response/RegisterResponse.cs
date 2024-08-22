namespace Quan_ly_ban_hang.Response
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
