using System;
using System.Data.Linq;
using System.Linq;
using ClientsDbExplorer.Interfaces;
using ClientsDbExplorer.Models;

namespace ClientsDbExplorer
{
    public class Db : IDb
    {
        private DataContext _db;

        public string ConnectionString { get; set; }

        public Db(string connectionString)
        {
            ConnectionString = connectionString;

            _db = new DataContext(ConnectionString);


        }

        public DataContext Context()
        {
            return _db;
        }
    }
}