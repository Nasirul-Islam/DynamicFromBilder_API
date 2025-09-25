using System.Data;
using System.Data.SqlClient;

namespace DynamicFromBilderAPI.Data
{
    public class MasterSpRepository
    {
        private readonly string _connectionString;

        public MasterSpRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MSSQLConnStr");
        }

        public DataSet ExecuteMasterSP(string procName = "", string procId = "", string djson1 = "", string desc01 = "", string desc02 = "", string desc03 = "", string desc04 = "", string desc05 = "")
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // here is all SP parameters 
                cmd.Parameters.AddWithValue("@ProcID", procId ?? "");
                cmd.Parameters.AddWithValue("@Djson1", (object?)djson1 ?? ""); 
                cmd.Parameters.AddWithValue("@Desc01", (object?)desc01 ?? "");
                cmd.Parameters.AddWithValue("@Desc02", (object?)desc02 ?? "");
                cmd.Parameters.AddWithValue("@Desc03", (object?)desc03 ?? "");
                cmd.Parameters.AddWithValue("@Desc04", (object?)desc04 ?? "");
                cmd.Parameters.AddWithValue("@Desc05", (object?)desc05 ?? "");

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }
            }

            return ds;
        }
    }
}

