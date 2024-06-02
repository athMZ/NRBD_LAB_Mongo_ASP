using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using NRBD_LAB_Mongo_ASP.Models;

namespace NRBD_LAB_Mongo_ASP
{
	public class MusicRepository : IMusicRepository
	{

		private readonly IMongoCollection<MusicAlbumModel> _musicCollection;
		private readonly IGridFSBucket _gridFs;

		public MusicRepository(IOptions<DatabaseSettings> dbSettings)
		{
			var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
			var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);

			_musicCollection = mongoDatabase.GetCollection<MusicAlbumModel>(dbSettings.Value.CollectionName);
			_gridFs = new GridFSBucket(mongoDatabase);
		}

		public async Task<IEnumerable<MusicAlbumModel>> GetAlbums(int? year, string? title)
		{
			var builder = new FilterDefinitionBuilder<MusicAlbumModel>();
			var filter = builder.Empty;

			if(year.HasValue)
				filter &= builder.Eq(c => c.Year, year.Value);

			if(!string.IsNullOrWhiteSpace(title))
				filter &= builder.Regex(c => c.Title, new BsonRegularExpression(title));

			return await _musicCollection.Find(filter).ToListAsync();
		}

		public async Task<MusicAlbumModel> GetById(string id) => await
					_musicCollection.Find(c => c._id == id).FirstOrDefaultAsync();

		public async Task Add(MusicAlbumModel musicAlbum) => await 
					_musicCollection.InsertOneAsync(musicAlbum);

		public async Task Update(MusicAlbumModel musicAlbum) => await
				_musicCollection.ReplaceOneAsync(c => c._id == musicAlbum._id, musicAlbum);

		public async Task Delete(string id) => await
			_musicCollection.DeleteOneAsync(c => c._id == id);

		public async Task<IEnumerable<MusicAlbumModel>> GetAll() => await
		_musicCollection.Find(c => true).ToListAsync();

		public async Task AddImage(string id, Stream stream)
		{
			var album = await GetById(id);
			if (album == null)
				throw new Exception("Album not found");

			if (album.Image != null)
				await _gridFs.DeleteAsync(new ObjectId(album.Image));

			var imageId = ObjectId.GenerateNewId(); //Generate new id for image
			await _gridFs.UploadFromStreamAsync(imageId.ToString(), stream); //Save image to GridFS

			var update = Builders<MusicAlbumModel>.Update.Set(c => c.Image, imageId.ToString());
			await _musicCollection.UpdateOneAsync(c => c._id == id, update);
		}

		public async Task<byte[]> GetImage(string id) =>  _gridFs.DownloadAsBytes(new ObjectId(id));



	}
}
