namespace PetStuff.Web.Services
{
    public interface IFileUploadService
    {
        Task<List<string>> UploadProductImagesAsync(IFormFileCollection files);
        Task<bool> DeleteProductImageAsync(string imageUrl);
    }
}
