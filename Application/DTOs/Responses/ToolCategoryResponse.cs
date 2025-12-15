namespace Application.DTOs.Responses
{
    public class ToolCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ToolCount { get; set; }
    }
}
