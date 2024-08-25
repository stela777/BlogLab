using BlogLab.Models.Blog;
using BlogLab.Models.Photo;
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
    public class BlogRepository : IBlogRepository
    {
        private readonly IConfiguration _config;

        public BlogRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> DeleteAsync(int blogId)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                affectedRows = await connection.ExecuteAsync(
                    "Blog_Delete",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);

            }
            return affectedRows;
        }

        public async Task<PagedResults<Blog>> GetAllAsync(BlogPaging blogPaging)
        {
            //Initializes an empty 'PagedResults<Blog> object which will eventually  hold the paginated
            //blogs and the total account of all blogs
            var results = new PagedResults<Blog>();

            //Database Connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                //Asynchronously opens the connection to the database
                await connection.OpenAsync();


                using (var multi = await connection.QueryMultipleAsync("Blog_All",
                    new
                    {
                        Offset = (blogPaging.Page -1) * blogPaging.PageSize,
                        PageSize = blogPaging.PageSize 
                    }, 
                    commandType: CommandType.StoredProcedure))
                {
                    results.Items = multi.Read<Blog>();
                    results.TotalCount = multi.ReadFirst<int>();
                }
                
            }
            return results;
        }

        public async Task<List<Blog>> GetAllByUserIdAsync(int applicationUserId)
        {
            //Blog list
            IEnumerable<Blog> blogs;

            //then we create a connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                blogs = await connection.QueryAsync<Blog>(
                    "Blog_GetByUserId",
                    new { ApplicationUserId = applicationUserId },
                    commandType: CommandType.StoredProcedure);
            }
            return blogs.ToList();

        }

        public async Task<List<Blog>> GetAllFamousAsync()
        {
            //Blog list
            IEnumerable<Blog> famousblogs;

            //then we create a connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                famousblogs = await connection.QueryAsync<Blog>(
                    "Blog_GetAllFamous",
                    new { },
                    commandType: CommandType.StoredProcedure);
            }
            return famousblogs.ToList();
        }

        public async Task<Blog> GetAsync(int blogId)
        {
            //photos list
            Blog blog;

            //then we create a connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                blog = await connection.QueryFirstOrDefaultAsync<Blog>(
                "Blog_Get",
                    new { BlogId = blogId },
                    commandType: CommandType.StoredProcedure);
            }
            return blog;
        }

        public async Task<Blog> UpsertAsync(BlogCreate blogCreate, int applicationUserId)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogId", typeof(int));
            dataTable.Columns.Add("Title", typeof(string));
            dataTable.Columns.Add("Content", typeof(string)); 
            dataTable.Columns.Add("PhotoId", typeof(int));

            dataTable.Rows.Add(blogCreate.BlogId, blogCreate.Title, blogCreate.Content, blogCreate.PhotoId);

            int? newBlogId;

            //then we create a connection
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                newBlogId = await connection.ExecuteScalarAsync<int?>(
                    "Blog_Upsert",
                    new { Blog = dataTable.AsTableValuedParameter("dbo.BlogType"), ApplicationUserId = applicationUserId},
                    commandType: CommandType.StoredProcedure);
            }
            newBlogId = newBlogId ?? blogCreate.BlogId;

            Blog blog = await GetAsync(newBlogId.Value);

            return blog;
        }
    }
}
