using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace GraduationProjectImageHandler
{
    // Process images class.
    public class ImageHandler : IImageHandler
    {
        private string _savedImageName;
        private string _savedBlackWhiteImageName;
        private BlankFile _blankFile;

        public Task<BlankFile> GenerateExcel(BlankFile param)
        {
            var path = Directory.GetCurrentDirectory() + "\\wwwroot\\" + DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds;
            _savedImageName = path + param.Name;
            _savedBlackWhiteImageName = path + param.Name + "_bw.jpg";
            _blankFile = param;

            SaveImage();
            SaveGrayScaleImage();

            //todo: create excel.

            File.Delete(_savedImageName);
            File.Delete(_savedBlackWhiteImageName);

            return Task.Run(() => param);
        }

        private void SaveImage()
        {
            var bytes = Convert.FromBase64String(_blankFile.Data);
            using (var stream = new FileStream(_savedImageName, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }

        private void SaveGrayScaleImage()
        {
            using (var image = new Bitmap(_savedImageName))
            {
                using (var bw = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format1bppIndexed))
                {
                    bw.Save(_savedBlackWhiteImageName);
                }
            }
        }
    }
}
