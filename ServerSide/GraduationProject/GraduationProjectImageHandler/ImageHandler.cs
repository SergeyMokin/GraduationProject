using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace GraduationProjectImageHandler
{
    // Process images class.
    public class ImageHandler : IImageHandler
    {
        private Tuple<string, string> _answerVariants = new Tuple<string, string>("YES", "NO");
        private Dictionary<int, string> _answers = new Dictionary<int, string>();

        private string _savedImageName;
        private string _savedBlackWhiteImageName;

        private string _excelFileName;
        private string _excelFilePath;

        private BlankFile _blankFile;

        public Task<BlankFile> GenerateExcel(BlankFile param)
        {
            var pathToSave = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds.ToString(CultureInfo.CurrentCulture));

            _savedImageName = pathToSave + param.Name;
            _savedBlackWhiteImageName = pathToSave + param.Name + "_bw";

            _excelFileName = param.Name.Replace(".bmp", "").Replace(".jpg", "").Replace(".png", "") + ".xlsx";
            _excelFilePath = pathToSave + _excelFileName;

            _blankFile = param;

            SearchAnswers();

            return Task.Run(() => CreateExcel());
        }

        private BlankFile CreateExcel()
        {
            using (var fs = new FileStream(_excelFilePath, FileMode.Create, FileAccess.Write))
            {

                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet = workbook.CreateSheet("ImageHandlerResult");

                for (var i = 0; i < 7; i++)
                {
                    IRow row = sheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(Questions.Values[i + 1]);
                    row.CreateCell(1).SetCellValue(_answers[i + 1]);
                }

                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);

                workbook.Write(fs);
            }

            var data = Convert.ToBase64String(File.ReadAllBytes(_excelFilePath));

            File.Delete(_excelFilePath);

            return new BlankFile
            {
                Id = 0,
                Name = _excelFileName,
                Data = data,
                Type = "GraduationBlank",
                FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        private void SearchAnswers()
        {
            SaveImage();
            SaveGrayScaleImage();

            using (var img = new Bitmap(_savedBlackWhiteImageName))
            {
                var i = 1;
                foreach (var c in AnswerCoordinates.Coordinates)
                {
                    var blackPixelLeftCount = SearchCountOfBlackPixelsByCoordinates(img, c.Value[0], c.Value[1], c.Value[2], c.Value[3]);
                    var blackPixelRightCount = SearchCountOfBlackPixelsByCoordinates(img, c.Value[4], c.Value[5], c.Value[6], c.Value[7]);

                    _answers.Add(i++, blackPixelLeftCount > blackPixelRightCount ? _answerVariants.Item1 : _answerVariants.Item2);
                }
            }

            File.Delete(_savedImageName);
            File.Delete(_savedBlackWhiteImageName);
        }

        private int SearchCountOfBlackPixelsByCoordinates(Bitmap img, int startX, int startY, int endX, int endY)
        {
            var blackPixelCount = 0;
            for (var x = startX; x < endX; x++)
            {
                for (var y = startY; y < endY; y++)
                {
                    var color = img.GetPixel(x, y);
                    if ((color.R + color.G + color.B) < 5)
                    {
                        blackPixelCount++;
                    }
                }
            }
            return blackPixelCount;
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
                using (var resImg = ResizeBitmap(image, new Size { Width = 1080, Height = 1397 }))
                {
                    using (var bw = resImg.Clone(new Rectangle(0, 0, resImg.Width, resImg.Height), PixelFormat.Format1bppIndexed))
                    {
                        bw.Save(_savedBlackWhiteImageName);
                    }
                }
            }
        }

        private Bitmap ResizeBitmap(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }
    }
}
