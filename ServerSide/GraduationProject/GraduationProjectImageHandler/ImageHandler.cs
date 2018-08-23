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
            return Task.Run(() => param);
        }
    }
}
