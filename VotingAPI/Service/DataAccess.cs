using Microsoft.Data.SqlClient;
using VotingAPI.Model;

namespace VotingAPI.Service
{
    public class DataAccess
    {
        private SqlCommand command;
        private readonly string _connectionString;

        public DataAccess(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public SqlCommand CreateCommand(string procedure_name)
        {
            SqlConnection sqlConnection = new SqlConnection(_connectionString);
            command = sqlConnection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = procedure_name;
            return command;
        }

        /// <summary>
        /// Run a procedure to insert, update or delete row in a table
        /// </summary>
        /// <returns></returns>
        public async Task<DbResponse> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            DbResponse dbResponse = new DbResponse();
            try
            {
                await command.Connection.OpenAsync();
                SqlDataReader reader =  await command.ExecuteReaderAsync(cancellationToken);
              
                while (reader.Read())
                {
                    dbResponse.Status = (int)reader["status"];
                    dbResponse.Message = (string)reader["error_msg"];
                    dbResponse.SystemError = (string)reader["sys_error"];
                    dbResponse.ErrorSeverity = (int)reader["error_severity"];                   
                }
                reader.Close();
               
            }
            catch (Exception ex)
            {
                dbResponse.Status = 100;
                dbResponse.Message = "Something went wrong. Please check log for more details.";
                dbResponse.SystemError = ex.Message;
                dbResponse.ErrorSeverity = 1;
            }

            finally
            {
                await command.Connection.CloseAsync();
            }
            return dbResponse;
        }
    }
}
