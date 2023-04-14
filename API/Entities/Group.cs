using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {   
        public Group()
        {
              
        }
        
        //name of the group should be unique in database by stating primary key
        public Group(string name)
        {
            Name = name;
        }

        //making it impossible to add same group name in database if exists.
        [Key] // primary key
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
        
    }
}