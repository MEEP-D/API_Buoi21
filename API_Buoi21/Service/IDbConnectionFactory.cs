using System.Data;

namespace API_Buoi21.Service
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
