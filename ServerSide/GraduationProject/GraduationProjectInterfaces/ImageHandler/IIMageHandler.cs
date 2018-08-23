using GraduationProjectModels;
using System.Threading.Tasks;

namespace GraduationProjectInterfaces.ImageHandler
{
    // Processing of images.
    public interface IImageHandler
    {
        Task<BlankFile> GenerateExcel(BlankFile param);
    }
}
