namespace YouTube.Models
{
    /// <summary>
    /// Representa um comentário em um vídeo
    /// Relações:
    /// - N:1 com Video (vários comentários pertencem a um vídeo)
    /// - N:1 com Account (vários comentários pertencem a uma conta)
    /// </summary>
    public record class Comment
    {
        public required Guid Id { get; set; }

        public required string Text { get; set; }

        // Data de criação do comentário
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Contador de likes
        public int Likes { get; set; } = 0;

        // Contador de dislikes
        public int Dislikes { get; set; } = 0;

        // Navegação: vídeo onde o comentário foi feito (obrigatório)
        public required Video Video { get; set; }

        // Navegação: autor do comentário (obrigatório)
        public required Account Author { get; set; }

        // Comentário editado?
        public bool IsEdited { get; set; } = false;

        // Métodos auxiliares
        public void AddLike()
        {
            this.Likes++;
        }

        public void AddDislike()
        {
            this.Dislikes++;
        }

        public void Edit(string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new ArgumentException("Texto não pode ser vazio");

            this.Text = newText;
            this.IsEdited = true;
        }
    }
}