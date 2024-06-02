using NRBD_LAB_Mongo_ASP.Models;

namespace NRBD_LAB_Mongo_ASP
{
	public class MusicAlbumList
	{
		public List<MusicAlbumModel> MusicAlbums { get; set; }
		public MusicAlbumFilter Filter { get; set; }
	}
}
