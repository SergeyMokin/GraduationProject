using GraduationProjectModels;
using System.Threading.Tasks;

namespace GraduationProjectInterfaces.ImageHandler
{
    public interface IImageHandler
    {
        Task<BlankFile> GenerateExcel(BlankFile param);
    }
}
