using Dapper;
using LGC.BNP.MIKUNI.Web.Models;
using System.Security.Cryptography;
using System.Text;

namespace LGC.BNP.MIKUNI.Web.Services
{
	public class AccountService
	{
		IDatabaseConnectionFactory _db; 
		public AccountService(IDatabaseConnectionFactory _database)
		{	
			_db = _database;
		}
		 public async Task<UserData> PostLogin(UserData model)
        {
            try
            {
                using var conn = await _db.CreateConnectionAsync();
                var user = new UserData();

				string password = model.password;
				byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
				byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string encodedPass = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

				var checkUser = conn.Query<UserData>(@"SELECT * FROM sec_User WHERE username = '"+ model.username + "' AND password = '"+ encodedPass + "'").FirstOrDefault();

                if (checkUser != null)
                {
					user = checkUser;
				}
				else
                {
                    user.error_message = "Invalid Username / Password !!";
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
		public async Task<List<UserData>> GetList()
		{
			var result = new List<UserData>();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<UserData>(@"SELECT * FROM sec_User WHERE is_deleted = 0  ORDER BY id").ToList();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<UserData> GetById(long id)
		{
			var result = new UserData();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				result = conn.Query<UserData>(@"SELECT * FROM sec_User WHERE (id = @id)", new { id }).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> Upsert(UserData model)
		{
			var result = new ReturnMessageModel();
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = "";
				if (model.id.HasValue)
				{
					query = @"UPDATE [dbo].[sec_User]
						SET [firstname] = @firstname
							,[lastname] = @lastname
							,[position] = @position
							,[department] = @department
							,[is_admin] = @is_admin
							,[is_staff] = @is_staff
							,[is_reporter] = @is_reporter
							,[email] = @email
						
							,[modified_by] = @modified_by
							,[modified_date] = @modified_date
							,[is_active] = @is_active
							,[username] = @username
							,[password] = @password
							,[profile_picture] = @profile_picture
						WHERE id = @id ";
				}
				else
				{
					query = @"INSERT INTO [dbo].[sec_User]
           ([firstname]
           ,[lastname]
           ,[position]
           ,[department]
           ,[is_admin]
           ,[is_staff]
           ,[is_reporter]
         
           ,[created_by]
           ,[created_date]
           ,[modified_by]
           ,[modified_date]
           ,[is_active]
           ,[username]
           ,[email]
           ,[password]
           ,[profile_picture], is_deleted)
			VALUES
           (@firstname
           ,@lastname
           ,@position
           ,@department
           ,@is_admin
           ,@is_staff
           ,@is_reporter
         
			,@created_by
           ,@created_date
           ,@modified_by
           ,@modified_date
           ,@is_active
           ,@username
		   ,@email
           ,@password
           ,@profile_picture, 0)";
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
		public async Task<string> GetRoleById(long id)
		{
			string result = "";
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				var data = conn.Query<UserData>(@"SELECT * FROM sec_User WHERE (id = @id)", new { id }).FirstOrDefault();
				if (data.is_staff)
				{
					result = "STAFF";
				}
				else if (data.is_admin)
				{
					result = "ADMINISTRATOR";
				}
				else if (data.is_reporter)
				{
					result = "REPORTER";
				}
				else
				{
					result = "NA";
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return result;
		}
		public async Task<ReturnMessageModel> DeleteById(long id)
		{
			var result = new ReturnMessageModel();
			result.isCompleted = false;
			try
			{
				using var conn = await _db.CreateConnectionAsync();
				string query = @"UPDATE sec_User SET is_deleted = 1 WHERE id = @id";
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
	}
}
