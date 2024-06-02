using Microsoft.AspNetCore.Mvc;
using NRBD_LAB_Mongo_ASP.Models;
using System.Diagnostics;

namespace NRBD_LAB_Mongo_ASP.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IMusicRepository _musicRepository;

		public HomeController(ILogger<HomeController> logger, IMusicRepository musicRepository)
		{
			_logger = logger;
			_musicRepository = musicRepository;
		}

		public async Task<IActionResult> Index(MusicAlbumFilter filter)
		{
			var musicAlbumList = new MusicAlbumList
			{
				MusicAlbums = (await _musicRepository.GetAlbums(filter.Year, filter.AlbumTitle)).ToList(),
				Filter = filter
			};

			return View(musicAlbumList);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(MusicAlbumModel album, IFormFile image)
		{
			if (!ModelState.IsValid) return View(album);
			
			var stream = new MemoryStream();
			image.CopyTo(stream);

			album.Image = Convert.ToBase64String(stream.ToArray());
			_musicRepository.AddImage(album._id, stream);

			_musicRepository.Add(album);
			return RedirectToAction("Index");

		}
		
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var album = await _musicRepository.GetById(id);
			return View(album);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			await _musicRepository.Delete(id);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			var album = await _musicRepository.GetById(id);
			if (album == null)
				return NotFound();

			return View(album);
		}

		[HttpPost]
		public IActionResult Edit(MusicAlbumModel album, IFormFile image)
		{
			if (!ModelState.IsValid) return View(album);

			var stream = new MemoryStream();
			image.CopyTo(stream);

			album.Image = Convert.ToBase64String(stream.ToArray());
			_musicRepository.AddImage(album._id, stream);

			_musicRepository.Update(album);
			return RedirectToAction("Index");

		}
	}
}
