namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } // User's ID, defasult constructor value is 0.
        public string UserName { get; set; } // ? is optional constructor as stirng by default requires a non-null value. 
        // Alternatively, disable null control in API.csproj file 'Nullable'
    }
}