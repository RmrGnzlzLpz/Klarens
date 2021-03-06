using Domain.Base;

namespace Domain.Entities
{
    public class Usuario : Entity<int>
    {
        public Persona Persona { get; set; }
        public Rol Rol { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UsuarioEstado Estado { get; set; }

        public Usuario() { }

        public Usuario(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public Usuario(Rol rol, string username, string password) : this(username, password)
        {
            Rol = rol;
        }
    }

    public enum UsuarioEstado
    {
        Inactivo = 0, Activo = 1
    }
}