using System;
using GraduationProjectModels;
using System.IO;
using System.Drawing;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace GraduationProjectImageHandler
{
    // Process images class.
    public class ImageHandler : BaseImageHandler
    {
        protected override BlankFile CreateExcel()
        {

            using (var fs = new FileStream(ExcelFilePath, FileMode.Create, FileAccess.Write))
            {

                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet = workbook.CreateSheet("ImageHandlerResult");

                for (var i = 0; i < Questions.Length; i++)
                {
                    IRow row = sheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(Questions[i]);
                    row.CreateCell(1).SetCellValue(Answers[i]);
                }

                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);

                workbook.Write(fs);
            }

            var data = Convert.ToBase64String(File.ReadAllBytes(ExcelFilePath));

            return new BlankFile
            {
                Id = 0,
                Name = ExcelFileName,
                Data = data,
                Type = RecognizedBlankType,
                FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        protected override void SearchAnswers()
        {
            if (Questions.Length < AnswerCoordinates.MainBlank.MaxQuestionCount
                || Questions.Length > AnswerCoordinates.MainBlank.MaxQuestionCount)
            {
                throw new ArgumentException($"{RecognizedBlankType} has not valid count of questions. It need to be between {AnswerCoordinates.MainBlank.MaxQuestionCount} and {AnswerCoordinates.MainBlank.MaxQuestionCount}");
            }

            using (var img = new Bitmap(SavedBlackWhiteImageName))
            {
                var yStep = 0;
                for (var i = 0; i < Questions.Length; i++)
                {
                    var whitePixelYesCount = SearchCountOfWhitePixelsByCoordinates(img,
                        AnswerCoordinates.MainBlank.StartPoint.Key,
                        AnswerCoordinates.MainBlank.StartPoint.Value + yStep,
                        AnswerCoordinates.MainBlank.EndPoint.Key,
                        AnswerCoordinates.MainBlank.EndPoint.Value + yStep);

                    var whitePixelMaybeCount = SearchCountOfWhitePixelsByCoordinates(img,
                        AnswerCoordinates.MainBlank.StartPoint.Key + AnswerCoordinates.MainBlank.XStep,
                        AnswerCoordinates.MainBlank.StartPoint.Value + yStep,
                        AnswerCoordinates.MainBlank.EndPoint.Key + AnswerCoordinates.MainBlank.XStep,
                        AnswerCoordinates.MainBlank.EndPoint.Value + yStep);

                    var whitePixelNoCount = SearchCountOfWhitePixelsByCoordinates(img,
                        AnswerCoordinates.MainBlank.StartPoint.Key + 2 * AnswerCoordinates.MainBlank.XStep,
                        AnswerCoordinates.MainBlank.StartPoint.Value + yStep,
                        AnswerCoordinates.MainBlank.EndPoint.Key + 2 * AnswerCoordinates.MainBlank.XStep,
                        AnswerCoordinates.MainBlank.EndPoint.Value + yStep);

                    var answer = whitePixelYesCount > whitePixelMaybeCount && whitePixelYesCount > whitePixelNoCount ?
                        BaseAnswerVariants.Item1
                        : whitePixelMaybeCount > whitePixelYesCount && whitePixelMaybeCount > whitePixelNoCount ?
                            BaseAnswerVariants.Item2
                            : BaseAnswerVariants.Item3;

                    yStep += AnswerCoordinates.MainBlank.YStep;

                    Answers.Add(i, answer);
                }
            }
        }
    }
}
