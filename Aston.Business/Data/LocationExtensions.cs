using Aston.Entities;
using Aston.Entities.DataContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Npgsql;
using Aston.Business.Utillities;

namespace Aston.Business.Data
{
    public class LocationExtensions
    {
        AstonContext context = new AstonContext();

        public Location GetLocationByCode(string code)
        {
            var obj = context.Location.Where(p => p.Code == code).FirstOrDefault();
            return obj;
        }
        public Location GetLocationByID(int id)
        {
            var obj = context.Location.Where(p => p.ID == id).FirstOrDefault();
            return obj;
        }
        public List<Location> GetLocation()
        {
            var obj = context.Location.Where(p => p.DeletedBy == null && p.DeletedDate == null).ToList();
            return obj;
        }
        public string GetLastNumberLocation()
        {
            List<int> listNo = context.Location.ToList().Select(o => Convert.ToInt32(o.No)).ToList();

            int lastNumber = listNo.Count > 0 ? listNo.Max() : 0;
            return Convert.ToString(lastNumber+1);

        }

        public List<LocationViewModel> SearchLocation_SP(int? LocationTypeCD, string Floor, int Skip)
        {
          
            var result = new List<LocationViewModel>();
            
            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_searchlocation";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    if(LocationTypeCD == null)
                    {
                        command.Parameters.AddWithValue("@locationtypecd", NpgsqlTypes.NpgsqlDbType.Integer, DBNull.Value);
                    }else
                    {
                        command.Parameters.AddWithValue("@locationtypecd", NpgsqlTypes.NpgsqlDbType.Integer, LocationTypeCD);
                    }

                    if(Floor == null)
                    {
                        command.Parameters.AddWithValue("@floor", NpgsqlTypes.NpgsqlDbType.Varchar, DBNull.Value);
                    }else
                    {
                        command.Parameters.AddWithValue("@floor", NpgsqlTypes.NpgsqlDbType.Varchar, Floor);
                    }
                        
                    command.Parameters.AddWithValue("@skip", NpgsqlTypes.NpgsqlDbType.Integer, Skip);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new LocationViewModel()
                            {
                                Location = new LocationSearchResult()
                                {
                                    ID = Convert.ToInt32(reader[0]),
                                    Code = reader[1].ToString(),
                                    Description = reader[2].ToString(),
                                    No = reader[3].ToString(),
                                    Name = reader[4].ToString(),
                                    Floor = reader[5].ToString(),
                                    LocationTypeCD = Convert.ToInt32(reader[6]),
                                    StatusCD = Convert.ToInt32(reader[7]),
                                    LocationTypeCDName = reader[8].ToString(),
                                    StatusCDName = reader[9].ToString(),
                                    TotalRow = Convert.ToInt32(reader[10])
                                }
                            });
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }
        

    }
}
