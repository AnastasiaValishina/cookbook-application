using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Cookbook.Api.Data
{
	public class DataContextDapper
	{
		private readonly IConfiguration _config;

		private const string CONNECTION_STRING = "DefaultConnection";
		public DataContextDapper(IConfiguration config)
		{
			_config = config;
		}

		public IEnumerable<T> LoadData<T>(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return dbConnection.Query<T>(sql);
		}

		public T LoadDataSingle<T>(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return dbConnection.QuerySingle<T>(sql);
		}

		public bool ExecuteSql(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return dbConnection.Execute(sql) > 0;
		}

		public int ExecuteSqlWithRowCount(string sql)
		{
			IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return dbConnection.Execute(sql);
		}

		public async Task<IEnumerable<T>> LoadDataAsync<T>(string sql)
		{
			using (IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING)))
			{
				return await dbConnection.QueryAsync<T>(sql);
			}
		}

		public async Task<T> LoadDataSingleAsync<T>(string sql)
		{
			using (IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING)))
			{
				return await dbConnection.QuerySingleAsync<T>(sql);
			}
		}

		public async Task<bool> ExecuteSqlAsync(string sql)
		{
			using (IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING)))
			{
				return await dbConnection.ExecuteAsync(sql) > 0;
			}
		}
	}
}
