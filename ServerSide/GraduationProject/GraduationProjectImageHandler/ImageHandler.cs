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
            param.Id = 0;
            param.Name = "hello_world.txt";
            param.Data = "SGVsbG8gV29ybGQh";
            param.Type = "test";
            param.FileType = "file/txt";
            return Task.Run(() => param);
        }
    }
}
