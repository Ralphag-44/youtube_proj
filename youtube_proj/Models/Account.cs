namespace YouTube.Models
{
    public record class Account
    {
        public Guid Id { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        // Data de criação da conta
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegação bidirecional: vídeos publicados por esta conta
        public List<Video> Videos { get; } = [];

        // Navegação bidirecional: comentários feitos por esta conta
        public List<Comment> Comments { get; } = [];

        // Método auxiliar para adicionar vídeo
        public void PublishVideo(string title, string description = "")
        {
            var video = new Video
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Account = this,
                PublishedAt = DateTime.UtcNow
            };
            this.Videos.Add(video);
        }
    }
}