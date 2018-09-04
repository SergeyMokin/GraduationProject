using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProjectImageHandler
{
    // Process images class.
    public class ImageHandler : IImageHandler
    {
        public Task<BlankFile> GenerateExcel(BlankFile param)
        {
            param.Type = "example";
            param.FileType = "file/txt";
            param.Data = "SGVsbG8gV29ybGQh";
            return Task.Run(() => param);
        }
    }
}
