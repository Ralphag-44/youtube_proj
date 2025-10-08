using YouTube.Models;

namespace YouTube.Repositories;

/// <summary>
/// Contrato (abstração) para repositório de contas
/// Define as operações possíveis sem especificar a implementação
/// </summary>
public interface IAccountRepository
{
    // Salvar uma nova conta
    void Save(Account account);

    // Buscar todas as contas
    IEnumerable<Account> GetAll();

    // Buscar conta por ID
    Account? GetById(Guid id);

    // Buscar conta por username
    Account? GetByUsername(string username);

    // Atualizar conta existente
    void Update(Account account);

    // Deletar conta
    void Delete(Guid id);
}