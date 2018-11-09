using System.Data.Linq;

namespace ClientsDbExplorer.Interfaces
{
    public interface IDb
    {
        DataContext Context();
    }
}