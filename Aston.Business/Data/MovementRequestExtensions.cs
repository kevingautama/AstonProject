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
    public class MovementRequestExtensions
    {
        AstonContext _context = new AstonContext();

        public MovementRequest GetMovementRequestByID(int id)
        {
            return _context.MovementRequest.Include(p => p.MovementRequestDetail).Include(p=>p.Location).Where(p => p.ID == id).FirstOrDefault();
        }

        public MovementRequestDetail GetMovementRequestDetailByID(int id)
        {
            return _context.MovementRequestDetail.Where(p => p.ID == id).FirstOrDefault();
        }
        public List<MovementRequest> GetMovementRequest()
        {
            return _context.MovementRequest.Include(p => p.MovementRequestDetail).Include(p=>p.Location).Where(p => p.DeletedDate == null && p.DeletedBy == null).ToList();
        }
        public List<MovementRequest> GetMovementRequestNeedApproval()
        {
            return _context.MovementRequest.Include(p => p.MovementRequestDetail).Include(p=>p.Location).Where(p => p.DeletedDate == null && p.DeletedBy == null && p.ApprovalStatus == 2).ToList();
        }

        public List<MovementRequest> GetMovementRequestToMove()
        {
            var obj = _context.MovementRequest.Include(p => p.MovementRequestDetail).Include(p => p.Location).Where(p => p.DeletedDate == null && p.DeletedBy == null && p.ApprovalStatus == 1).ToList();
            return obj;
        }
        public List<MovementRequestDetail> GetMovementRequestToMoveByDepartment(int depatmentid)
        {
            var mv = _context.MovementRequestDetail.Where(p => p.DeletedDate == null && p.RequestedTo == depatmentid).ToList();               
            return mv;
        }
        public List<MovementRequestViewModel> SearchMovementRequests_SP(int LocationID, int ApprovalStatus, int Skip)
        {
            var result = new List<MovementRequestViewModel>();
            var obj = new MovementRequestViewModel();

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_searchmovementrequest";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    if(LocationID == null)
                    {
                        command.Parameters.AddWithValue("@p_locationid", NpgsqlTypes.NpgsqlDbType.Integer, DBNull.Value);
                    }else
                    {
                        command.Parameters.AddWithValue("@p_locationid", NpgsqlTypes.NpgsqlDbType.Integer, LocationID);
                    }
                    if(ApprovalStatus == null)
                    {
                        command.Parameters.AddWithValue("@p_approvalstatus", NpgsqlTypes.NpgsqlDbType.Integer, DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@p_approvalstatus", NpgsqlTypes.NpgsqlDbType.Integer, ApprovalStatus);
                    }
    
                    command.Parameters.AddWithValue("@skip", NpgsqlTypes.NpgsqlDbType.Integer, Skip);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new MovementRequestViewModel {
                                MovementRequest = new MovementRequestSearchResult
                                {
                                    ID = Convert.ToInt32(reader[0]),
                                    MovementDate = reader[1].ToString(),
                                    Description = reader[2].ToString(),
                                    ApprovedDate = reader[3].ToString(),
                                    ApprovedBy=reader[4].ToString(),
                                    ApprovalStatus= Convert.ToInt32(reader[5]),
                                    Notes = reader[6].ToString(),
                                    LocationID = Convert.ToInt32(reader[7]),
                                    LocationName = reader[8].ToString(),
                                    ApprovalStatusName = reader[9].ToString(),
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
        public int NumberofMovementRequest()
        {
            return _context.MovementRequest.Include(p => p.MovementRequestDetail).Include(p => p.Location).Where(p => p.DeletedDate == null && p.DeletedBy == null).ToList().Count;
        }

        public List<HistoryViewModel> AssetHistory_SP(int AssetID, int Skip)
        {
            var result = new List<HistoryViewModel>();

            

            using (NpgsqlConnection connection =
            new NpgsqlConnection(ConfigureSetting.GetConnectionString))
            {
                connection.Open();

                string sql = "sp_assethistory_pagination";


                using (NpgsqlCommand command =
                   new NpgsqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@skip", NpgsqlTypes.NpgsqlDbType.Integer, Skip);
                    command.Parameters.AddWithValue("@assetid", NpgsqlTypes.NpgsqlDbType.Integer, AssetID);

                    using (var reader = command.ExecuteReader())
                    {
                        result = DataReaderMap.DataReaderMapToList<HistoryViewModel>(reader);
                    }
                }
                connection.Close();
            }

            return result;
        }
    }
}
