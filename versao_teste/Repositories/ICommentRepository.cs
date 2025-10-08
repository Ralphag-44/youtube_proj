using YouTube.Models;

namespace YouTube.Repositories;

/// <summary>
/// Contrato (abstração) para repositório de comentários
/// </summary>
public interface ICommentRepository
{
    // Salvar um novo comentário
    void Save(Comment comment);

    // Buscar todos os comentários
    IEnumerable<Comment> GetAll();

    // Buscar comentário por ID
    Comment? GetById(Guid id);

    // Buscar comentários de um vídeo específico
    IEnumerable<Comment> GetByVideoId(Guid videoId);

    // Buscar comentários de um autor específico
    IEnumerable<Comment> GetByAuthorId(Guid authorId);

    // Atualizar comentário existente
    void Update(Comment comment);

    // Deletar comentário
    void Delete(Guid id);
}