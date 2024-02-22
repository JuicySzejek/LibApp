using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LibApp.Models;
using LibApp.ViewModels;
using LibApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace LibApp.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly string apiUrl = "https://localhost:7192/api/books";
        private readonly IHttpClientFactory _httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Authorize(Roles = "user, storemanager, owner")]
        public async Task<IActionResult> Index()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(apiUrl);
            var books = JsonConvert.DeserializeObject<List<Book>>(response);

            return View(books);
        }

        [Authorize(Roles = "storemanager, owner")]
        public async Task<IActionResult> Edit(int id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync($"{apiUrl}/{id}");
            var book = JsonConvert.DeserializeObject<Book>(response);

            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookFormViewModel
            {
                Book = book,
                Genres = await GetGenresFromApiAsync()
            };

            return View("BookForm", viewModel);
        }

        [Authorize(Roles = "user, storemanager, owner")]
        public async Task<ActionResult> Details(int id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync($"{apiUrl}/{id}");
            var book = JsonConvert.DeserializeObject<Book>(response);

            if (book == null)
            {
                return Content("Book not found");
            }

            return View(book);
        }

        [Authorize(Roles = "storemanager, owner")]
        public async Task<IActionResult> New()
        {
            var viewModel = new BookFormViewModel
            {
                Genres = await GetGenresFromApiAsync()
            };

            return View("BookForm", viewModel);
        }
                
        [HttpPost]
        [Authorize(Roles = "storemanager, owner")]
        public async Task<IActionResult> Save(Book book)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    
                    if (book == null)
                        return BadRequest("Invalid book object");
                    

                    if (book.Id == 0)
                    {
                        book.DateAdded = DateTime.Now;

                        var postResponse = await httpClient.PostAsJsonAsync(apiUrl, book);

                        // Check if the response indicates success
                        if (!postResponse.IsSuccessStatusCode)
                        {
                            // Optionally, log or handle the response details
                            Console.WriteLine($"API request failed: {postResponse.StatusCode}");

                            // Extract and log the content of the response
                            var responseContent = await postResponse.Content.ReadAsStringAsync();
                            Console.WriteLine($"Response Content: {responseContent}");

                            return StatusCode((int)postResponse.StatusCode);
                        }
                    }
                    else
                    {
                        var putResponse = await httpClient.PutAsJsonAsync($"{apiUrl}/{book.Id}", book);

                        // Check if the response indicates success
                        if (!putResponse.IsSuccessStatusCode)
                        {
                            // Optionally, log or handle the response details
                            Console.WriteLine($"API request failed: {putResponse.StatusCode}");
                            return StatusCode((int)putResponse.StatusCode);
                        }
                    }
                }

                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500); // Or return an appropriate error response
            }
        }

        [Authorize(Roles = "user, storemanager, owner")]
        private async Task<List<Genre>> GetGenresFromApiAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync("https://localhost:7192/api/books/genres");
            return JsonConvert.DeserializeObject<List<Genre>>(response);
        }
    }
}
