using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
	public interface IPhotoService
	{
		Task<ImageUploadResult> AddPhotoAsync(IFormFile file, string param);
		Task<DeletionResult> DeletePhotoAsync(string publicId);
	}
}