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
    public class AssetLocationExtensions
    {
        AstonContext context = new AstonContext();
        public AssetLocation GetAssetLocationByID(int id)
        {
            var obj = context.AssetLocation.Include(p => p.Asset).Include(p => p.Location).Where(p => p.ID == id).FirstOrDefault();
         
            return obj;
        }

        public List<AssetLocation> GetAssetLocationByLocationID(int id)
        {
            var obj = context.AssetLocation.Include(p => p.Asset).Include(p => p.Location).Where(p => p.LocationID == id && p.DeletedDate == null && p.DeletedBy == null).ToList();
            return obj;
        }

        public List<AssetLocation> GetAssetLocation()
        {
            var obj = context.AssetLocation.Include(p=>p.Asset).Include(p=>p.Location).Where(p => p.DeletedBy == null && p.DeletedDate == null).ToList();
            return obj;
        }

        public List<AssetLocation> GetAssetLocationByMovementDetailID(int id )
        {
            var obj = context.AssetLocation.Include(p=>p.Asset).Include(p=>p.Location).Where(p => p.MovementRequestDetailID == id && p.DeletedDate == null).ToList();
            return obj;
        }

        public List<AssetLocationViewModel> Pagination_AssetLocation_SP(int Skip)
        {
            var result = new List<AssetLocationViewModel>();
            var obj = new AssetLocationViewModel();

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_assetlocation_pagination";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@skip", NpgsqlTypes.NpgsqlDbType.Integer, Skip);

                    using (var reader = command.ExecuteReader())
                    {
                        //make single level list
                        while (reader.Read())
                        {
                            result.Add(new AssetLocationViewModel()
                            {
                                AssetLocation = new AssetLocationPagination
                                {
                                    ID = Convert.ToInt32(reader[0]),
                                    AssetID = Convert.ToInt32(reader[1]),
                                    LocationID = Convert.ToInt32(reader[2]),
                                    OnTransition = Convert.ToBoolean(reader[3]),
                                    MovementRequestDetailID = Convert.ToInt32(reader[4]),
                                    LocationName = reader[5].ToString(),
                                    AssetName = reader[6].ToString(),
                                    TotalRow = Convert.ToInt32(reader[7])
                                }
                            });
                        }
                    }
                }
                connection.Close();
            }

            return result;
        }

        public List<AssetOpnameTransactionViewModel> GetAssetLatestLocationByLocationID(int LocationID,DateTime Opnamedate)
        {
            List<AssetOpnameTransactionViewModel> result = new List<AssetOpnameTransactionViewModel>();

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_assetlocationlatest";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@locationid", NpgsqlTypes.NpgsqlDbType.Integer, LocationID);
                    command.Parameters.AddWithValue("@opnamedate", NpgsqlTypes.NpgsqlDbType.Timestamp, Opnamedate);

                    using (var reader = command.ExecuteReader())
                    {
                        result = DataReaderMap.DataReaderMapToList<AssetOpnameTransactionViewModel>(reader);
                    }
                }
                connection.Close();
            }
            return result;
        }

        public List<AssetOpnameTransactionViewModel> GetAssetLocationOpnameLatestByLocationID(int LocationID, DateTime Opnamedate)
        {
            List<AssetOpnameTransactionViewModel> result = new List<AssetOpnameTransactionViewModel>();

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_assetlocationopnamelatest";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@locationid", NpgsqlTypes.NpgsqlDbType.Integer, LocationID);
                    command.Parameters.AddWithValue("@opnamedate", NpgsqlTypes.NpgsqlDbType.Timestamp, Opnamedate);

                    using (var reader = command.ExecuteReader())
                    {
                        result = DataReaderMap.DataReaderMapToList<AssetOpnameTransactionViewModel>(reader);
                    }
                }
                connection.Close();
            }
            return result;
        }
    }
}
