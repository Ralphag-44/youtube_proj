using Microsoft.AspNetCore.Mvc;
using YouTube.Models;
using YouTube.Repositories;

namespace WebYouTube.Controllers;

/// <summary>
/// Controller para gerenciar vídeos do YouTube
/// Rota base: /api/v1/videos
/// </summary>
[ApiController]
[Route("api/v1/videos")]
public class VideosController : ControllerBase
{
    private readonly ILogger<VideosController> _logger;
    private readonly IVideoRepository _videoRepo;
    private readonly IAccountRepository _accountRepo;

    public VideosController(
        ILogger<VideosController> logger,
        IVideoRepository videoRepo,
        IAccountRepository accountRepo)
    {
        _logger = logger;
        _videoRepo = videoRepo;
        _accountRepo = accountRepo;
    }

    /// <summary>
    /// POST /api/v1/videos - Publicar novo vídeo
    /// </summary>
    [HttpPost(Name = "PublishVideo")]
    public IActionResult PublishVideo([FromBody] NewVideoDTO newVideo)
    {
        // Verificar se a conta existe
        var account = _accountRepo.GetById(newVideo.AccountId);
        if (account == null)
        {
            return BadRequest($"Conta com ID {newVideo.AccountId} não encontrada");
        }

        // Criar vídeo
        var video = new Video
        {
            Id = Guid.NewGuid(),
            Title = newVideo.Title,
            Description = newVideo.Description,
            DurationInSeconds = newVideo.DurationInSeconds,
            Account = account,
            PublishedAt = DateTime.UtcNow
        };

        _videoRepo.Save(video);
        
        // Adicionar à lista de vídeos da conta
        account.Videos.Add(video);
        _accountRepo.Update(account);

        _logger.LogInformation("Novo vídeo publicado: {Title} por {Username}", 
            video.Title, account.Username);

        return CreatedAtRoute("GetVideoById", new { id = video.Id }, video);
    }

    /// <summary>
    /// GET /api/v1/videos - Listar todos os vídeos
    /// </summary>
    [HttpGet(Name = "GetAllVideos")]
    public ActionResult<IEnumerable<Video>> GetAll()
    {
        var videos = _videoRepo.GetAll();
        return Ok(videos);
    }

    /// <summary>
    /// GET /api/v1/videos/{id} - Buscar vídeo por ID
    /// </summary>
    [HttpGet("{id}", Name = "GetVideoById")]
    public ActionResult<Video> GetById(Guid id)
    {
        var video = _videoRepo.GetById(id);
        
        if (video == null)
        {
            return NotFound($"Vídeo com ID {id} não encontrado");
        }

        return Ok(video);
    }

    /// <summary>
    /// GET /api/v1/videos/account/{accountId} - Listar vídeos de uma conta
    /// </summary>
    [HttpGet("account/{accountId}", Name = "GetVideosByAccount")]
    public ActionResult<IEnumerable<Video>> GetByAccount(Guid accountId)
    {
        var videos = _videoRepo.GetByAccountId(accountId);
        return Ok(videos);
    }

    /// <summary>
    /// GET /api/v1/videos/trending - Vídeos mais visualizados
    /// </summary>
    [HttpGet("trending", Name = "GetTrendingVideos")]
    public ActionResult<IEnumerable<Video>> GetTrending([FromQuery] int limit = 10)
    {
        var videos = _videoRepo.GetMostViewed(limit);
        return Ok(videos);
    }

    /// <summary>
    /// POST /api/v1/videos/{id}/view - Incrementar visualização
    /// </summary>
    [HttpPost("{id}/view", Name = "IncrementVideoView")]
    public IActionResult IncrementView(Guid id)
    {
        var video = _videoRepo.GetById(id);
        
        if (video == null)
        {
            return NotFound($"Vídeo com ID {id} não encontrado");
        }

        video.IncrementView();
        _videoRepo.Update(video);

        return Ok(new { Views = video.Views });
    }

    /// <summary>
    /// PUT /api/v1/videos/{id} - Atualizar vídeo
    /// </summary>
    [HttpPut("{id}", Name = "UpdateVideo")]
    public IActionResult UpdateVideo(Guid id, [FromBody] UpdateVideoDTO updateVideo)
    {
        var video = _videoRepo.GetById(id);
        
        if (video == null)
        {
            return NotFound($"Vídeo com ID {id} não encontrado");
        }

        // Atualizar campos
        video.Title = updateVideo.Title ?? video.Title;
        video.Description = updateVideo.Description ?? video.Description;

        _videoRepo.Update(video);

        return Ok(video);
    }

    /// <summary>
    /// DELETE /api/v1/videos/{id} - Deletar vídeo
    /// </summary>
    [HttpDelete("{id}", Name = "DeleteVideo")]
    public IActionResult DeleteVideo(Guid id)
    {
        var video = _videoRepo.GetById(id);
        
        if (video == null)
        {
            return NotFound($"Vídeo com ID {id} não encontrado");
        }

        _videoRepo.Delete(id);

        return NoContent();
    }
}

// DTOs para as requisições

/// <summary>
/// DTO para publicação de novo vídeo
/// </summary>
public record class NewVideoDTO(
    string Title, 
    string? Description, 
    int DurationInSeconds, 
    Guid AccountId
);

/// <summary>
/// DTO para atualização de vídeo
/// </summary>
public record class UpdateVideoDTO(string? Title, string? Description);