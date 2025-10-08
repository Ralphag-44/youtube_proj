using Microsoft.AspNetCore.Mvc;
using YouTube.Models;
using YouTube.Repositories;

namespace WebYouTube.Controllers;

/// <summary>
/// Controller para gerenciar comentários nos vídeos
/// Rota base: /api/v1/comments
/// </summary>
[ApiController]
[Route("api/v1/comments")]
public class CommentsController : ControllerBase
{
    private readonly ILogger<CommentsController> _logger;
    private readonly ICommentRepository _commentRepo;
    private readonly IVideoRepository _videoRepo;
    private readonly IAccountRepository _accountRepo;

    public CommentsController(
        ILogger<CommentsController> logger,
        ICommentRepository commentRepo,
        IVideoRepository videoRepo,
        IAccountRepository accountRepo)
    {
        _logger = logger;
        _commentRepo = commentRepo;
        _videoRepo = videoRepo;
        _accountRepo = accountRepo;
    }

    /// <summary>
    /// POST /api/v1/comments - Criar novo comentário
    /// </summary>
    [HttpPost(Name = "CreateComment")]
    public IActionResult CreateComment([FromBody] NewCommentDTO newComment)
    {
        // Verificar se o vídeo existe
        var video = _videoRepo.GetById(newComment.VideoId);
        if (video == null)
        {
            return BadRequest($"Vídeo com ID {newComment.VideoId} não encontrado");
        }

        // Verificar se a conta existe
        var author = _accountRepo.GetById(newComment.AuthorId);
        if (author == null)
        {
            return BadRequest($"Conta com ID {newComment.AuthorId} não encontrada");
        }

        // Criar comentário
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Text = newComment.Text,
            Video = video,
            Author = author,
            CreatedAt = DateTime.UtcNow
        };

        _commentRepo.Save(comment);
        
        // Adicionar às listas de navegação
        video.Comments.Add(comment);
        author.Comments.Add(comment);
        
        _videoRepo.Update(video);
        _accountRepo.Update(author);

        _logger.LogInformation("Novo comentário criado no vídeo {VideoTitle} por {Username}", 
            video.Title, author.Username);

        return CreatedAtRoute("GetCommentById", new { id = comment.Id }, comment);
    }

    /// <summary>
    /// GET /api/v1/comments - Listar todos os comentários
    /// </summary>
    [HttpGet(Name = "GetAllComments")]
    public ActionResult<IEnumerable<Comment>> GetAll()
    {
        var comments = _commentRepo.GetAll();
        return Ok(comments);
    }

    /// <summary>
    /// GET /api/v1/comments/{id} - Buscar comentário por ID
    /// </summary>
    [HttpGet("{id}", Name = "GetCommentById")]
    public ActionResult<Comment> GetById(Guid id)
    {
        var comment = _commentRepo.GetById(id);
        
        if (comment == null)
        {
            return NotFound($"Comentário com ID {id} não encontrado");
        }

        return Ok(comment);
    }

    /// <summary>
    /// GET /api/v1/comments/video/{videoId} - Listar comentários de um vídeo
    /// </summary>
    [HttpGet("video/{videoId}", Name = "GetCommentsByVideo")]
    public ActionResult<IEnumerable<Comment>> GetByVideo(Guid videoId)
    {
        var comments = _commentRepo.GetByVideoId(videoId);
        return Ok(comments);
    }

    /// <summary>
    /// GET /api/v1/comments/author/{authorId} - Listar comentários de um autor
    /// </summary>
    [HttpGet("author/{authorId}", Name = "GetCommentsByAuthor")]
    public ActionResult<IEnumerable<Comment>> GetByAuthor(Guid authorId)
    {
        var comments = _commentRepo.GetByAuthorId(authorId);
        return Ok(comments);
    }

    /// <summary>
    /// PUT /api/v1/comments/{id} - Editar comentário
    /// </summary>
    [HttpPut("{id}", Name = "EditComment")]
    public IActionResult EditComment(Guid id, [FromBody] EditCommentDTO editComment)
    {
        var comment = _commentRepo.GetById(id);
        
        if (comment == null)
        {
            return NotFound($"Comentário com ID {id} não encontrado");
        }

        // Editar comentário
        comment.Edit(editComment.Text);
        _commentRepo.Update(comment);

        return Ok(comment);
    }

    /// <summary>
    /// POST /api/v1/comments/{id}/like - Adicionar like ao comentário
    /// </summary>
    [HttpPost("{id}/like", Name = "LikeComment")]
    public IActionResult LikeComment(Guid id)
    {
        var comment = _commentRepo.GetById(id);
        
        if (comment == null)
        {
            return NotFound($"Comentário com ID {id} não encontrado");
        }

        comment.AddLike();
        _commentRepo.Update(comment);

        return Ok(new { Likes = comment.Likes });
    }

    /// <summary>
    /// POST /api/v1/comments/{id}/dislike - Adicionar dislike ao comentário
    /// </summary>
    [HttpPost("{id}/dislike", Name = "DislikeComment")]
    public IActionResult DislikeComment(Guid id)
    {
        var comment = _commentRepo.GetById(id);
        
        if (comment == null)
        {
            return NotFound($"Comentário com ID {id} não encontrado");
        }

        comment.AddDislike();
        _commentRepo.Update(comment);

        return Ok(new { Dislikes = comment.Dislikes });
    }

    /// <summary>
    /// DELETE /api/v1/comments/{id} - Deletar comentário
    /// </summary>
    [HttpDelete("{id}", Name = "DeleteComment")]
    public IActionResult DeleteComment(Guid id)
    {
        var comment = _commentRepo.GetById(id);
        
        if (comment == null)
        {
            return NotFound($"Comentário com ID {id} não encontrado");
        }

        _commentRepo.Delete(id);

        return NoContent();
    }
}

// DTOs para as requisições

/// <summary>
/// DTO para criação de novo comentário
/// </summary>
public record class NewCommentDTO(string Text, Guid VideoId, Guid AuthorId);

/// <summary>
/// DTO para edição de comentário
/// </summary>
public record class EditCommentDTO(string Text);