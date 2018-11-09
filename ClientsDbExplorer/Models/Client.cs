using System;
using System.Data.Linq.Mapping;
using ReactiveUI;

namespace ClientsDbExplorer.Models
{
    [Table(Name = "Clients")]
    public class Client : ReactiveObject
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column] public string Name { get; set; }
        [Column] public DateTime Birthday { get; set; }
        [Column] public string Phone { get; set; }
    }
}