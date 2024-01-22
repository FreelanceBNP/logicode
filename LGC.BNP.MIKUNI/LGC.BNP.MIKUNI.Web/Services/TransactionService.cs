using Dapper;
using LGC.BNP.MIKUNI.Web.Models;
using System.Diagnostics;
using System.Reflection;

namespace LGC.BNP.MIKUNI.Web.Services
{
    public class TransactionService
    {
        IDatabaseConnectionFactory _db;
        public TransactionService(IDatabaseConnectionFactory _database)
        {
            _db = _database;
        }
        public async Task<List<BeginningData>> GetAllBeginningList()
        {
            var result = new List<BeginningData>();
            try
            {
                using var conn = await _db.CreateConnectionAsync();
                result = conn.Query<BeginningData>(@"SELECT        tran_Beginning.id, tran_Beginning.tran_code, tran_Beginning.remark, tran_Beginning.itemtube_id, tran_Beginning.template_id, tran_Beginning.created_by, tran_Beginning.created_date, tran_Beginning.modified_by, 
                         tran_Beginning.modified_date, tran_Beginning.is_active, tran_Beginning.is_deleted, tran_Beginning.doc_status, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, mas_Template.template_code, 
                         mas_Template.template_name, mas_Template.template_description
FROM            tran_Beginning INNER JOIN
                         mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id INNER JOIN
                         mas_Template ON tran_Beginning.template_id = mas_Template.id
WHERE        (tran_Beginning.is_deleted = 0)
ORDER BY tran_Beginning.id").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
		public async Task<List<BeginningData>> SearchBeginningListForReceiving(string code)
		{
			var result = new List<BeginningData>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<BeginningData>(@"SELECT        tran_Beginning.id, tran_Beginning.tran_code, tran_Beginning.remark, tran_Beginning.itemtube_id, tran_Beginning.template_id, tran_Beginning.created_by, tran_Beginning.created_date, tran_Beginning.modified_by, 
                         tran_Beginning.modified_date, tran_Beginning.is_active, tran_Beginning.is_deleted, tran_Beginning.doc_status, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, mas_Template.template_code, 
                         mas_Template.template_name, mas_Template.template_description, tran_Beginning.is_entry, tran_Beginning.is_print
FROM            tran_Beginning INNER JOIN
                         mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id INNER JOIN
                         mas_Template ON tran_Beginning.template_id = mas_Template.id
WHERE        (tran_Beginning.is_deleted = 0)  AND (tran_Beginning.is_active = 1) AND (tran_Beginning.is_entry = 1) AND 
( (mas_ItemTube.itemtube_code = @code) OR (tran_Beginning.tran_code = @code) )
ORDER BY tran_Beginning.id", new { code }).ToList();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<BeginningData> GetBeginningById(long id)
		{
			var result = new BeginningData();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<BeginningData>(@"SELECT * FROM tran_Beginning WHERE (id = @id)", new { id }).FirstOrDefault();
                result.details = conn.Query<BeginningDetailData>(@"SELECT * FROM tran_BeginningDetail WHERE (beginning_id = @id)", new { id }).ToList();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> UpsertBeginning(BeginningData model)
		{
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var trans = conn.BeginTransaction();
				string query = "";
				if (model.id.HasValue)
				{
					query = @"UPDATE [dbo].[tran_Beginning]
   SET 
      [remark] = @remark
      ,[itemtube_id] = @itemtube_id
      ,[template_id] = @template_id
      ,[modified_by] = @modified_by
      ,[modified_date] = @modified_date
      ,[is_active] = @is_active
      ,[is_deleted] = @is_deleted,doc_status = @doc_status,is_entry = @is_entry, is_print = @is_print 
 WHERE (id=@id)";
				}
				else
				{
					query = @"INSERT INTO [dbo].[tran_Beginning]
           ([tran_code]
           ,[remark]
           ,[itemtube_id]
           ,[template_id]
           ,[created_by]
           ,[created_date]
           ,[modified_by]
           ,[modified_date]
           ,[is_active]
           ,[is_deleted],doc_status,is_entry,is_print)
     VALUES
           (@tran_code
           ,@remark
           ,@itemtube_id
           ,@template_id
           ,@created_by
           ,@created_date
           ,@modified_by
           ,@modified_date
           ,@is_active
           ,@is_deleted,@doc_status,@is_entry,@is_print)";
				}
				await conn.ExecuteAsync(query, model, transaction: trans);

				//details
				if (model.id.HasValue)
				{
					//detail first
					query = @"DELETE FROM [dbo].[tran_BeginningDetail] WHERE (beginning_id = @id)";
					await conn.ExecuteAsync(query, model, transaction: trans);
				}

				query = @"INSERT INTO [dbo].[tran_BeginningDetail]
           ([beginning_id]
           ,[template_build_id]
			,[template_build_code]
           ,[tran_value_string]
           ,[tran_value_number]
           ,[created_by]
           ,[created_date])
     VALUES
           (@beginning_id
           ,@template_build_id
				,@template_build_code
           ,@tran_value_string
           ,@tran_value_number
           ,@created_by
           ,@created_date)";
				await conn.ExecuteAsync(query, model.details, transaction: trans);

				trans.Commit();
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> DeleteBeginningById(long id)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = @"UPDATE tran_Beginning SET is_deleted = 1 WHERE id = @id";
				await conn.ExecuteAsync(query, new { id });
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> LaunchProject(long id)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = @"UPDATE tran_Beginning SET doc_status = 'LAUNCH' WHERE id = @id";
				await conn.ExecuteAsync(query, new { id });
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.message.Add(ex.Message);
			}
			return result;
		}

		public async Task<ReturnMessageModel> UpdateStatusById(long id, ProjectStatus doc_status)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = @"UPDATE tran_Beginning SET doc_status = @doc_status ";
				switch (doc_status.ToString())
				{
					case "PRINTED":
						query += ", is_print = 1 ";
						break;
					default:
						break;
				}
				query += " WHERE id = @id";
				await conn.ExecuteAsync(query, new { id, doc_status = doc_status.ToString() });
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<List<StockOverviewData>> GetInventoryList()
		{
			var result = new List<StockOverviewData>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<StockOverviewData>(@"SELECT  id, beginning_id, doc_status, created_by, created_date, modified_by, modified_date, re_print_no
FROM      tran_StockOverview").ToList();
				foreach (var item in result)
				{
					item.project = conn.Query<BeginningData>(@"SELECT        tran_Beginning.id AS beginning_id, tran_Beginning.tran_code, tran_Beginning.remark, tran_Beginning.itemtube_id, tran_Beginning.template_id, tran_Beginning.created_by, tran_Beginning.created_date, 
                         tran_Beginning.modified_by, tran_Beginning.modified_date, tran_Beginning.is_active, tran_Beginning.is_deleted, tran_Beginning.doc_status, tran_Beginning.is_entry, tran_Beginning.is_print, mas_ItemTube.itemtube_code, 
                         mas_ItemTube.itemtube_description
FROM            tran_Beginning INNER JOIN
                         mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
WHERE        (tran_Beginning.id = @id)", new { id = item.beginning_id }).FirstOrDefault();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<StockOverviewData> GetInventoryById(long id)
		{
			var result = new StockOverviewData();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<StockOverviewData>(@"SELECT   tran_StockOverview.id, tran_StockOverview.beginning_id, tran_StockOverview.doc_status, tran_StockOverview.created_by, tran_StockOverview.created_date, tran_StockOverview.modified_by, tran_StockOverview.modified_date, tran_StockOverview.re_print_no, tran_StockOverview.stamp_receiving, tran_StockOverview.stamp_receiving_by, 
             tran_StockOverview.stamp_delivery, tran_StockOverview.stamp_delivery_by, tran_StockOverview.stamp_collection, tran_StockOverview.stamp_collection_by, tran_StockOverview.stamp_aliquot, tran_StockOverview.stamp_aliquot_by, tran_StockOverview.stamp_preparation, tran_StockOverview.stamp_preparation_by, 
             tran_StockOverview.stamp_analysis, tran_StockOverview.stamp_analysis_by, tran_StockOverview.stamp_destroy, tran_StockOverview.stamp_destroy_by, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, tran_Beginning.tran_code, tran_Beginning.remark
FROM     tran_StockOverview INNER JOIN
             tran_Beginning ON tran_StockOverview.beginning_id = tran_Beginning.id INNER JOIN
             mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id WHERE (tran_StockOverview.id = @id)", new { id }).FirstOrDefault();
				result.project = conn.Query<BeginningData>(@"	SELECT   tran_Beginning.id, tran_Beginning.tran_code, tran_Beginning.remark, tran_Beginning.itemtube_id, tran_Beginning.template_id, tran_Beginning.created_by, tran_Beginning.created_date, tran_Beginning.modified_by, tran_Beginning.modified_date, tran_Beginning.is_active, tran_Beginning.is_deleted, tran_Beginning.doc_status, 
             tran_Beginning.is_entry, tran_Beginning.is_print, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description
FROM     tran_Beginning INNER JOIN
             mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
 WHERE (tran_Beginning.id = @id)", new { id = result.beginning_id }).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<StockOverviewData> GetInventoryByCode(string code, string doc_status = "", bool use_serapate = false)
		{
			var result = new StockOverviewData();
			string query = "";
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				if (!use_serapate)
				{
                    query = @"SELECT   tran_StockOverview.id, tran_StockOverview.beginning_id, tran_StockOverview.doc_status, tran_StockOverview.created_by, tran_StockOverview.created_date, tran_StockOverview.modified_by, tran_StockOverview.modified_date, tran_StockOverview.re_print_no, tran_StockOverview.stamp_receiving, tran_StockOverview.stamp_receiving_by, 
             tran_StockOverview.stamp_delivery, tran_StockOverview.stamp_delivery_by, tran_StockOverview.stamp_collection, tran_StockOverview.stamp_collection_by, tran_StockOverview.stamp_aliquot, tran_StockOverview.stamp_aliquot_by, tran_StockOverview.stamp_preparation, tran_StockOverview.stamp_preparation_by, 
             tran_StockOverview.stamp_analysis, tran_StockOverview.stamp_analysis_by, tran_StockOverview.stamp_destroy, tran_StockOverview.stamp_destroy_by, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, tran_Beginning.tran_code, tran_Beginning.remark
FROM     tran_StockOverview INNER JOIN
             tran_Beginning ON tran_StockOverview.beginning_id = tran_Beginning.id INNER JOIN
             mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
WHERE   (tran_Beginning.is_deleted = 0) AND (tran_Beginning.is_active = 1) AND (mas_ItemTube.itemtube_code = @code) AND (tran_StockOverview.doc_status = @doc_status) AND (tran_StockOverview.is_separate = @use_serapate) OR
             (tran_Beginning.tran_code = @code) AND (tran_Beginning.is_deleted = 0) AND (tran_Beginning.is_active = 1) AND (tran_StockOverview.doc_status = @doc_status)  AND (tran_StockOverview.is_separate = @use_serapate)";
				}
				else
				{
                    switch (doc_status)
                    {
                        case "SAMPLE_PREPARATION":
                            query = @"SELECT   tran_StockOverview.id, tran_StockOverview.beginning_id, tran_StockOverview.doc_status, tran_StockOverview.created_by, tran_StockOverview.created_date, tran_StockOverview.modified_by, tran_StockOverview.modified_date, tran_StockOverview.re_print_no, tran_StockOverview.stamp_receiving, tran_StockOverview.stamp_receiving_by, 
             tran_StockOverview.stamp_delivery, tran_StockOverview.stamp_delivery_by, tran_StockOverview.stamp_collection, tran_StockOverview.stamp_collection_by, tran_StockOverview.stamp_aliquot, tran_StockOverview.stamp_aliquot_by, tran_StockOverview.stamp_preparation, tran_StockOverview.stamp_preparation_by, 
             tran_StockOverview.stamp_analysis, tran_StockOverview.stamp_analysis_by, tran_StockOverview.stamp_destroy, tran_StockOverview.stamp_destroy_by, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, tran_Beginning.tran_code, tran_Beginning.remark
FROM     tran_StockOverview INNER JOIN
             tran_Beginning ON tran_StockOverview.beginning_id = tran_Beginning.id INNER JOIN
             mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
WHERE   (tran_Beginning.is_deleted = 0) AND (tran_Beginning.is_active = 1) AND (mas_ItemTube.itemtube_code = @code) AND (tran_StockOverview.doc_status = @doc_status) AND (tran_StockOverview.is_separate = @use_serapate) OR
             (tran_Beginning.tran_code = @code) AND (tran_Beginning.is_deleted = 0) AND (tran_Beginning.is_active = 1) AND (tran_StockOverview.doc_status = @doc_status)  AND (tran_StockOverview.is_separate = @use_serapate)  AND  (tran_StockOverview.is_parent = 0) AND (tran_StockOverview.parent_code IS NOT NULL)";
                            break;
                        default:
                            query = @"SELECT        tran_StockOverview.id, tran_StockOverview.beginning_id, tran_StockOverview.doc_status, tran_StockOverview.created_by, tran_StockOverview.created_date, tran_StockOverview.modified_by, 
                         tran_StockOverview.modified_date, tran_StockOverview.re_print_no, tran_StockOverview.stamp_receiving, tran_StockOverview.stamp_receiving_by, tran_StockOverview.stamp_delivery, 
                         tran_StockOverview.stamp_delivery_by, tran_StockOverview.stamp_collection, tran_StockOverview.stamp_collection_by, tran_StockOverview.stamp_aliquot, tran_StockOverview.stamp_aliquot_by, 
                         tran_StockOverview.stamp_preparation, tran_StockOverview.stamp_preparation_by, tran_StockOverview.stamp_analysis, tran_StockOverview.stamp_analysis_by, tran_StockOverview.stamp_destroy, 
                         tran_StockOverview.stamp_destroy_by, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, tran_Beginning.tran_code, tran_Beginning.remark, tran_StockOverview.separate_tube_code
FROM            tran_StockOverview INNER JOIN
                         tran_Beginning ON tran_StockOverview.beginning_id = tran_Beginning.id INNER JOIN
                         mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
WHERE        (tran_Beginning.is_deleted = 0) AND (tran_Beginning.is_active = 1) AND (tran_StockOverview.doc_status = @doc_status) AND (tran_StockOverview.is_separate = @use_serapate) AND 
                         (tran_StockOverview.separate_tube_code = @code)";
                            break;
                    }
                   
				}
				result = conn.Query<StockOverviewData>(query, new { code, doc_status, use_serapate }).FirstOrDefault();
				if (result != null)
				{
					result.project = conn.Query<BeginningData>(@"SELECT   tran_Beginning.id, tran_Beginning.tran_code, tran_Beginning.remark, tran_Beginning.itemtube_id, tran_Beginning.template_id, tran_Beginning.created_by, tran_Beginning.created_date, tran_Beginning.modified_by, tran_Beginning.modified_date, tran_Beginning.is_active, tran_Beginning.is_deleted, tran_Beginning.doc_status, 
             tran_Beginning.is_entry, tran_Beginning.is_print, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description
FROM     tran_Beginning INNER JOIN
             mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
 WHERE (tran_Beginning.id = @id)", new { id = result.beginning_id }).FirstOrDefault();
				}
				else
				{
					result = new StockOverviewData();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<StockOverviewData> GetInventoryByIdWithDetail(long id)
		{
			var result = new StockOverviewData();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<StockOverviewData>(@"SELECT        tran_StockOverview.id, tran_StockOverview.beginning_id, tran_StockOverview.doc_status, tran_StockOverview.created_by, tran_StockOverview.created_date, tran_StockOverview.modified_by, 
                         tran_StockOverview.modified_date, tran_StockOverview.re_print_no, tran_StockOverview.stamp_receiving, tran_StockOverview.stamp_receiving_by, tran_StockOverview.stamp_delivery, 
                         tran_StockOverview.stamp_delivery_by, tran_StockOverview.stamp_collection, tran_StockOverview.stamp_collection_by, tran_StockOverview.stamp_aliquot, tran_StockOverview.stamp_aliquot_by, 
                         tran_StockOverview.stamp_preparation, tran_StockOverview.stamp_preparation_by, tran_StockOverview.stamp_analysis, tran_StockOverview.stamp_analysis_by, tran_StockOverview.stamp_destroy, 
                         tran_StockOverview.stamp_destroy_by, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, tran_Beginning.tran_code, tran_Beginning.remark, tran_StockOverview.separate_tube_code, 
                         tran_StockOverview.is_parent, tran_StockOverview.is_separate
FROM            tran_StockOverview INNER JOIN
                         tran_Beginning ON tran_StockOverview.beginning_id = tran_Beginning.id INNER JOIN
                         mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
WHERE   (tran_Beginning.is_deleted = 0) AND (tran_Beginning.is_active = 1) AND (tran_StockOverview.id = @id)  ", new { id }).FirstOrDefault();
				if (result != null)
				{
					result.project = conn.Query<BeginningData>(@"SELECT   tran_Beginning.id, tran_Beginning.tran_code, tran_Beginning.remark, tran_Beginning.itemtube_id, tran_Beginning.template_id, tran_Beginning.created_by, tran_Beginning.created_date, tran_Beginning.modified_by, tran_Beginning.modified_date, tran_Beginning.is_active, tran_Beginning.is_deleted, tran_Beginning.doc_status, 
             tran_Beginning.is_entry, tran_Beginning.is_print, mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description
FROM     tran_Beginning INNER JOIN
             mas_ItemTube ON tran_Beginning.itemtube_id = mas_ItemTube.id
 WHERE (tran_Beginning.id = @id)", new { id = result.beginning_id }).FirstOrDefault();
				}
				else
				{
					result = new StockOverviewData();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> UpdateStockStatusById(long id, StockStatus doc_status, string marker, BeginningData model = null, string remark = "")
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var trans = conn.BeginTransaction();
				string query = string.Empty;

				switch (doc_status.ToString())
				{
					case "STOCK":
						query = @"UPDATE tran_Beginning SET doc_status = @doc_status  WHERE id = @beginning_id";
						await conn.ExecuteAsync(query, new { beginning_id = model.id, doc_status = ProjectStatus.RELEASED.ToString() }, transaction: trans);

						query = @"INSERT INTO [dbo].[tran_StockOverview]
           ([beginning_id]
           ,[doc_status]
           ,[created_by]
           ,[created_date]
           ,[modified_by]
           ,[modified_date]
           ,[re_print_no]
           ,[stamp_receiving]
           ,[stamp_receiving_by], receiving_remark)
     VALUES
           (@beginning_id
           ,@doc_status
           ,@created_by
           ,@created_date
           ,@created_by
           ,@created_date
           ,@re_print_no
			,@created_date
           ,@created_by,@remark
           )";
						await conn.ExecuteAsync(query, new
						{
							beginning_id = model.id,
							doc_status = doc_status.ToString(),
							created_by = marker,
							created_date = DateTime.Now,
							re_print_no = 0, remark
						}, transaction: trans);
						break;
                    case "DELIVERY":
                        query = @"UPDATE tran_StockOverview SET doc_status = @doc_status, [stamp_delivery] = @created_date, [stamp_delivery_by] = @created_by, delivery_remark = @remark  WHERE id = @id";
                        await conn.ExecuteAsync(query, new 
						{ 
							id, 
							doc_status = doc_status.ToString(),
                            created_by = marker,
                            created_date = DateTime.Now,
							remark
						}, transaction: trans);
                    
                        break;
                    case "SAMPLE_COLLECTION":
                        query = @"UPDATE tran_StockOverview SET doc_status = @doc_status, [stamp_collection] = @created_date, [stamp_collection_by] = @created_by, collection_remark = @remark  WHERE id = @id";
                        await conn.ExecuteAsync(query, new
                        {
                            id,
                            doc_status = doc_status.ToString(),
                            created_by = marker,
                            created_date = DateTime.Now, remark
                        }, transaction: trans);
                        break;
                    case "ALIQUOT":
                        query = @"UPDATE tran_StockOverview SET doc_status = @doc_status, [stamp_aliquot] = @created_date, [stamp_aliquot_by] = @created_by, aliquot_remark = @remark  WHERE id = @id";
                        await conn.ExecuteAsync(query, new
                        {
                            id,
                            doc_status = doc_status.ToString(),
                            created_by = marker,
                            created_date = DateTime.Now,
							remark
						}, transaction: trans);

                        break;
                    case "SAMPLE_PREPARATION":
                        query = @"UPDATE tran_StockOverview SET doc_status = @doc_status, [stamp_preparation] = @created_date, [stamp_preparation_by] = @created_by, preparation_remark = @remark  WHERE id = @id";
                        await conn.ExecuteAsync(query, new
                        {
                            id,
                            doc_status = doc_status.ToString(),
                            created_by = marker,
                            created_date = DateTime.Now,
							remark
						}, transaction: trans);
                        break;
                    case "ANALYSIS":
                        query = @"UPDATE tran_StockOverview SET doc_status = @doc_status, [stamp_analysis] = @created_date, [stamp_analysis_by] = @created_by, analysis_remark = @remark  WHERE id = @id";
                        await conn.ExecuteAsync(query, new
                        {
                            id,
                            doc_status = doc_status.ToString(),
                            created_by = marker,
                            created_date = DateTime.Now,
                            remark
                        }, transaction: trans);
                        break;
                    default:
						break;
				}
				trans.Commit();
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<List<StockOverviewData>> GetInventoryListByStatus(string doc_status)
        {
            var result = new List<StockOverviewData>();
            try
            {
                using var conn = await _db.CreateConnectionAsync();
				string query = "";
				switch (doc_status)
				{
					case "SAMPLE_PREPARATION":
						query = "SELECT * FROM tran_StockOverview WHERE (doc_status = @doc_status)  AND (parent_code IS NULL) AND ((is_parent = 1) OR (is_separate = 1) )";
						break;
					default:
						query = "SELECT * FROM tran_StockOverview WHERE (doc_status = @doc_status) AND (is_separate = 0)";
						break;
				}
				result = conn.Query<StockOverviewData>(query, new { doc_status }).ToList();
				foreach (var item in result)
				{
					item.project = conn.Query<BeginningData>(@"SELECT  mas_ItemTube.itemtube_code, mas_ItemTube.itemtube_description, mas_ItemTube.created_by, mas_ItemTube.created_date, mas_ItemTube.modified_by, mas_ItemTube.modified_date, tran_Beginning.id, tran_Beginning.tran_code, tran_Beginning.remark, 
             tran_Beginning.itemtube_id
FROM   mas_ItemTube INNER JOIN
             tran_Beginning ON mas_ItemTube.id = tran_Beginning.itemtube_id
WHERE (mas_ItemTube.is_active = 1) AND (mas_ItemTube.is_deleted = 0) AND (tran_Beginning.is_active = 1) AND (tran_Beginning.is_deleted = 0) AND (tran_Beginning.id = @id)", new { id = item.beginning_id }).FirstOrDefault();
					item.is_separate = GetInventorySeparateListByIdAndStatus(item.beginning_id, doc_status).Result.Count > 0;
                }
			}
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
		public async Task<List<StockOverviewData>> GetInventorySeparateListByIdAndStatus(long id, string doc_status)
		{
			var result = new List<StockOverviewData>();
			try
			{
				var _stock = GetInventoryByIdWithDetail(id).Result;
				using var conn = await _db.CreateConnectionAsync();
				string query = "";
				switch (doc_status)
				{
					case "SAMPLE_PREPARATION":
						query = "SELECT * FROM tran_StockOverview WHERE (doc_status = @doc_status) AND (parent_code = '"+ _stock.separate_tube_code +"')";
						break;
					default:
						query = "SELECT * FROM tran_StockOverview WHERE (doc_status = @doc_status) AND (is_separate = 1) AND (beginning_id = @id)";
						break;
				}
				result = conn.Query<StockOverviewData>(query, new { doc_status, id }).ToList();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SeparateCollection(long id, string marker, string location, DateTime expire)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{	//Stock Data
				var _stock = GetInventoryByIdWithDetail(id).Result;

				using var conn = await _db.CreateConnectionAsync();
			
				string query = string.Empty;

				query = @"
INSERT INTO [dbo].[tran_StockOverview]
SELECT  [beginning_id]
      ,[doc_status]
      ,@created_by
      ,@created_date
      ,@created_by
      ,@created_date
      ,[re_print_no]
      ,@separate_tube_code
      ,@location
      ,@expire
      ,[stamp_receiving]
      ,[stamp_receiving_by]
      ,[receiving_remark]
      ,[stamp_delivery]
      ,[stamp_delivery_by]
      ,[delivery_remark]
      ,[stamp_collection]
      ,[stamp_collection_by]
      ,[collection_remark]
      ,[stamp_aliquot]
      ,[stamp_aliquot_by]
      ,[aliquot_remark]
      ,[stamp_preparation]
      ,[stamp_preparation_by]
      ,[preparation_remark]
      ,[stamp_analysis]
      ,[stamp_analysis_by]
      ,[analysis_remark]
      ,[stamp_destroy]
      ,[stamp_destroy_by]
      ,[destroy_remark]
      ,1,@created_date,@created_by,0,NULL
  FROM [dbo].[tran_StockOverview] WHERE (id = @id)";
				await conn.ExecuteAsync(query, new
				{
					id,
					doc_status = StockStatus.SAMPLE_COLLECTION.ToString(),
					created_by = marker,
					created_date = DateTime.Now,
					separate_tube_code = String.Format("{0}-SC001", _stock.project.itemtube_code.Trim()),
					location,
					expire
				});
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SeparateAliquot(long id, string marker, int amount)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{	
				//Stock Data
				var _stock = GetInventoryByIdWithDetail(id).Result;
				using var conn = await _db.CreateConnectionAsync();
				string query = string.Empty;
				var trans = conn.BeginTransaction();

				query = @"
INSERT INTO [dbo].[tran_StockOverview]
SELECT  [beginning_id]
      ,[doc_status]
      ,@created_by
      ,@created_date
      ,@created_by
      ,@created_date
      ,[re_print_no]
      ,@separate_tube_code
      ,null
      ,null
      ,[stamp_receiving]
      ,[stamp_receiving_by]
      ,[receiving_remark]
      ,[stamp_delivery]
      ,[stamp_delivery_by]
      ,[delivery_remark]
      ,[stamp_collection]
      ,[stamp_collection_by]
      ,[collection_remark]
      ,[stamp_aliquot]
      ,[stamp_aliquot_by]
      ,[aliquot_remark]
      ,[stamp_preparation]
      ,[stamp_preparation_by]
      ,[preparation_remark]
      ,[stamp_analysis]
      ,[stamp_analysis_by]
      ,[analysis_remark]
      ,[stamp_destroy]
      ,[stamp_destroy_by]
      ,[destroy_remark]
      ,1,@created_date,@created_by,0,NULL
  FROM [dbo].[tran_StockOverview] WHERE (id = @id)";
				for (int i = 0; i < amount; i++)
				{
					await conn.ExecuteAsync(query, new
					{
						id,
						doc_status = StockStatus.ALIQUOT.ToString(),
						created_by = marker,
						created_date = DateTime.Now,
						separate_tube_code = String.Format("{0}-AO{1}", _stock.project.itemtube_code.Trim(), (i + 1).ToString().PadLeft(3, '0'))
					}, transaction: trans);
				}
				trans.Commit();
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SeparatePreparation(long id, string marker, int amount)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{	
				//Stock Data
				var _stock = GetInventoryByIdWithDetail(id).Result;
				using var conn = await _db.CreateConnectionAsync();
				string query = string.Empty;
				var trans = conn.BeginTransaction();

				query = @"
INSERT INTO [dbo].[tran_StockOverview]
SELECT  [beginning_id]
      ,[doc_status]
      ,@created_by
      ,@created_date
      ,@created_by
      ,@created_date
      ,[re_print_no]
      ,@separate_tube_code
      ,null
      ,null
      ,[stamp_receiving]
      ,[stamp_receiving_by]
      ,[receiving_remark]
      ,[stamp_delivery]
      ,[stamp_delivery_by]
      ,[delivery_remark]
      ,[stamp_collection]
      ,[stamp_collection_by]
      ,[collection_remark]
      ,[stamp_aliquot]
      ,[stamp_aliquot_by]
      ,[aliquot_remark]
      ,[stamp_preparation]
      ,[stamp_preparation_by]
      ,[preparation_remark]
      ,[stamp_analysis]
      ,[stamp_analysis_by]
      ,[analysis_remark]
      ,[stamp_destroy]
      ,[stamp_destroy_by]
      ,[destroy_remark]
      ,1,@created_date,@created_by,0,@parent_code
  FROM [dbo].[tran_StockOverview] WHERE (id = @id)";
				for (int i = 0; i < amount; i++)
				{
					await conn.ExecuteAsync(query, new
					{
						id,
						doc_status = StockStatus.ALIQUOT.ToString(),
						created_by = marker,
						created_date = DateTime.Now,
						separate_tube_code = String.Format("{0}-SP{1}", _stock.separate_tube_code.Trim(), (i + 1).ToString().PadLeft(3, '0')),
						parent_code = _stock.separate_tube_code.Trim()
					}, transaction: trans);
				}

				query = "UPDATE tran_StockOverview SET is_parent = 1 WHERE id = @id";
				await conn.ExecuteAsync(query, new
				{
					id
				}, transaction: trans);

				trans.Commit();
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}
	}
}
