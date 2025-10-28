using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class CommentMemoryStore
    {
        private static readonly Dictionary<string, List<CommentDto>> _comments = new();
        private static int _nextId = 1;

        public static List<CommentDto> GetByArticle(string articleId)
        {
            if (!_comments.ContainsKey(articleId))
                _comments[articleId] = new List<CommentDto>();
            return _comments[articleId];
        }

        public static CommentDto AddComment(string articleId, string content, string author)
        {
            var comment = new CommentDto
            {
                Id = _nextId++,
                ArticleId = articleId,
                AuthorName = author,
                Content = content
            };
            GetByArticle(articleId).Add(comment);
            return comment;
        }

        public static bool UpdateComment(string articleId, int commentId, string newContent)
        {
            var comment = GetByArticle(articleId).FirstOrDefault(c => c.Id == commentId);
            if (comment == null) return false;
            comment.Content = newContent;
            comment.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public static bool DeleteComment(string articleId, int commentId)
        {
            var list = GetByArticle(articleId);
            var cmt = list.FirstOrDefault(c => c.Id == commentId);
            if (cmt == null) return false;
            list.Remove(cmt);
            return true;
        }
    }
}
