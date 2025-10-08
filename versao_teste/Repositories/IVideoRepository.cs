using YouTube.Models;

namespace YouTube.Repositories;

/// <summary>
/// Contrato (abstração) para repositório de vídeos
/// </summary>
public interface IVideoRepository
{
    // Salvar um novo vídeo
    void Save(Video video);

    // Buscar todos os vídeos
    IEnumerable<Video> GetAll();

    // Buscar vídeo por ID
    Video? GetById(Guid id);

    // Buscar vídeos de uma conta específica
    IEnumerable<Video> GetByAccountId(Guid accountId);

    // Buscar vídeos mais visualizados
    IEnumerable<Video> GetMostViewed(int limit = 10);

    // Atualizar vídeo existente
    void Update(Video video);

    // Deletar vídeo
    void Delete(Guid id);
}