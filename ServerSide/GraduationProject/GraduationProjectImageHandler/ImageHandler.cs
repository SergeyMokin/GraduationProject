using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraduationProjectModels;

namespace GraduationProjectImageHandler
{
    // Process images class.
    public class ImageHandler : BaseImageHandler
    {
        protected override void SearchAnswers()
        {
            if (Questions.Length < AnswerCoordinates.MainBlank.MaxQuestionCount
                || Questions.Length > AnswerCoordinates.MainBlank.MaxQuestionCount)
            {
                throw new InvalidOperationException();
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

                    yStep = yStep % 2 == 0 ? yStep + 1 : yStep;

                    Answers.Add(i, answer);
                }
            }
        }

        public override async Task<IEnumerable<string>> GetQuestionsFromBlank(TypeFile typeFile)
        {
            var pathToSave = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds.ToString(CultureInfo.CurrentCulture));

            var savedImageName = pathToSave + "_qadd_img";

            SaveImage(typeFile.Data, savedImageName);

            var questionList = new List<string>();

            var text = (await MicrosoftVisionApiCaller.ExtractLocalTextAsync(savedImageName)).ToArray();
            
            var currentY = AnswerCoordinates.MainBlank.QStartPoint.Value;
            while (currentY + AnswerCoordinates.MainBlank.QyStep < BlankFileSettings.BlankHeight)
            {
                var contains = text.Where(x =>
                        x.BoundingBox[0] > AnswerCoordinates.MainBlank.QStartPoint.Key
                        && x.BoundingBox[1] >= currentY
                        && x.BoundingBox[4] < AnswerCoordinates.MainBlank.QStartPoint.Key + AnswerCoordinates.MainBlank.QWidth
                        && x.BoundingBox[5] < currentY + AnswerCoordinates.MainBlank.QHeight).Select(x => x.Text)
                    .ToArray();

                var result = contains.Any()
                    ? string.Join(" ", contains)
                    : null;

                if (!(string.IsNullOrWhiteSpace(result) || result.All(char.IsDigit)))
                {
                    questionList.Add(result);
                }

                currentY += AnswerCoordinates.MainBlank.QyStep;
            }

            File.Delete(savedImageName);

            return questionList.AsEnumerable();
        }

       
    }
}
