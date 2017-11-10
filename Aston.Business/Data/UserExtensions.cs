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
using NpgsqlTypes;
using Aston.Business.Utillities;

namespace Aston.Business.Data
{
    public class UserExtensions
    {
        AstonContext context = new AstonContext();
 
        public List<UserPaginationViewModel> GetUser_Pagination(int Skip)
        {
            var result = new List<UserPaginationViewModel>();
            var obj = new UserPaginationViewModel();

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_userpagination";


            using (NpgsqlCommand command =
               new NpgsqlCommand(sql,connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@skip", NpgsqlTypes.NpgsqlDbType.Integer, Skip);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result =  DataReaderMap.DataReaderMapToList<UserPaginationViewModel>(reader);
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }
    }
}
