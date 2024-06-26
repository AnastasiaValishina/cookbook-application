﻿using Dapper;
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

		public async Task<bool> ExecuteSqlWithParametersAsync(string sql, DynamicParameters parameters)
		{
			using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return await dbConnection.ExecuteAsync(sql, parameters) > 0;
		}

		public async Task<IEnumerable<T>> LoadDataAsync<T>(string sql)
		{
			using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return await dbConnection.QueryAsync<T>(sql);
		}

		public async Task<int> ExecuteScalarWithParamsAsync(string sql, DynamicParameters parameters)
		{
			using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return await dbConnection.ExecuteScalarAsync<int>(sql, parameters);
		}

		public async Task<IEnumerable<T>> LoadDataWithParamsAsync<T>(string sql, DynamicParameters parameters)
		{
			using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return await dbConnection.QueryAsync<T>(sql, parameters);
		}

		public async Task<T> LoadDataSingleWithParamsAsync<T>(string sql, DynamicParameters parameters)
		{
			using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(CONNECTION_STRING));
			return await dbConnection.QuerySingleAsync<T>(sql, parameters);
		}
	}
}
