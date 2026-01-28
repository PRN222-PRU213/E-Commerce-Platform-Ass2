namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO tạo mới Category
    /// </summary>
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }

    /// <summary>
    /// DTO cập nhật Category
    /// </summary>
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
