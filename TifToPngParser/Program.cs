using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

var inputPath = @"Input/";
var outputPath = @"Output/";

foreach (var file in Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories))
{
    var item = file.Replace('\\', '/');
    var innerPath = String.Join("/", item.Split('/').Skip(1)).Split('.').First().Replace(@"\", "/");
    var filename = item.Split('/').Last().Split('.').First().Replace(@"\", "/");
    var extension = item.Split('.').Last();

    if (innerPath.Contains('/'))
    {
        new DirectoryInfo(Path.Combine(outputPath, innerPath.Replace(filename, ""))).Create();
    }
    switch (extension)
    {
        case "tif":
        case "tiff":
            using (var tiff = new Bitmap(item))
            {
                tiff.Save(Path.Combine(outputPath, innerPath + ".png"), ImageFormat.Png);
            }
            break;
        case "mtl":
            var text = File.ReadAllText(item);
            text = text.Replace(".tiff", ".png");
            text = text.Replace(".tif", ".png");
            File.WriteAllText(Path.Combine(outputPath, innerPath + ".mtl"), text);
            break;
        default:
            File.Copy(item, Path.Combine(outputPath, innerPath + "." + extension), true);
            break;
    }
}