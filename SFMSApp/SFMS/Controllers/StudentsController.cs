using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFMS.Models;

namespace SFMS.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SFMSContext _dbcontext;
        private readonly HttpClient _httpClient;
        public const string currentstudent = "_";
        public string idNum = "";
        public byte[] studentimg = new byte[0];

        public StudentsController(SFMSContext dbcontext, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _dbcontext = dbcontext;
            _httpClient.BaseAddress = new Uri("http://localhost:5171");
        }

        [HttpGet, Route("Search")]
        public IActionResult Index()
        {
            string? query = HttpContext.Session.GetString(idNum);
            ViewBag.query = query;  
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(Query query)
        {
            var id = query.StudentNumber;

            TempData["id"] = id;

            HttpContext.Session.SetString(idNum, id);

            HttpResponseMessage response = await _httpClient.GetAsync($"/Info/{id}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString(currentstudent, content);
                return RedirectToAction("Student", new { id = id});
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                return RedirectToAction("Failure");
            }
        }

        [HttpGet("[controller]/[action]/{id}")]
        public async Task<IActionResult> Student(int id) 
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(currentstudent)))
            {
                var stages = await _dbcontext.Stages.ToListAsync();
                var student = HttpContext.Session.GetString(currentstudent);
                ViewBag.student = JsonConvert.DeserializeObject<dynamic>(student);
                ViewBag.id = id;
                if (!string.IsNullOrEmpty(student))
                {
                    return View(stages);
                }
            }
            return RedirectToAction("Failure");
        }

        [HttpGet]
        [Route("[controller]/Student/{id}/{slug}")]
        public async Task<IActionResult> Stage(int id, string slug)
        {
            try
            {
                var stages = await _dbcontext.Stages.ToListAsync();
                var student = HttpContext.Session.GetString(currentstudent);
                ViewBag.student = JsonConvert.DeserializeObject<dynamic>(student);

                Dictionary<string, object> jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(student);

                List<string> keys = new List<string>();
                foreach (var kvp in jsonData)
                {
                    keys.Add(kvp.Key);
                    //object value = kvp.Value;
                }

                ViewBag.Keys = keys;

                return View(stages);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult NotFound()
        {
            ViewBag.id = TempData["id"];
            return View();
        }

        [HttpGet]
        public IActionResult Failure()
        {
            return View();
        }

        public ActionResult GetImage()
        {
            // Replace this with your actual byte array data.
            var std = HttpContext.Session.GetString(currentstudent);
            studentimg = JsonConvert.DeserializeObject<dynamic>(std).photo;

            if (studentimg != null)
            {
                // Create a MemoryStream from the byte array.
                using (MemoryStream stream = new MemoryStream(studentimg))
                {
                    // Create an Image object from the MemoryStream.
                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

                    // Return the image as a FileResult with the appropriate content type.
                    return File(stream.ToArray(), "image/jpeg"); // Change content type as needed
                }
            }
            else
            {
                // Handle the case where the byte array is null (e.g., no image data found).
                // You can return a default image or an error message.
                return null;
            }
        }
    }
}
