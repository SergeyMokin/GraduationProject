using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Runtime.InteropServices;

namespace GraduationProjectImageHandler
{
    // Process images class.
    public class ImageHandler : IImageHandler
    {
        private readonly Tuple<string, string> _answerVariants = new Tuple<string, string>("YES", "NO");
        private readonly Dictionary<int, string> _answers = new Dictionary<int, string>();
        private string[] _questions;

        private string _savedImageName;
        private string _savedBlackWhiteImageName;

        private string _excelFileName;
        private string _excelFilePath;
        private string _recognizedBlankType;

        private BlankFile _blankFile;

        public Task<BlankFile> GenerateExcel(BlankFile param, IEnumerable<string> questions)
        {
            _questions = questions?.ToArray();

            if (_questions?.Length != AppSettings.QuestionsCount)
            {
                throw new NotImplementedException();
            }

            var pathToSave = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds.ToString(CultureInfo.CurrentCulture));

            _savedImageName = pathToSave + param.Name;
            _savedBlackWhiteImageName = pathToSave + param.Name + "_bw";

            _excelFileName =  DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss_") + param.Name.Replace(".bmp", "").Replace(".jpg", "").Replace(".png", "") + ".xlsx";
            _excelFilePath = pathToSave + _excelFileName;
            _recognizedBlankType = param.Type;

            _blankFile = param;

            SearchAnswers();

            return Task.FromResult(CreateExcel());
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
                    row.CreateCell(0).SetCellValue(_questions[i]);
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
                Type = _recognizedBlankType,
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
                    var blackPixelLeftCount = SearchCountOfWhitePixelsByCoordinates(img, c.Value[0], c.Value[1], c.Value[2], c.Value[3]);
                    var blackPixelRightCount = SearchCountOfWhitePixelsByCoordinates(img, c.Value[4], c.Value[5], c.Value[6], c.Value[7]);

                    _answers.Add(i++, blackPixelLeftCount > blackPixelRightCount ? _answerVariants.Item1 : _answerVariants.Item2);
                }
            }

            File.Delete(_savedImageName);
            File.Delete(_savedBlackWhiteImageName);
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
            using (var img = new Bitmap(_savedImageName))
                using (var resizedImg = ResizeBitmap(img, new Size { Width = 1080, Height = 1397 }))
                    using (var kirschImg = ConvolutionFilter(resizedImg, Kirsch3X3Horizontal, Kirsch3X3Vertical))
                        using (var bw = kirschImg.Clone(new Rectangle(0, 0, kirschImg.Width, kirschImg.Height), PixelFormat.Format1bppIndexed))
                            bw.Save(_savedBlackWhiteImageName);
        }

        private static int SearchCountOfWhitePixelsByCoordinates(Bitmap img, int startX, int startY, int endX, int endY)
        {
            var whitePixelCount = 0;
            for (var x = startX; x < endX; x++)
            {
                for (var y = startY; y < endY; y++)
                {
                    var color = img.GetPixel(x, y);
                    if ((color.R + color.G + color.B) > 5)
                    {
                        whitePixelCount++;
                    }
                }
            }
            return whitePixelCount;
        }

        private static Bitmap ResizeBitmap(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }

        private static double[,] Kirsch3X3Horizontal => new double[,]
        {
            {  5,  5,  5 },
            { -3,  0, -3 },
            { -3, -3, -3 },
        };

        private static double[,] Kirsch3X3Vertical => new double[,]
        {
            {  5, -3, -3, },
            {  5,  0, -3, },
            {  5, -3, -3, },
        };

        private static Bitmap ConvolutionFilter(Bitmap sourceBitmap, double[,] xFilterMatrix, double[,] yFilterMatrix, bool grayscale = true)
        {
            var sourceData =
                sourceBitmap.LockBits(new Rectangle(0, 0,
                        sourceBitmap.Width, sourceBitmap.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

            var pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            var resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            if (grayscale)
            {
                for (var k = 0; k < pixelBuffer.Length; k += 4)
                {
                    var rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            const int filterOffset = 1;

            for (var offsetY = filterOffset;offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (var offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    double greenX;
                    double redX;
                    var blueX = greenX = redX = 0;
                    double greenY;
                    double redY;
                    var blueY = greenY = redY = 0;
                    
                    var byteOffset = offsetY * sourceData.Stride + offsetX * 4;
                    
                    for (var filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (var filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            var calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);
                            
                            blueX += pixelBuffer[calcOffset] * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            
                            greenX += pixelBuffer[calcOffset + 1] * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            
                            redX += (pixelBuffer[calcOffset + 2]) * xFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            
                            blueY += (pixelBuffer[calcOffset]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            
                            greenY += (pixelBuffer[calcOffset + 1]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                            
                            redY += (pixelBuffer[calcOffset + 2]) * yFilterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }
                    
                    var blueTotal = Math.Sqrt((blueX * blueX) + (blueY * blueY));
                    
                    var greenTotal = Math.Sqrt((greenX * greenX) + (greenY * greenY));
                    
                    var redTotal = Math.Sqrt((redX * redX) + (redY * redY));

                    blueTotal = blueTotal > 255 ? 255 : blueTotal < 0 ? 0 : blueTotal;

                    greenTotal = greenTotal > 255 ? 255 : greenTotal < 0 ? 0 : greenTotal;

                    redTotal = redTotal > 255 ? 255 : redTotal < 0 ? 0 : redTotal;
                    
                    resultBuffer[byteOffset] = (byte)(blueTotal);
                    resultBuffer[byteOffset + 1] = (byte)(greenTotal);
                    resultBuffer[byteOffset + 2] = (byte)(redTotal);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }
            
            var resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            
            var resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            
            return resultBitmap;
        }
    }
}
