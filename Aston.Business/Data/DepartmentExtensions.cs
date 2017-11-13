using Aston.Business.Utillities;
using Aston.Entities;
using Aston.Entities.DataContext;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Aston.Business.Data
{
    public class DepartmentExtensions
    {
        AstonContext context = new AstonContext();

        public List<Department> GetActiveDepartment()
        {
            var obj = context.Department.Where(p => p.IsActive == true).ToList();
            return obj;
        }

        public Department GetDepartmentByID(int id)
        {
            var obj = context.Department.Where(p => p.ID == id).FirstOrDefault();
            return obj;
        }

        public List<DepartmentViewModel> GetDepartment_Pagination(int Skip)
        {
            var result = new List<DepartmentViewModel>();
            var obj = new DepartmentViewModel();

            using (NpgsqlConnection connection =
           new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_departmentpagination";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@skip", NpgsqlTypes.NpgsqlDbType.Integer, Skip);

                    using (var reader = command.ExecuteReader())
                    {
                        result = DataReaderMap.DataReaderMapToList<DepartmentViewModel>(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }
    }
}
