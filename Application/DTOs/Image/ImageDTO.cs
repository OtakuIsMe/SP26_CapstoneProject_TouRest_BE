using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Image
{
    public class ImageDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public ImageType Type { get; set; }
        public Guid TypeId { get; set; }
        public int PicNumber { get; set; }
    }
}
