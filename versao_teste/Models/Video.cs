namespace YouTube.Models
{
    /// <summary>
    /// Representa um vídeo publicado no YouTube
    /// Relações:
    /// - N:1 com Account (vários vídeos pertencem a uma conta)
    /// - 1:N com Comment (um vídeo pode ter vários comentários)
    /// </summary>
    public record class Video
    {
        public required Guid Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        // Duração em segundos
        public int DurationInSeconds { get; set; }

        // Contador de visualizações
        public int Views { get; set; } = 0;

        // Data de publicação
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        // Navegação: conta que publicou este vídeo (obrigatório)
        public required Account Account { get; set; }

        // Navegação bidirecional: comentários neste vídeo
        public List<Comment> Comments { get; } = [];

        // Método para adicionar comentário ao vídeo
        public void AddComment(Account author, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Comentário não pode ser vazio");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Text = text,
                Video = this,
                Author = author,
                CreatedAt = DateTime.UtcNow
            };

            this.Comments.Add(comment);
            author.Comments.Add(comment);
        }

        // Método para incrementar visualizações
        public void IncrementView()
        {
            this.Views++;
        }
    }
}