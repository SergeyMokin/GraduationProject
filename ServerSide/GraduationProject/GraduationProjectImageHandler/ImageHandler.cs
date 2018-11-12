using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectModels;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace GraduationProjectImageHandler
{
    public abstract class ImageHandler: IImageHandler
    {
        protected readonly Tuple<string, string, string> BaseAnswerVariants = new Tuple<string, string, string>("YES", "MAY BE", "NO");
        protected readonly Dictionary<int, string> Answers = new Dictionary<int, string>();
        protected string[] Questions;
        protected string SavedBlackWhiteImageName;

        private string _savedImageName;

        private string _excelFileName;
        private string _excelFilePath;
        private string _recognizedBlankType;

        private BlankFile _blankFile;

        /// <summary>
        ///     Override this method to search answers on blank.
        /// </summary>
        protected abstract void SearchAnswers();

        /// <summary>
        ///     Override to get questions from blank.
        /// </summary>
        /// <param name="typeFile"></param>
        /// <returns></returns>
        public abstract Task<IEnumerable<string>> GetQuestionsFromBlank(TypeFile typeFile);

        /// <summary>
        ///     Search count of white pixels on rectangle.
        /// </summary>
        /// <param name="img">Img to search</param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns>Count of white pixels</returns>
        protected static int SearchCountOfWhitePixelsByCoordinates(Bitmap img, int startX, int startY, int endX, int endY)
        {
            const int threshold = 5;
            var whitePixelCount = 0;
            try
            {
                for (var x = startX; x < endX; x++)
                {
                    for (var y = startY; y < endY; y++)
                    {
                        var color = img.GetPixel(x, y);
                        if ((color.R + color.G + color.B) > threshold)
                        {
                            whitePixelCount++;
                        }
                    }
                }
            }
            catch
            {
                img.Dispose();
                throw;
            }
            return whitePixelCount;
        }

        /// <summary>
        /// Search sides what contains borders.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns>Number of side</returns>
        protected static int[] CheckPositions(Bitmap img, int startX, int startY, int endX, int endY)
        {
            const int threshold = 5;
            var byteArr = new byte[endX - startX][];
            var lst = new List<int>();
            var countTop = 0;
            var countBottom = 0;
            var countLeft = 0;
            var countRight = 0;

            try
            {
                for (var x = startX; x < endX; x++)
                {
                    byteArr[x - startX] = new byte[endY - startY];
                    for (var y = startY; y < endY; y++)
                    {
                        var pixel = img.GetPixel(x, y);
                        if ((pixel.R + pixel.G + pixel.B) / 3 > threshold)
                        {
                            byteArr[x - startX][y - startY] = 255;
                        }
                        else
                        {
                            byteArr[x - startX][y - startY] = 0;
                        }
                    }
                }

                for (var x = 0; x < byteArr.Length; x++)
                {
                    if (byteArr[x][0] == 255 || byteArr[x][1] == 255 || byteArr[x][2] == 255)
                    {
                        countTop++;
                    }
                    if (byteArr[x][byteArr.Length - 3] == 255 || byteArr[x][byteArr.Length - 2] == 255 || byteArr[x][byteArr.Length - 1] == 255)
                    {
                        countBottom++;
                    }
                }

                for (var y = 0; y < byteArr[0].Length; y++)
                {
                    if (byteArr[0][y] == 255 || byteArr[1][y] == 255 || byteArr[2][y] == 255)
                    {
                        countLeft++;
                    }
                    if (byteArr[byteArr.Length - 1][y] == 255 || byteArr[byteArr.Length - 2][y] == 255 || byteArr[byteArr.Length - 3][y] == 255)
                    {
                        countRight++;
                    }
                }

                if (countTop >= (endX - startX - 10))
                {
                    lst.Add(Sides.Top);
                }

                if (countBottom >= (endX - startX - 10))
                {
                    lst.Add(Sides.Bottom);
                }

                if (countLeft >= (endY - startY - 10))
                {
                    lst.Add(Sides.Left);
                }

                if (countRight >= (endY - startY - 10))
                {
                    lst.Add(Sides.Right);
                }

                return lst.Distinct().ToArray();
            }
            catch
            {
                img.Dispose();
                throw;
            }

            
        }

        /// <summary>
        /// Change start points by ref.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="badPositions"></param>
        /// <param name="shift"></param>
        protected void ChangeStartPoints(ref int startX, ref int startY, ref int endX, ref int endY, int[] badPositions, int shift)
        {
            if (!badPositions.Any())
            {
                return;
            }
            
            if (badPositions.Contains(Sides.Top))
            {
                startY += shift;
                endY += shift;
            }
            if (badPositions.Contains(Sides.Bottom))
            {
                startY -= shift;
                endY -= shift;
            }
            if (badPositions.Contains(Sides.Left))
            {
                startX += shift;
                endX += shift;
            }
            if (badPositions.Contains(Sides.Right))
            {
                startX -= shift;
                endX -= shift;
            }
        }

        public Task<BlankFile> GenerateExcel(BlankFile param, IEnumerable<string> questions)
        {
            try
            {
                Questions = questions?.ToArray();

                var pathToSave = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds.ToString(CultureInfo.CurrentCulture));

                _savedImageName = pathToSave + param.Name;
                SavedBlackWhiteImageName = pathToSave + param.Name + "_bw";

                _excelFileName = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss_") + param.Name.Replace(".bmp", "").Replace(".jpg", "").Replace(".png", "") + ".xlsx";
                _excelFilePath = pathToSave + _excelFileName;
                _recognizedBlankType = param.Type;

                _blankFile = param;

                SaveImage(_blankFile.Data, _savedImageName);
                SaveGrayScaleImage();

                SearchAnswers();

                var result = Task.FromResult(CreateExcel());

                return result;
            }
            finally
            {
                RemoveFiles();

                GC.Collect();
            }
            
        }
        
        private BlankFile CreateExcel()
        {
            try
            {
                using (var fs = new FileStream(_excelFilePath, FileMode.Create, FileAccess.Write))
                {

                    var workbook = (IWorkbook) new XSSFWorkbook();

                    var sheet = workbook.CreateSheet("ImageHandlerResult");

                    for (var i = 0; i < Questions.Length; i++)
                    {
                        var row = sheet.CreateRow(i);
                        row.CreateCell(0).SetCellValue(Questions[i]);
                        row.CreateCell(1).SetCellValue(Answers[i]);
                    }

                    sheet.AutoSizeColumn(0);
                    sheet.AutoSizeColumn(1);

                    workbook.Write(fs);
                }

                var data = Convert.ToBase64String(File.ReadAllBytes(_excelFilePath));

                return new BlankFile
                {
                    Id = 0,
                    Name = _excelFileName,
                    Data = data,
                    Type = _recognizedBlankType,
                    FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
            }
            catch
            {
                throw new InvalidOperationException();
            }
        }

        protected void SaveImage(string data, string path)
        {
            var bytes = Convert.FromBase64String(data);
            using (var stream = new FileStream(path + "_middle", FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }

            using (var img = new Bitmap(path + "_middle"))
                using (var resizedImg = ResizeBitmap(img, new Size { Width = BlankFileSettings.BlankWidth, Height = BlankFileSettings.BlankHeight }))
                    resizedImg.Save(path);

            File.Delete(path + "_middle");

        }

        private void SaveGrayScaleImage()
        {
            using (var img = new Bitmap(_savedImageName))
                using (var kirschImg = ConvolutionFilter(img, Kirsch3X3Horizontal, Kirsch3X3Vertical))
                    using (var bw = kirschImg.Clone(new Rectangle(0, 0, kirschImg.Width, kirschImg.Height), PixelFormat.Format1bppIndexed))
                        bw.Save(SavedBlackWhiteImageName);
        }

        private void RemoveFiles()
        {
            if(File.Exists(_savedImageName))
                File.Delete(_savedImageName);

            if (File.Exists(SavedBlackWhiteImageName))
                File.Delete(SavedBlackWhiteImageName);

            if (File.Exists(_excelFilePath))
                File.Delete(_excelFilePath);
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
            var sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
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

            for (var offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
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
