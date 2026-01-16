namespace OnlineShop_MobileApp.ViewModel.Common
{
    public static class ResourcesLoader
    {
        public static ImageSource LoadMauiImage(string fileName) => ImageSource.FromFile(fileName);
        public static async Task<ImageSource?> LoadImageFromPackageAsync(string relativePath)
        {
            try
            {
                await using var stream = await FileSystem.OpenAppPackageFileAsync(relativePath);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var bytes = ms.ToArray();

                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
