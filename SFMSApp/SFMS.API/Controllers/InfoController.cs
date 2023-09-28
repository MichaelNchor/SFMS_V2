using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using Newtonsoft.Json;

namespace SFMS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : Controller
    {
        private readonly IConfiguration _configuration;

        public InfoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public IActionResult Index(string id)
        {
            string connectionString = _configuration.GetConnectionString("Info");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Now you can execute raw SQL queries here using the connection object.
                using (SqlCommand command = new SqlCommand("SELECT * FROM Student join Studentphoto ON StudentPhoto.studentid=Student.studentid WHERE Student.studentid = @id;", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var student = new Dictionary<string, object>();

                            // Loop through the columns and add them to the dictionary
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                // Convert column names to lowercase and remove square brackets
                                string columnName = reader.GetName(i).Trim('[', ']').ToLower();
                                student[columnName] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }

                            return new JsonResult(student) { StatusCode = 200 };
                        }
                    }
                }
            }

            return NotFound(); ;
        }
    }
}
