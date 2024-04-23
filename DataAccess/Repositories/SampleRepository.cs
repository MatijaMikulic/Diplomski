using Dapper;
using DataAccess.Configurations;
using DataAccess.Entity;
using DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    internal class SampleRepository : ISampleRepository
    {
        private readonly DBConfiguration _configuration;

        public SampleRepository(IOptions<DBConfiguration> options)
        {
            _configuration = options.Value;
        }
        public async Task<int> AddAsync(Sample entity)
        {
            var sql = "Insert into Samples (Name,Description) VALUES (@Name,@Description)";
            using (var connection = new SqlConnection(_configuration.DefaultConnection))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

        public async Task<IReadOnlyList<Sample>> GetAllAsync()
        {
            var sqlQuery = "SELECT * FROM Samples";
            using (var connection = new SqlConnection(_configuration.DefaultConnection))
            {
                connection.Open();
                var result = await connection.QueryAsync<Sample>(sqlQuery);
                return result.ToList();
            }
        }

        public async Task<Sample> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Samples WHERE Id = @Id";
            using (var connection = new SqlConnection(_configuration.DefaultConnection))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Sample>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> RemoveAsync(Sample entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            int rowsAffected;
            using (var connection = new SqlConnection(_configuration.DefaultConnection))
            {
                string sql = "DELETE FROM Samples WHERE Id = @Id";
                rowsAffected = await connection.ExecuteAsync(sql, new { Id = entity.Id });
            }

            return rowsAffected;
        }

        public async Task<int> RemoveAtAsync(int id)
        {
            var sql = "DELETE FROM Samples WHERE Id = @Id";
            using (var connection = new SqlConnection(_configuration.DefaultConnection))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(Sample entity)
        {
            var sql = "UPDATE Products SET Name = @Name, Description = @Description WHERE Id = @Id";
            using (var connection = new SqlConnection(_configuration.DefaultConnection))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
    }
}
