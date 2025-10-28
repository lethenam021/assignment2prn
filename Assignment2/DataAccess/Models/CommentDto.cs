using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CommentDto
    {
        public int Id { get; set; }             // Mã định danh (tự tăng)
        public string? ArticleId { get; set; }      // ID bài viết liên quan
        public string AuthorName { get; set; } = "Ẩn danh";  // Tên người bình luận
        public string Content { get; set; } = "";             // Nội dung bình luận
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Thời điểm tạo
        public DateTime? UpdatedAt { get; set; }               // Thời điểm cập nhật (nếu có)
    }
}
