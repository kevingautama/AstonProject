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
    public class AssetExtensions
    {
        AstonContext context = new AstonContext();
        public Asset GetAssetInfoByID(int id)
        {
            var obj = context.Asset.Where(p => p.ID == id).FirstOrDefault();
            return obj;
        }
        public Asset GetAssetInfoByCode (string code)
        {
            var obj = context.Asset.Where(p => p.Code == code).FirstOrDefault();
            return obj;
        }
        public List<Asset> GetAsset()
        {
            var obj = context.Asset.Where(p => p.DeletedBy == null && p.DeletedDate == null).ToList();
            return obj;
        }
        public string GetLastNumberAsset()
        {
            List<int> listNo = context.Asset.ToList().Select(o => Convert.ToInt32(o.No)).ToList();

            int lastNumber = listNo.Count > 0 ? listNo.Max() : 0;
            return Convert.ToString(lastNumber+1);

        }
    
        public List<Asset> GetAssetByCategoryCode(int code)
        {
            var obj = context.Asset.Where(p => p.CategoryCD == code && p.DeletedDate == null && p.StatusCD == 1).ToList();
            return obj;
        }

        public List<Asset> SearchAsset(int categorycode, bool? ismovable, string owner)
        {
            List<Asset> obj = new List<Asset>();
            if (categorycode != null)
            {
                obj = context.Asset.Where(p => p.CategoryCD == categorycode && p.DeletedDate == null).ToList();
            }
            if (ismovable != null)
            {
                if (obj.Count != 0)
                {
                    obj = obj.Where(p => p.IsMovable == ismovable && p.DeletedDate == null).ToList();
                }
                else
                {
                    obj = context.Asset.Where(p => p.IsMovable == ismovable && p.DeletedDate == null).ToList();
                }

            }
            if (owner != null)
            {
                if (obj.Count != 0)
                {
                    obj = obj.Where(p => p.Owner == owner && p.DeletedDate == null).ToList();
                }
                else
                {
                    obj = context.Asset.Where(p => p.Owner == owner && p.DeletedDate == null).ToList();
                }
            }



            return obj;
        }

        public List<AssetViewModel> SearchAsset_SP(int categorycode, bool? ismovable, string owner, int Skip)
        {
            var result = new List<AssetViewModel>();
            var obj = new AssetViewModel();

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_searchasset";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@categorycd", NpgsqlTypes.NpgsqlDbType.Integer, categorycode);
                    
                    if (ismovable == null)
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlDbType.Boolean, DBNull.Value);
                    }else
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlDbType.Boolean, ismovable);
                    }

                    if (owner == null)
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlDbType.Text, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlDbType.Text, owner);
                    }

                    command.Parameters.AddWithValue("@skip", NpgsqlDbType.Integer, Skip);
                    

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // make single level list
                            result.Add(new AssetViewModel
                            {
                                Asset = new AseetSearchResult
                                {
                                    ID = Convert.ToInt16(reader[0]),
                                    Code = reader[1].ToString(),
                                    Description = reader[2].ToString(),
                                    No = reader[3].ToString(),
                                    Name = reader[4].ToString(),
                                    IsMovable = Convert.ToBoolean(reader[5]),
                                    Owner=reader[6].ToString(),
                                    PurchaseDate = reader[7].ToString(),
                                    PurchasePrice = Convert.ToDecimal(reader[8]),
                                    DepreciationDuration = Convert.ToInt16(reader[9]),
                                    DisposedDate = reader[10].ToString(),
                                    ManufactureDate = reader[11].ToString(),
                                    CategoryCD = Convert.ToInt16(reader[12]),
                                    StatusCD = Convert.ToInt16(reader[13]),
                                    CategoryCDName = reader[14].ToString(),
                                    StatusCDName = reader[15].ToString(),
                                    CurrentValue = Convert.ToDouble(reader[16]),
                                    TotalRow = Convert.ToInt16(reader[17])
                                }
                            });
                        }
                    }
                }
                connection.Close();
            }

            return result;
        }

        public List<AssetViewModel> ReportAsset_SP(int categorycode, bool? ismovable, string owner)
        {
            var result = new List<AssetViewModel>();
            var obj = new AssetViewModel();

            using (NpgsqlConnection connection =
           new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_reportasset";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                  
                        command.Parameters.AddWithValue("@categorycd", NpgsqlTypes.NpgsqlDbType.Integer, categorycode);
                   
                    if (ismovable == null)
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, ismovable);
                    }

                    if (owner == null)
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, owner);
                    }


                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new AssetViewModel()
                            {
                                Asset = new AseetSearchResult
                                {
                                    ID = Convert.ToInt16(reader[0]),
                                    Code = reader[1].ToString(),
                                    Description = reader[2].ToString(),
                                    No = reader[3].ToString(),
                                    Name = reader[4].ToString(),
                                    IsMovable = Convert.ToBoolean(reader[5]),
                                    Owner = reader[6].ToString(),
                                    PurchaseDate = reader[7].ToString(),
                                    PurchasePrice = Convert.ToDecimal(reader[8]),
                                    DepreciationDuration = Convert.ToInt16(reader[9]),
                                    DisposedDate = reader[10].ToString(),
                                    ManufactureDate = reader[11].ToString(),
                                    CategoryCD = Convert.ToInt16(reader[12]),
                                    StatusCD = Convert.ToInt16(reader[13]),
                                    CategoryCDName = reader[14].ToString(),
                                    StatusCDName = reader[15].ToString(),
                                    CurrentValue = Convert.ToDouble(reader[16]),
                                    TotalRow = Convert.ToInt16(reader[17])
                                }
                            });
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }

        public List<AssetViewModel> ReportAssetZeroValue_SP(int categorycode, bool? ismovable, string owner)
        {
            var result = new List<AssetViewModel>();
            var obj = new AssetViewModel();

            using (NpgsqlConnection connection =
           new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_reportasset_zerovalue";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                   
                        command.Parameters.AddWithValue("@categorycd", NpgsqlTypes.NpgsqlDbType.Integer, categorycode);
                
                    
                    if (ismovable == null)
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, ismovable);
                    }

                    if (owner == null)
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, owner);
                    }


                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new AssetViewModel()
                            {
                                Asset = new AseetSearchResult
                                {
                                    ID = Convert.ToInt16(reader[0]),
                                    Code = reader[1].ToString(),
                                    Description = reader[2].ToString(),
                                    No = reader[3].ToString(),
                                    Name = reader[4].ToString(),
                                    IsMovable = Convert.ToBoolean(reader[5]),
                                    Owner = reader[6].ToString(),
                                    PurchaseDate = reader[7].ToString(),
                                    PurchasePrice = Convert.ToDecimal(reader[8]),
                                    DepreciationDuration = Convert.ToInt16(reader[9]),
                                    DisposedDate = reader[10].ToString(),
                                    ManufactureDate = reader[11].ToString(),
                                    CategoryCD = Convert.ToInt16(reader[12]),
                                    StatusCD = Convert.ToInt16(reader[13]),
                                    CategoryCDName = reader[14].ToString(),
                                    StatusCDName = reader[15].ToString(),
                                    CurrentValue = Convert.ToDouble(reader[16]),
                                    TotalRow = Convert.ToInt16(reader[17])
                                }
                            });
                        }
                    }
                }
                connection.Close();
            }


            return result;
        }

        public List<MismatchReportViewModel> MismatchReport_SP(int categorycode, bool? ismovable, string owner)
        {
            var result = new List<MismatchReportViewModel>();

          

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_mismatchreport";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@categorycd", NpgsqlTypes.NpgsqlDbType.Integer, categorycode);
                    if (ismovable == null)
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, DBNull.Value);
                    }else
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, ismovable);
                    }

                    if(owner == null)
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, owner);
                    }
                    

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = DataReaderMap.DataReaderMapToList<MismatchReportViewModel>(reader);
                        }
                    }
                }
                connection.Close();
            }

            return result;
        }

        public List<MismatchReportViewModel> LostAssetReport_SP(int categorycode, bool? ismovable, string owner)
        {
            var result = new List<MismatchReportViewModel>();

            using (NpgsqlConnection connection =
           new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_lostassetreport";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@categorycd", NpgsqlTypes.NpgsqlDbType.Integer, categorycode);
                    if (ismovable == null)
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ismovable", NpgsqlTypes.NpgsqlDbType.Boolean, ismovable);
                    }

                    if (owner == null)
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@owner", NpgsqlTypes.NpgsqlDbType.Varchar, owner);
                    }


                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = DataReaderMap.DataReaderMapToList<MismatchReportViewModel>(reader);
                        }
                    }
                }
                connection.Close();
            }



            return result;
        }
    }
}
