using NRBD_LAB_Mongo_ASP.Models;

namespace NRBD_LAB_Mongo_ASP
{
	public interface IMusicRepository
	{
		Task<IEnumerable<MusicAlbumModel>> GetAlbums(int? year, string? title);

		Task<MusicAlbumModel> GetById(string id);
		Task Add(MusicAlbumModel musicAlbum);
		Task Update(MusicAlbumModel musicAlbum);
		Task Delete(string id);
		Task<IEnumerable<MusicAlbumModel>> GetAll();
		Task AddImage(string id, Stream stream);
		Task<byte[]> GetImage(string id);
	}
}
