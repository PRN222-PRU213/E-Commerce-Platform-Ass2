namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO cho Category
    /// </summary>
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}
