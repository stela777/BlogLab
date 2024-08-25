using BlogLab.Models.BlogComment;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    class BlogCommentRepository : IBlogCommentRepository
    {

        private readonly IConfiguration _config;

        public BlogCommentRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> DeleteAsync(int blogCommentId)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                affectedRows = await connection.ExecuteAsync(
                   //procedure
                    "BlogComment_Delete",
                    new { BlogCommentId = blogCommentId },
                    commandType: CommandType.StoredProcedure);

            }
            return affectedRows;
        }

        public async Task<List<BlogComment>> GetAllAsync(int blogId)
        {
            //photos list
            IEnumerable<BlogComment> blogComments;

            //then we create a connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                blogComments = await connection.QueryAsync<BlogComment>(
                    //procedure, where we get from
                    "BlogComment_GetAll",
                    //what we're going to pass in, the blog id we got from our method
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);
            }
            return blogComments.ToList();
        }

        public async Task<BlogComment> GetAsync(int blogCommentId)
        {
            //BlogComment list
            BlogComment blogComment;

            //then we create a connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();


                //<BlogComment> as the type is going to be returned
                blogComment = await connection.QueryFirstOrDefaultAsync<BlogComment>(
                   //we want to call the BlogComment store procedure
                "BlogComment_Get",
                    new { BlogCommentId = blogCommentId },
                    commandType: CommandType.StoredProcedure);
            }
            return blogComment;
        }

        public async Task<BlogComment> UpsertAsync(BlogCommentCreate blogCommentCreate, int applicationUserId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogCommentId", typeof(int));
            dataTable.Columns.Add("ParentBlogCommentId", typeof(int));
            dataTable.Columns.Add("BlogId", typeof(int));
            dataTable.Columns.Add("Content", typeof(string));


            dataTable.Rows.Add(
                blogCommentCreate.BlogCommentId, 
                blogCommentCreate.ParentBlogCommentId, 
                blogCommentCreate.BlogId,
                blogCommentCreate.Content);

            int? newBlogCommentId;


            //then we create a connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                newBlogCommentId = await connection.ExecuteScalarAsync<int?>(
                "BlogComment_Upsert",
                    new { BlogComment = dataTable.AsTableValuedParameter("dbo.BlogCommentType") },
                    commandType: CommandType.StoredProcedure);
            }

            newBlogCommentId = newBlogCommentId ?? blogCommentCreate.BlogCommentId;

            BlogComment blogComment = await GetAsync(newBlogCommentId.Value);

            return blogComment;
        }
    }
}
