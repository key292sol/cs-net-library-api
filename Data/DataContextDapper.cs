using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

public class DataContextDapper
{
	private readonly IDbConnection dbConn;

	public DataContextDapper(IConfiguration config)
	{
		dbConn = new SqlConnection(config.GetConnectionString("Default"));
	}

	public IEnumerable<T> LoadData<T>(string sql, DynamicParameters parameters)
	{
		return dbConn.Query<T>(sql, parameters);
	}

	public IEnumerable<T> LoadData<T>(string sql)
	{
		return dbConn.Query<T>(sql);
	}

	public T LoadSingle<T>(string sql, DynamicParameters parameters)
	{
		return dbConn.QuerySingle<T>(sql, parameters);
	}

	public T LoadSingle<T>(string sql)
	{
		return dbConn.QuerySingle<T>(sql);
	}

	public int ExecuteQuery(string sql)
	{
		return dbConn.Execute(sql);
	}

	public int ExecuteQuery(string sql, DynamicParameters parameters)
	{
		return dbConn.Execute(sql, parameters);
	}

	~DataContextDapper()
	{
		dbConn.Close();
	}
}