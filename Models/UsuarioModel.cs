
using System.ComponentModel.DataAnnotations;

public class UsuarioModel
{
    [Key]
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}