using MongoDB.Bson.Serialization.Attributes;

namespace NRBD_LAB_Mongo_ASP.Models;

public class MusicAlbumModel
{
	[BsonId]
	[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
	public string? _id { get; set; }
	public required string Title { get; set; }
	public required int Year { get; set; }
	public string? Image { get; set; }
}