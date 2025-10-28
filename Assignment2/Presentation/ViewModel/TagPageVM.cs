namespace Presentation.ViewModel
{
    public class TagPageVM
    {
        public List<DataAccess.Models.Tag> Tags { get; set; } = new();
        public TagVM TagVM { get; set; } = new();
    }
}
