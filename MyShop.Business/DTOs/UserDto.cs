namespace MyShop.Business.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool LockStatus { get; set; } = false;
    }
}
