using Dapper;
using LGC.BNP.MIKUNI.Web.Models;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Mail;
namespace LGC.BNP.MIKUNI.Web.Services
{
	public class MasterService
	{
		private IConfiguration _config;
		IDatabaseConnectionFactory _db;
		private readonly MailService _mail;
		public MasterService(IConfiguration config, IDatabaseConnectionFactory _database, MailService mail)
		{
			_config = config;
			_db = _database;
			_mail = mail;
		}
		//Item Tube
		public async Task<List<ItemTubeData>> GetItemTubeList()
		{
			var result = new List<ItemTubeData>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<ItemTubeData>(@"SELECT * FROM mas_item WHERE is_deleted = 0  ORDER BY id").ToList();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnList<MasterItem>> GetItemMaster()
		{
			var result = new ReturnList<MasterItem>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var list = conn.Query<MasterItem>($@"SELECT
														s.is_active as is_vip,
														s.is_delete as is_del_vip,
														m.*
														FROM mas_item  m
														LEFT JOIN sec_employee_config s on m.emp_name = s.name
														WHERE m.is_deleted = 0
														ORDER BY mas_item_id").ToList();
				foreach (var item in list) {
					if (item.is_vip && !item.is_del_vip) {
						item.is_allow = true;
						item.type_allow = "vip";
					}
				result.data = list;
				}
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
		public async Task<ReturnObject<MasterItem>> GetItemMasterById(long mas_item_id)
		{
			var result = new ReturnObject<MasterItem>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result.data = conn.Query<MasterItem>(@"SELECT * FROM mas_item WHERE (mas_item_id = @mas_item_id)", new { mas_item_id }).FirstOrDefault();
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
		public async Task<ReturnMessageModel> MappingTagMaster(MappingRequest model) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var getDataTag =  conn.Query<MasTagMapping>(@"SELECT * FROM mas_tag_mapping WHERE tag_id = @tag_id", new { tag_id = model.tag_id }).FirstOrDefault();
				if (getDataTag != null) {
					if (getDataTag.status == "mapping") {
						result.isCompleted = false;
						result.message.Add("Tag is mapping");
						return result;
					}
					string query = @"UPDATE mas_tag_mapping
					SET mas_item_id = @mas_item_id
					, status = 'mapping'
					, update_date = @update_date
					, update_by = @update_by
					WHERE tag_id = @tag_id";
					await conn.ExecuteAsync(query, new { mas_item_id = model.mas_item_id
					, tag_id = model.tag_id
					, update_date = DateTime.Now
					, update_by = model.update_by });
					result.isCompleted = true;
					result.message.Add("success");
				} else {
					result.isCompleted = false;
					result.message.Add("Tag not found");
				}


			} catch (Exception e) {
				result.isCompleted = false;
				result.message.Add(e.Message);
			}

			return result;
		}
		public async Task<ReturnMessageModel> CancelMappingTag(MappingRequest model) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var getDataTag =  conn.Query<MasTagMapping>(@"SELECT * FROM mas_tag_mapping WHERE tag_id = @tag_id", new { tag_id = model.tag_id }).FirstOrDefault();
				if (getDataTag != null) {
					if (getDataTag.status == "mapping") {
						string query = @"UPDATE mas_tag_mapping
						SET mas_item_id = null
						, status = 'active'
						, update_date = @update_date
						, update_by = @update_by
						WHERE tag_id = @tag_id";
						await conn.ExecuteAsync(query, new {
						tag_id = model.tag_id
						, update_date = DateTime.Now
						, update_by = model.update_by });
						result.isCompleted = true;
						result.message.Add("success");
						return result;
					} else {
						result.isCompleted = false;
						result.message.Add("Tag is not mapping");
						return result;
					}
				} else {
					result.isCompleted = false;
					result.message.Add("Tag not found");
					return result;
				}


			} catch (Exception e) {
				result.isCompleted = false;
				result.message.Add(e.Message);
			}

			return result;
		}
		public async Task<ReturnMessageModel> UpsertItemMaster(MasterItem model)
		{
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = "";
				if (model.mas_item_id != 0)
				{
					query = @"UPDATE [dbo].[mas_item]
							SET [title] = @title
							,[fix_asset] = @fix_asset
							,[top_no] = @top_no
							,[details] = @details
							,[computer_type] = @computer_type
							,[computer_brand] = @computer_brand
							,[serial_number] = @serial_number
							,[computer_model] = @computer_model
							,[mit_ad_user] = @mit_ad_user
							,[location] = @location
							,[location_no] = @location_no
							,[location_sub] = @location_sub
							,[mit_name] = @mit_name
							,[emp_name] = @emp_name
							,[section_code] = @section_code
							,[description] = @description
							,[update_date] = @update_date
							,[update_by] = @update_by
							WHERE mas_item_id=@mas_item_id";
				}
				else
				{
					query = @"INSERT INTO [dbo].[mas_item]
								([title]
								,[fix_asset]
								,[top_no]
								,[status]
								,[details]
								,[computer_type]
								,[computer_brand]
								,[serial_number]
								,[computer_model]
								,[mit_ad_user]
								,[location]
								,[location_no]
								,[location_sub]
								,[mit_name]
								,[emp_name]
								,[section_code]
								,[description]
								,[created_by]
								,[created_date])
							VALUES
								(@title
								,@fix_asset
								,@top_no
								,'ACTIVE'
								,@details
								,@computer_type
								,@computer_brand
								,@serial_number
								,@computer_model
								,@mit_ad_user
								,@location
								,@location_no
								,@location_sub
								,@mit_name
								,@emp_name
								,@section_code
								,@description
								,@created_by
								,@created_date)";
				}
				await conn.ExecuteAsync(query, model);
				if (model.mas_item_id != 0) {
					saveLogMaster(model.mas_item_id);
				} else {
					//get id insert lastest
					var id = conn.Query<long>(@"SELECT IDENT_CURRENT('mas_item')").FirstOrDefault();
					saveLogMaster(id);
				}

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
		// public async Task<ReturnMessageModel> DeleteItemTubeById(long id)
		// {
		// 	var result = new ReturnMessageModel();
		// 	result.isCompleted = false;
		// 	try
		// 	{
		// 		using var conn = await _db.CreateConnectionAsync();
		// 		string query = @"UPDATE mas_ItemTube SET is_deleted = 1 WHERE id = @id";
		// 		await conn.ExecuteAsync(query, new { id });
		// 		result.isCompleted = true;
		// 		result.message.Add("success");
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		result.message.Add(ex.Message);
		// 	}
		// 	return result;
		// }
		public async Task<ReturnMessageModel> DeleteTagId(DeleteTag model)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				// check status mapping
				var check = conn.Query<MasTagMapping>(@"SELECT * FROM mas_tag_mapping WHERE tag_id = @tag_id", new { tag_id = model.tag_id }).FirstOrDefault();
				if (check != null) {
					if (check.status == "mapping") {
						result.isCompleted = false;
						result.message.Add("Tag is mapping");
						return result;
					} else {
						string query = @"UPDATE mas_tag_mapping SET
								update_by = @update_by
								,is_deleted= @is_deleted
								,deleted_by = @deleted_by
								WHERE tag_id = @tag_id";
						await conn.ExecuteAsync(query, new {
							update_by = model.update_by
							,is_deleted = model.is_deleted
							,deleted_by = model.deleted_by
							,tag_id = model.tag_id});
						}
						result.isCompleted = true;
						result.message.Add("success");
				} else {
					result.isCompleted = false;
					result.message.Add("Tag not found");
					return result;
				}

			}
			catch (Exception ex)
			{
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}

		// Template
		public async Task<List<TemplateData>> GetTemplateList()
		{
			var result = new List<TemplateData>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<TemplateData>(@"SELECT * FROM mas_Template WHERE is_deleted = 0  ORDER BY id").ToList();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnList<MasTagMapping>> GetTagList()
		{
			var result = new ReturnList<MasTagMapping>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result.data = conn.Query<MasTagMapping>(@"SELECT m.*
					,i.title
					,i.fix_asset
					,i.top_no
					,i.status as status_item
					,i.details
					,i.computer_type
					,i.computer_brand
					,i.serial_number
					,i.computer_model
					,i.mit_ad_user
				FROM mas_tag_mapping m
				LEFT JOIN mas_item i ON i.mas_item_id = m.mas_item_id
				WHERE m.is_deleted = 0
				ORDER BY m.tag_id").ToList();
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
		public async Task<TemplateData> GetTemplateById(long id)
		{
			var result = new TemplateData();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<TemplateData>(@"SELECT * FROM mas_Template WHERE (id = @id)", new { id }).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> UpsertTemplate(TemplateData model)
		{
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = "";
				if (model.id.HasValue)
				{
					query = @"UPDATE [dbo].[mas_Template]
   SET [template_description] = @template_description
		,[template_name] = @template_name
      ,[modified_by] = @modified_by
      ,[modified_date] = @modified_date
      ,[is_active] = @is_active,template_build_str = @template_build_str
 WHERE id=@id";
				}
				else
				{
					query = @"INSERT INTO [dbo].[mas_Template]
           ([template_code]
			,template_name
           ,[template_description]
           ,[created_by]
           ,[created_date]
           ,[modified_by]
           ,[modified_date]
           ,[is_active],template_build_str
           ,[is_deleted])
     VALUES
           (@template_code
			,@template_name
           ,@template_description
           ,@created_by
           ,@created_date
           ,@modified_by
           ,@modified_date
           ,@is_active,@template_build_str
           ,0)";
				}
				await conn.ExecuteAsync(query, model);

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
		public async Task<ReturnMessageModel> DeleteTemplateById(long id)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = @"UPDATE mas_Template SET is_deleted = 1 WHERE id = @id";
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

        //Template Build
        public async Task<List<TemplateBuildData>> GetTemplateBuildList()
        {
            var result = new List<TemplateBuildData>();
            try
            {
                using var conn = await _db.CreateConnectionAsync();
                result = conn.Query<TemplateBuildData>(@"SELECT * FROM mas_TemplateBuild  ORDER BY column_seq").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<TemplateBuildData> GetTemplateBuildById(long id)
        {
            var result = new TemplateBuildData();
            try
            {
                using var conn = await _db.CreateConnectionAsync();
                result = conn.Query<TemplateBuildData>(@"SELECT * FROM mas_TemplateBuild WHERE (id = @id)", new { id }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public async Task<ReturnMessageModel> UpsertTemplateBuild(TemplateBuildData model)
        {
            var result = new ReturnMessageModel();
            try
            {
                using var conn = await _db.CreateConnectionAsync();
                string query = "";
                if (model.id.HasValue)
                {
                    query = @"UPDATE [dbo].[mas_TemplateBuild]
					SET [column_display_text] = @column_display_text
						,[column_type] = @column_type
						,[column_ddl_value] = @column_ddl_value
						,[column_is_required] = @column_is_required
					,column_seq = @column_seq
						,[modified_by] = @modified_by
						,[modified_date] = @modified_date
						,[is_active] = @is_active
					WHERE  id=@id";
                }
                else
                {
                    query = @"INSERT INTO [dbo].[mas_TemplateBuild]
						([column_code]
						,[column_display_text]
						,[column_type]
						,[column_ddl_value]
						,[column_is_required],column_seq
						,[created_by]
						,[created_date]
						,[modified_by]
						,[modified_date]
						,[is_active])
					VALUES
						(@column_code
						,@column_display_text
						,@column_type
						,@column_ddl_value
						,@column_is_required,@column_seq
						,@created_by
						,@created_date
						,@modified_by
						,@modified_date
						,@is_active)";
                }
                await conn.ExecuteAsync(query, model);

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
        public async Task<ReturnMessageModel> DeleteTemplateBuildById(long id)
        {
            var result = new ReturnMessageModel();
            result.isCompleted = false;
            try
            {
                using var conn = await _db.CreateConnectionAsync();
                string query = @"DELETE FROM mas_TemplateBuild WHERE id = @id";
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
		//Addon
		public async Task<TemplateBuildData> GetTemplateBuildByCode(string code)
		{
			var result = new TemplateBuildData();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<TemplateBuildData>(@"SELECT * FROM mas_TemplateBuild WHERE (column_code = @code)", new { code }).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}

		public async Task<ReturnList<ComputerType>> GetComputerType() {
			var result = new ReturnList<ComputerType>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result.data =  conn.Query<ComputerType>(@"SELECT * FROM sec_computer_type WHERE is_deleted = 0").ToList();
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SaveDataType(RequestSave model) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				if (model.type == "add") {
					string query = @"INSERT INTO [dbo].[sec_computer_type]
							([computer_type]
							,[is_active]
							,[is_deleted]
							,[created_by]
							,[created_date])
						VALUES
							(@computer_type
							,@is_active
							,@is_deleted
							,@created_by
							,@created_date)";
					await conn.ExecuteAsync(query, new { computer_type = model.data
					, is_active = true
					, is_deleted = false
					, created_by = model.action_by
					, created_date = DateTime.Now });
				} else if (model.type == "edit") {
					string query = @"UPDATE [dbo].[sec_computer_type]
							SET [computer_type] = @computer_type
							,[update_by] = @update_by
							,[update_date] = @update_date
						WHERE computer_type_id = @id";
					await conn.ExecuteAsync(query, new { computer_type = model.data
					, update_by = model.action_by
					, update_date = DateTime.Now
					, id = model.id });
				} else if (model.type == "delete") {
					string query = @"UPDATE [dbo].[sec_computer_type]
							SET [is_deleted] = @is_deleted
							,[update_by] = @update_by
							,[update_date] = @update_date
						WHERE computer_type_id = @id";
					await conn.ExecuteAsync(query, new { is_deleted = true
					, update_by = model.action_by
					, update_date = DateTime.Now
					, id = model.id });
				}
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SaveDataCSV(string action_by) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				// var checkDupSql = @"SELECT title, fix_asset, top_no, status, details, computer_type, computer_brand, serial_number, computer_model, mit_ad_user, location, location_no, location_sub, mit_name, emp_name, section_code, description, created_by, created_date
				// 	FROM mas_item_csv
				// 	WHERE not EXISTS (
				// 		SELECT title
				// 		from mas_item_backup
				// 		WHERE mas_item_csv.title = mas_item_backup.title
				// 		AND mas_item_csv.serial_number = mas_item_backup.serial_number
				// 	)";
				var checkDupSql = @"SELECT title, fix_asset, top_no, status, details, computer_type, computer_brand, serial_number, computer_model, mit_ad_user, location, location_no, location_sub, mit_name, emp_name, section_code, description, created_by, created_date, emp_email
					FROM mas_item_csv ";
				var checkDup = conn.Query<MasterItemCSV>(checkDupSql).ToList();
				if (checkDup.Count() > 0) {
					foreach (var item in checkDup)
					{
						var dataMasterQry = @"SELECT * FROM mas_item WHERE title = @title";
						var dataMaster = conn.Query<MasterItem>(dataMasterQry, new { title = item.title }).FirstOrDefault();
						if (dataMaster != null) {
							//update
							var updateQry = @"UPDATE mas_item SET
												title = @title
												,fix_asset = @fix_asset
												,top_no = @top_no
												,status = @status
												,details = @details
												,computer_type = @computer_type
												,computer_brand = @computer_brand
												,serial_number = @serial_number
												,computer_model = @computer_model
												,mit_ad_user = @mit_ad_user
												,location = @location
												,location_no = @location_no
												,location_sub = @location_sub
												,mit_name = @mit_name
												,emp_name = @emp_name
												,section_code = @section_code
												,description = @description
												,update_date = getdate()
												,update_by = @action_by
												,emp_email = @emp_email
												WHERE mas_item_id = @mas_item_id";
							await conn.ExecuteAsync(updateQry, new {
								title = item.title
								,fix_asset = item.fix_asset
								,top_no = item.top_no
								,status = item.status
								,details = item.details
								,computer_type = item.computer_type
								,computer_brand = item.computer_brand
								,serial_number = item.serial_number
								,computer_model = item.computer_model
								,mit_ad_user = item.mit_ad_user
								,location = item.location
								,location_no = item.location_no
								,location_sub = item.location_sub
								,mit_name = item.mit_name
								,emp_name = item.emp_name
								,section_code = item.section_code
								,description = item.description
								,action_by = action_by
								,mas_item_id = dataMaster.mas_item_id
								,emp_email = item.emp_email
							});
							saveLogMaster(dataMaster.mas_item_id);
						} else {
							//insert
							var insertQry = @"INSERT INTO mas_item (title, fix_asset, top_no, status, details, computer_type, computer_brand, serial_number, computer_model, mit_ad_user, location, location_no, location_sub, mit_name, emp_name, section_code, description, created_by, created_date, emp_email, is_deleted )
								VALUES (@title, @fix_asset, @top_no, @status, @details, @computer_type, @computer_brand, @serial_number, @computer_model, @mit_ad_user, @location, @location_no, @location_sub, @mit_name, @emp_name, @section_code, @description, @action_by, getdate(), @emp_email, 0)";
							await conn.ExecuteAsync(insertQry, new {
								title = item.title
								,fix_asset = item.fix_asset
								,top_no = item.top_no
								,status = item.status
								,details = item.details
								,computer_type = item.computer_type
								,computer_brand = item.computer_brand
								,serial_number = item.serial_number
								,computer_model = item.computer_model
								,mit_ad_user = item.mit_ad_user
								,location = item.location
								,location_no = item.location_no
								,location_sub = item.location_sub
								,mit_name = item.mit_name
								,emp_name = item.emp_name
								,section_code = item.section_code
								,description = item.description
								,action_by = action_by
								,emp_email = item.emp_email
							});
							//get id insert lastest
							var id = conn.Query<long>(@"SELECT IDENT_CURRENT('mas_item')").FirstOrDefault();
							saveLogMaster(id);
						}
					}
				}
				// var dupQty = @"
				// 	INSERT INTO mas_item_backup (title, fix_asset, top_no, status, details, computer_type, computer_brand, serial_number, computer_model, mit_ad_user, location, location_no, location_sub, mit_name, emp_name, section_code, description, created_by, created_date )
				// 		SELECT title, fix_asset, top_no, status, details, computer_type, computer_brand, serial_number, computer_model, mit_ad_user, location, location_no, location_sub, mit_name, emp_name, section_code, description, created_by, created_date
				// 	FROM mas_item_csv WHERE not EXISTS (
				// 		SELECT title
				// 		from mas_item_backup
				// 		WHERE mas_item_csv.title = mas_item_backup.title
				// 		AND mas_item_csv.serial_number = mas_item_backup.serial_number
				// 	)
				// ";
				// await conn.ExecuteAsync(dupQty);
				result.isCompleted = true;
				result.message.Add("success");
			} catch (Exception e) {
				result.isCompleted = false;
				result.message.Add(e.Message);
			}
			return result;
		}
		public async Task<ReturnList<MasterItemCSV>> CheckDupplicate(DataCSV model) {
			var result = new ReturnList<MasterItemCSV>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				// JSON.parse(model.data_csv)
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MasterItemCSV>>(model.data_csv);
				await conn.ExecuteAsync(@"TRUNCATE TABLE mas_item_csv");

				var checkDupplicate = conn.Query<MasterItemCSV>(@"SELECT * FROM mas_item_backup WHERE is_deleted = 0").ToList();
				var stringInsert = $@"INSERT INTO [dbo].[mas_item_csv]
								([title]
								,[fix_asset]
								,[top_no]
								,[status]
								,[details]
								,[computer_type]
								,[computer_brand]
								,[serial_number]
								,[computer_model]
								,[mit_ad_user]
								,[location]
								,[location_no]
								,[location_sub]
								,[mit_name]
								,[emp_name]
								,[emp_email]
								,[section_code]
								,[description]
								,[created_by]
								,[created_date]
								) VALUES ";
				var count = 0;
				foreach (var item in data)
				{
					stringInsert += $@"
						(N'{(item.title != "" ? item.title : null) }'
						,N'{(item.fix_asset != "" ? item.fix_asset : null) }'
						,N'{(item.top_no != "" ? item.top_no : null) }'
						,N'{(item.status != "" ? item.status : null) }'
						,N'{(item.details != "" ? item.details : null) }'
						,N'{(item.computer_type != "" ? item.computer_type : null) }'
						,N'{(item.computer_brand != "" ? item.computer_brand : null) }'
						,N'{(item.serial_number != "" ? item.serial_number : null) }'
						,N'{(item.computer_model != "" ? item.computer_model : null) }'
						,N'{(item.mit_ad_user != "" ? item.mit_ad_user : null) }'
						,N'{(item.location != "" ? item.location : null) }'
						,N'{(item.location_no != "" ? item.location_no : null) }'
						,N'{(item.location_sub != "" ? item.location_sub : null) }'
						,N'{(item.mit_name != "" ? item.mit_name : null) }'
						,N'{(item.emp_name != "" ? item.emp_name : null) }'
						,N'{(item.emp_email != "" ? item.emp_email : null) }'
						,N'{(item.section_code != "" ? item.section_code : null) }'
						,N'{(item.description != "" ? item.description : null) }'
						,N'{(model.action_by != "" ? model.action_by : null) }'
						,getdate())
						
					";
					if (count == data.Count - 1) {
						stringInsert += ";";
					} else {
						stringInsert += ",";
					}
					count++;
				}
				await conn.ExecuteAsync(stringInsert);
				// check data dupplicate
				var datadupplicate = new List<MasterItemCSV>();
				var dupQty = @"
					SELECT * FROM mas_item_csv WHERE  EXISTS (
						SELECT title
						from mas_item_backup
						WHERE mas_item_csv.title = mas_item_backup.title
						AND mas_item_csv.serial_number = mas_item_backup.serial_number
					)
				";
				datadupplicate = conn.Query<MasterItemCSV>(dupQty).ToList();
				if (datadupplicate.Count == 0) {
					result.isCompleted = true;
					result.message.Add("success");
				} else {
					result.isCompleted = false;
					result.message.Add("Duplicate data");
					result.data = datadupplicate;

				}


			} catch (Exception e) {
				result.isCompleted = false;
				result.message.Add(e.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SaveDataBrand(RequestSave model) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				if (model.type == "add") {
					string query = @"INSERT INTO [dbo].[sec_computer_brand]
							([computer_brand]
							,[is_active]
							,[is_deleted]
							,[created_by]
							,[created_date])
						VALUES
							(@computer_brand
							,@is_active
							,@is_deleted
							,@created_by
							,@created_date)";
					await conn.ExecuteAsync(query, new { computer_brand = model.data
					, is_active = true
					, is_deleted = false
					, created_by = model.action_by
					, created_date = DateTime.Now });
				} else if (model.type == "edit") {
					string query = @"UPDATE [dbo].[sec_computer_brand]
							SET [computer_brand] = @computer_brand
							,[update_by] = @update_by
							,[update_date] = @update_date
						WHERE computer_brand_id = @id";
					await conn.ExecuteAsync(query, new { computer_brand = model.data
					, update_by = model.action_by
					, update_date = DateTime.Now
					, id = model.id });
				} else if (model.type == "delete") {
					string query = @"UPDATE [dbo].[sec_computer_brand]
							SET [is_deleted] = @is_deleted
							,[update_by] = @update_by
							,[update_date] = @update_date
						WHERE computer_brand_id = @id";
					await conn.ExecuteAsync(query, new { is_deleted = true
					, update_by = model.action_by
					, update_date = DateTime.Now
					, id = model.id });
				}
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnList<ComputerBrand>> GetComputerBrand() {
			var result = new ReturnList<ComputerBrand>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result.data =  conn.Query<ComputerBrand>(@"SELECT * FROM sec_computer_brand WHERE is_deleted = 0").ToList();
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SaveTag(List<MasTagMapping> model) {
			var result = new ReturnList<ComputerBrand>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				//insert tag
				foreach (var item in model)
				{
					// check before insert
					var check = conn.Query<MasTagMapping>(@"SELECT * FROM mas_tag_mapping WHERE tag_serial = @tag_serial", new { tag_serial = item.tag_serial }).FirstOrDefault();
					if (check != null) {
						continue;
					}
					string query = @"INSERT INTO [dbo].[mas_tag_mapping]
							([tag_serial]
							,[created_date]
							,[created_by]
							,[status]
							,[is_deleted]
							,[deleted_by])
						VALUES
							(@tag_serial
							,@created_date
							,@created_by
							,@status
							,@is_deleted
							,@deleted_by)";
					await conn.ExecuteAsync(query, item);
				}

				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async void saveLogMaster(long mas_item_id) {
			try
			{
					// string qryGet = $@"SELECT ms.title, ms.fix_asset, ms.computer_type, ms.computer_brand, ms.serial_number, ms.computer_model, ms.mit_name, ms.emp_name, ms.description , ec.name
					// 	,CASE
					// 		WHEN ec.name is not null THEN 1
					// 		ELSE ms.is_allow
					// 	END AS is_allow 
					// 	,CASE
					// 		WHEN ec.name is not null THEN 'member vip'
					// 		ELSE ms.type_allow
					// 	END AS type_allow 
					// 	,CASE
					// 		WHEN ec.name is not null THEN null
					// 		ELSE  ms.aging_date
					// 	END AS aging_date 
					// 	FROM mas_item as ms
					// 	LEFT JOIN sec_employee_config as ec ON ec.name = ms.emp_name AND ec.is_active = 1 AND ec.is_delete = 0
					// 	WHERE ms.is_allow = 1 ";
				using var conn = await _db.CreateConnectionAsync();
				// insert select
				var qry = @"INSERT INTO mas_item_log (mas_item_id, title, fix_asset, top_no, status, details, computer_type, computer_brand, serial_number, computer_model, mit_ad_user, location, location_no, location_sub, mit_name, emp_name, section_code, description, created_by, created_date, update_by, update_date, is_allow, aging_date, type_allow, emp_email)
							SELECT ms.mas_item_id, ms.title, ms.fix_asset, ms.top_no, ms.status, ms.details, ms.computer_type, ms.computer_brand, ms.serial_number, ms.computer_model, ms.mit_ad_user, ms.location, ms.location_no, ms.location_sub, ms.mit_name, ms.emp_name, ms.section_code, ms.description, ms.created_by, ms.created_date, ms.update_by, ms.update_date, ms.is_allow, ms.aging_date
							,CASE
								WHEN ec.name is not null THEN 'privilege'
								ELSE ms.type_allow
							END AS type_allow 
							, ec.email
							FROM mas_item as ms
							LEFT JOIN sec_employee_config as ec ON ec.name = ms.emp_name AND ec.is_active = 1 AND ec.is_delete = 0
							WHERE ms.mas_item_id = @mas_item_id";
				await conn.ExecuteAsync(qry, new { mas_item_id });
			}
			catch (Exception ex)
			{

			}
		}
		public async Task<ReturnMessageModel> UpdateAllow(requset_allow_masterItem model) {
			var result = new ReturnList<ComputerBrand>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var update_type = string.Empty;
				if (model.is_allow)
				{
					update_type = $@",type_allow ='{model.type_allow}'";
				}
				string qryUpdate = $@"UPDATE mas_item SET
												is_allow = '{model.is_allow}'
												,aging_date= '{model.aging_date}'
												,allow_day = '{model.allow_day}'
												,update_date = getdate()
												,update_by ='{model.update_by}'
												{update_type}
												WHERE mas_item_id = '{model.mas_item_id}'";
				await conn.ExecuteAsync(qryUpdate);
				saveLogMaster(model.mas_item_id);
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnList<ReportAllowData>> GetreportAllowData() {
			var result = new ReturnList<ReportAllowData>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryGet = $@"SELECT ms.title, ms.fix_asset, ms.computer_type, ms.computer_brand, ms.serial_number, ms.computer_model, ms.mit_name, ms.emp_name, ms.description , ec.name
						,CASE
							WHEN ec.name is not null THEN 1
							ELSE ms.is_allow
						END AS is_allow
						,CASE
							WHEN ec.name is not null THEN 'member vip'
							ELSE ms.type_allow
						END AS type_allow
						,CASE
							WHEN ec.name is not null THEN null
							ELSE  ms.aging_date
						END AS aging_date
						FROM mas_item as ms
						LEFT JOIN sec_employee_config as ec ON ec.name = ms.emp_name AND ec.is_active = 1 AND ec.is_delete = 0
						WHERE ms.is_allow = 1 ";
				var resQry =  conn.Query<ReportAllowData>(qryGet).ToList();
				result.data = resQry;
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SetActiveDataMember(SetActiveRequest model) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryUpdate = $@"UPDATE sec_employee_config SET
												is_active= @status
												,update_date = getdate()
												,update_by = @update_by
												WHERE employee_id = @employee_id";
				await conn.ExecuteAsync(qryUpdate, new { status = model.status, update_by = model.action_by, employee_id = model.employee_id });
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnList<SetVipDataRequest>> GetVIPData() {
			var result = new ReturnList<SetVipDataRequest>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryGet = $@"SELECT * FROM sec_employee_config WHERE is_delete = 0";
				var resQry =  conn.Query<SetVipDataRequest>(qryGet).ToList();
				result.data = resQry;
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> SetVIPData(SetVipDataRequest model) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				if (model.type_request == "add") {
					// insert
					//check dupp
					var checkDup = conn.Query<SetVipDataRequest>(@"SELECT * FROM sec_employee_config WHERE name = @name", new { name = model.name }).FirstOrDefault();
					if (checkDup != null) {
						if (checkDup.is_delete) {
							// update
							string qryUpdate = $@"UPDATE sec_employee_config SET
													is_delete = 0
													,update_date = getdate()
													,update_by = @update_by
													WHERE employee_id = @employee_id";
							await conn.ExecuteAsync(qryUpdate, new { name = checkDup.name, update_by = model.action_by, employee_id = checkDup.employee_id });
						} else {
							result.isCompleted = false;
							result.message.Add("Name is dupplicate");
							return result;
						}
					} else {
						string qryInsert = $@"INSERT INTO sec_employee_config (name, desciption, created_date, created_by, is_active)
							VALUES (@name, @desciption, getdate(), @create_by, 1)";
						await conn.ExecuteAsync(qryInsert, new { name = model.name, desciption = model.desciption, create_by = model.action_by });
					}
				}
				if (model.type_request == "edit") {
					// update
					string qryUpdate = $@"UPDATE sec_employee_config SET
													name = @name
													,desciption= @desciption
													,update_date = getdate()
													,update_by = @update_by
													WHERE employee_id = @employee_id";
					await conn.ExecuteAsync(qryUpdate, new { name = model.name, desciption = model.desciption, update_by = model.action_by, employee_id = model.employee_id });
				}

				if (model.type_request == "delete") {
					// delete
					string qryDelete = $@"UPDATE sec_employee_config SET
													is_delete = 1
													,update_date= getdate()
													,update_by = @update_by
													WHERE employee_id = @employee_id";
					await conn.ExecuteAsync(qryDelete, new { update_by = model.action_by, employee_id = model.employee_id });
				}
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
			return result;
		}
		public async Task<ReturnList<MasterItemLog>> GetReportLog(MasLogRequest model) {
			var result = new ReturnList<MasterItemLog>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryGet = $@"SELECT * FROM mas_item_log WHERE 1 = 1";
				if (model.title != null) {
					qryGet += $@" AND title LIKE '%{model.title}%'";
				}
				if (model.fix_asset != null) {
					qryGet += $@" AND fix_asset LIKE '%{model.fix_asset}%'";
				}
				if (model.emp_name != null) {
					qryGet += $@" AND emp_name LIKE '%{model.emp_name}%'";
				}
				if (model.serial_number != null) {
					qryGet += $@" AND serial_number LIKE '%{model.serial_number}%'";
				}
				if (model.start_date != null) {
					qryGet += $@" AND log_date >= '{model.start_date} 00:00:00'";
				} else {
					qryGet += $@" AND log_date >= '{DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")} 00:00:00'";
				}
				if (model.end_date != null) {
					qryGet += $@" AND log_date <= '{model.end_date} 23:59:59'";
				} else {
					qryGet += $@" AND log_date <= '{DateTime.Now.ToString("yyyy-MM-dd")} 23:59:59'";
				}
				// order by
				qryGet += $@" ORDER BY mas_item_log_id DESC";
				var resQry =  conn.Query<MasterItemLog>(qryGet).ToList();
				result.data = resQry;
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = false;
				result.message.Add(ex.Message);
			}
				return result;
		}


		public async Task<ReturnList<MasterItem>> GetDataItem()
		{
			var result = new ReturnList<MasterItem>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var list = conn.Query<MasterItem>($@"SELECT
														s.is_active as is_vip,
														s.is_delete as is_del_vip,
														m.*
														FROM mas_item  m
														LEFT JOIN sec_employee_config s on m.emp_name = s.name
														WHERE m.is_deleted = 0
														ORDER BY mas_item_id").ToList();
				foreach (var item in list) {
					if (item.is_vip && !item.is_del_vip) {
						item.is_allow = true;
						item.type_allow = "vip";
					}
				result.data = list;
				}
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
		public async Task<ReturnList<event_tag_model>>GetEventTag()
		{
			var result = new ReturnList<event_tag_model>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();

				string qry = $@"SELECT TOP 10
									e.id,
									e.title,
									e.is_alert,
									e.created_date,
									s.is_active as is_vip,
									s.is_delete as is_del_vip,
									is_allow,
									e.type_allow,
									m.emp_name,
									m.mit_name,
									m.mit_ad_user,
									m.serial_number,
									m.computer_brand,
									m.computer_model,
									aging_date,
									computer_type
									FROM tran_EventTag e
									LEFT JOIN mas_item m on m.title = e.title
									LEFT JOIN sec_employee_config s on m.emp_name = s.name
									WHERE m.is_deleted =0 and e.is_alert =1 and e.created_date >= CAST(GETDATE() as DATE)
									ORDER BY id Desc ";
                var list = conn.Query<event_tag_model>(qry).ToList();
				result.data = list;
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
		public async Task<ReturnList<event_tag_model>>GetEventTagAllDay()
		{
			var result = new ReturnList<event_tag_model>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();

				string qry = $@"SELECT TOP 10
									e.id,
									e.title,
									e.is_alert,
									e.created_date,
									s.is_active as is_vip,
									s.is_delete as is_del_vip,
									is_allow,
									e.type_allow,
									m.emp_name,
									m.mit_name,
									m.mit_ad_user,
									m.serial_number,
									m.computer_brand,
									m.computer_model,
									aging_date,
									computer_type
									FROM tran_EventTag e
									LEFT JOIN mas_item m on m.title = e.title
									LEFT JOIN sec_employee_config s on m.emp_name = s.name
									WHERE m.is_deleted =0 and e.is_alert =1 and e.created_date >= CAST(GETDATE() as DATE)
									ORDER BY id Desc ";
                var list = conn.Query<event_tag_model>(qry).ToList();
				foreach (var item in list) {
					if (item.is_vip && !item.is_del_vip) {
						item.is_allow = true;
						item.type_allow = "vip";
					}
				result.data = list;
				}
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
		public async Task<ReturnMessageModel>SaveTagEvent(save_event_tag_list model)
		{
			var result = new ReturnMessageModel();
			try
			{
				var TimeSaveTag = _config.GetSection("TimeSaveTag").Value;
				using var conn = await _db.CreateConnectionAsync();
				
				foreach (var item in model.eventList){
					string query = $@"INSERT INTO tran_EventTag(
							title
							,is_alert
							,type_allow
							,created_date
							,updated_date)
						VALUES(
							'{item.title}'
							,'{item.is_alert}'
							,'{item.type_allow}'
							,getdate()
							,getdate())";
					var checkTitle = conn.Query<event_tag_model>($@"SELECT * FROM tran_EventTag WHERE title = '{item.title}' order by id desc").FirstOrDefault();
					if (checkTitle == null) {
						await conn.ExecuteAsync(query);
					} else {	
						var nowDate = DateTime.Now;
						var dateTag = checkTitle.created_date.AddMinutes(Convert.ToInt32(TimeSaveTag));
						if (dateTag <= nowDate) {
							await conn.ExecuteAsync(query);
						}
					}
				}
				result.isCompleted = true;
				result.message.Add("success");
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
			}
			return result;
		}

		public async Task<ReturnList<TagLog>> GetReportTagLog(TagLogRequest model) {
			var result = new ReturnList<TagLog>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryGet = $@"SELECT te.type_allow, mi.title, mi.fix_asset, mi.computer_type, mi.computer_brand, mi.computer_model, mi.emp_name, mi.mit_name, mi.mit_ad_user, mi.serial_number, te.created_date as tag_location_date, te.is_alert
				FROM tran_EventTag as te
				LEFT JOIN mas_item as mi ON mi.title = te.title
				WHERE 1 = 1 ";
				if (model.title != null) {
					qryGet += $@" AND mi.title LIKE '%{model.title}%'";
				}
				if (model.serial_number != null) {
					qryGet += $@" AND mi.tag_serial LIKE '%{model.serial_number}%'";
				}
				if (model.start_date != null) {
					qryGet += $@" AND te.created_date >= '{model.start_date} 00:00:00'";
				} else {
					qryGet += $@" AND te.created_date >= '{DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")} 00:00:00'";
				}
				if (model.end_date != null) {
					qryGet += $@" AND te.created_date <= '{model.end_date} 23:59:59'";
				} else {
					qryGet += $@" AND te.created_date <= '{DateTime.Now.ToString("yyyy-MM-dd")} 23:59:59'";
				}
				// order by
				qryGet += $@" ORDER BY te.id DESC";
				var resQry =  conn.Query<TagLog>(qryGet).ToList();
				result.data = resQry;
				result.isCompleted = true;
				result.message.Add("success");
				return result;
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = true;
				result.message.Add(ex.Message);
				return result;
			}
		}

		public async Task<ReturnMessageModel> DeleteItemMasItem(DeleteItemMasItemReq model) {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryDelete = $@"UPDATE mas_item SET
												is_deleted = 1
												,update_date = getdate()
												,update_by = @action_by
												WHERE mas_item_id = @mas_item_id";
				await conn.ExecuteAsync(qryDelete, new { action_by = model.action_by, mas_item_id = model.mas_item_id });
				result.isCompleted = true;
				result.message.Add("success");
				return result;
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = false;
				result.message.Add(ex.Message);
				return result;
			}
		}
		public async Task<ReturnObject<MasterItem>> GetItemMasterDetail(string title = "") {
			var result = new ReturnObject<MasterItem>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryGet = $@"SELECT * FROM mas_item WHERE title = @title";
				var resQry =  conn.Query<MasterItem>(qryGet, new { title }).FirstOrDefault();
				result.data = resQry;
				result.isCompleted = true;
				result.message.Add("success");
				
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = false;
				result.message.Add(ex.Message);
				
			}
			return result;
		}

		public async Task<ReturnMessageModel> CheckAllowPerson() {
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string qryGet = $@"SELECT  ms.mas_item_id
						, ms.aging_date 
						, ms.title
						, ms.fix_asset
						, ms.emp_name
						, ms.details
						, ms.computer_type
						, ms.computer_brand
						, ms.computer_model
						, ms.serial_number
						, FORMAT(DATEADD(day, -3, ms.aging_date), 'yyyy-MM-dd') as notice_date
						, FORMAT(GETDATE(), 'yyyy-MM-dd') as now_date
						, mail.is_send 
							FROM mas_item as ms
							LEFT JOIN sec_send_mail as mail ON mail.title = ms.title 
							WHERE ms.type_allow = 'normal' 
							AND FORMAT(DATEADD(day, -3, ms.aging_date), 'yyyy-MM-dd') = FORMAT(GETDATE(), 'yyyy-MM-dd')
							AND (mail.is_send = 0 OR mail.is_send is null)";
				var resQry =  conn.Query<CheckNotice>(qryGet).ToList();
				if (resQry.Count() > 0) {
					var getMailByUser = $@"SELECT CONCAT(firstname, ' ',lastname) as name, email 
						FROM sec_User 
						WHERE is_deleted = 0 
						AND is_active = 1 
						AND email is not null 
						AND (is_admin = 1 OR is_staff = 1) ";
					var resMail =  conn.Query<MailUser>(getMailByUser).ToList();
					if (resMail.Count() > 0) {
						foreach (var item in resQry) {
							if (!item.is_send) {
								// send mail
								var modelMailDetail = new MasterItem() {
									title = item.title
									,fix_asset = item.fix_asset
									,emp_name = item.emp_name
									,details = item.details
									,computer_type = item.computer_type
									,computer_brand = item.computer_brand
									,computer_model = item.computer_model
									,serial_number = item.serial_number
								};
								var body = await _mail.RenderToStringAsync("Master/EmailTemplate", modelMailDetail);
								
								foreach (var itemMail in resMail) {
									var modelSend = new MailSender()
									{
										send_to = itemMail.email,
										send_name = itemMail.name,
										subject = "แจ้งเตือนการยืมอุปกรณ์ " + item.title,
										body = body
									};
									var resSendMail = await _mail.SendMail(modelSend);
								}
								var mail = new SendMailLog();
								mail.title = item.title;
								mail.is_send = true;
								// mail.created_date = DateTime.Now;
								// mail.created_by = "system";
								await conn.ExecuteAsync(@"INSERT INTO sec_send_mail 
								(title, is_send, create_date, create_by) 
								VALUES (@title, @is_send,getdate(), 'system')", mail);
							}
							result.isCompleted = true;
							result.message.Add("success");
						}
					} else {
						result.isCompleted = false;
						result.message.Add("not found email");
					}
				} else {
					result.isCompleted = true;
					result.message.Add("not found");
				}
				
			}
			catch (Exception ex)
			{
				// throw new Exception(ex.Message);
				result.isCompleted = false;
				result.message.Add(ex.Message);
				
			}
			return result;
		}
	}
}
