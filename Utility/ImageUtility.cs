using MessagePack.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System.IO;
using TestApp.Data.Migrations;

namespace TestApp.Utility
{
    public class ImageUtility
    {
        byte[] image;
        public ImageUtility()
        {

        }
        public async Task loadImage(IFormFile input)
        {
            byte[] imageresult = { };
            using (var memoryStream = new MemoryStream())
            {
                await input.CopyToAsync(memoryStream);
                imageresult = memoryStream.ToArray();
            }
            image = imageresult;
        }

        public byte[] getThumbnail()
        {
            byte[] imageresult = { };
            Image modified_image = Image.Load<Rgba32>(image);
            modified_image.Mutate(x => x.Resize(150, 100));
            PngEncoder pngEncoder = new PngEncoder();
            using (var ms = new MemoryStream())
            {
                modified_image.Save(ms, pngEncoder);
                imageresult = ms.ToArray();
            }
            return imageresult;
        }
        public byte[] getImage()
        {
            return image;

        }


    }
}
