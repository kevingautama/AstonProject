using Aston.Entities;
using Aston.Entities.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Npgsql;
using Aston.Business.Utillities;

namespace Aston.Business.Data
{
    public class RoleExtensions
    {
        AstonContext context = new AstonContext();

        public List<RolePaginationViewModel> GetRole_Pagination(int Skip)
        {
            var result = new List<RolePaginationViewModel>();
            var obj = new RolePaginationViewModel();

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_rolepagination";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@skip", NpgsqlTypes.NpgsqlDbType.Integer, Skip);

                    using (var reader = command.ExecuteReader())
                    {
                        result = DataReaderMap.DataReaderMapToList<RolePaginationViewModel>(reader);
                    }
                }
                connection.Close();
            }
            return result;
        }
    }
}
