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
    public class MainBlankImageHandler : ImageHandler
    {
        protected override void SearchAnswers()
        {
            const int minQuestionCount = 1;
            if (Questions.Length < minQuestionCount
                || Questions.Length > AnswerCoordinates.MainBlank.MaxQuestionCount)
            {
                throw new InvalidOperationException();
            }

            using (var img = new Bitmap(SavedBlackWhiteImageName))
            {
                const int shift = 20;
                var startX = AnswerCoordinates.MainBlank.StartPoint.Key;
                var startY = AnswerCoordinates.MainBlank.StartPoint.Value;
                var endX = AnswerCoordinates.MainBlank.EndPoint.Key;
                var endY = AnswerCoordinates.MainBlank.EndPoint.Value;
                for (var i = 0; i < Questions.Length; i++)
                {
                    var badPositions = CheckPositions(img, startX, startY, endX, endY);

                    ChangeStartPoints(ref startX, ref startY, ref endX, ref endY, badPositions, shift);

                    var whitePixelYesCount = SearchCountOfWhitePixelsByCoordinates(img, startX, startY, endX, endY);

                    var whitePixelMaybeCount = SearchCountOfWhitePixelsByCoordinates(img, startX + AnswerCoordinates.MainBlank.XStep, startY, endX + AnswerCoordinates.MainBlank.XStep, endY);

                    var whitePixelNoCount = SearchCountOfWhitePixelsByCoordinates(img, startX + 2 * AnswerCoordinates.MainBlank.XStep, startY, endX + 2 * AnswerCoordinates.MainBlank.XStep, endY);

                    var answer = whitePixelYesCount > whitePixelMaybeCount && whitePixelYesCount > whitePixelNoCount ?
                        BaseAnswerVariants.Item1
                        : whitePixelMaybeCount > whitePixelYesCount && whitePixelMaybeCount > whitePixelNoCount ?
                            BaseAnswerVariants.Item2
                            : BaseAnswerVariants.Item3;

                    startY += AnswerCoordinates.MainBlank.YStep;
                    endY += AnswerCoordinates.MainBlank.YStep;

                    startY = startY % 2 == 0 ? startY + 1 : startY;
                    endY = endY % 2 == 0 ? endY + 1 : endY;

                    Answers.Add(i, answer);
                }
            }
        }

        public override async Task<IEnumerable<string>> GetQuestionsFromBlank(TypeFile typeFile)
        {
            const int shift = 5;

            var pathToSave = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds.ToString(CultureInfo.CurrentCulture));

            var savedImageName = pathToSave + "_qadd_img";

            SaveImage(typeFile.Data, savedImageName);

            var questionList = new List<string>();

            var text = (await MicrosoftVisionApiCaller.ExtractLocalTextAsync(savedImageName)).ToArray();
            
            var currentY = AnswerCoordinates.MainBlank.QStartPoint.Value;

            if (text.Any(x => x.Text.Contains(AnswerCoordinates.MainBlank.Questions)))
            {
                currentY = text.First(x => x.Text.Contains(AnswerCoordinates.MainBlank.Questions)).BoundingBox[1] + AnswerCoordinates.MainBlank.QShift;
            }

            var counter = 0;
            while (currentY + AnswerCoordinates.MainBlank.QyStep < BlankFileSettings.BlankHeight &&
                   counter++ < AnswerCoordinates.MainBlank.MaxQuestionCount)
            {
                if (text.Any(x =>
                    x.BoundingBox[0] > AnswerCoordinates.MainBlank.QStartPoint.Key
                    && x.BoundingBox[1] >= currentY
                    && x.BoundingBox[4] < AnswerCoordinates.MainBlank.QStartPoint.Key +
                    AnswerCoordinates.MainBlank.QWidth
                    && x.BoundingBox[1] < currentY + AnswerCoordinates.MainBlank.QyStep))
                {
                    currentY = text.First(x =>
                            x.BoundingBox[0] > AnswerCoordinates.MainBlank.QStartPoint.Key
                            && x.BoundingBox[1] >= currentY
                            && x.BoundingBox[4] < AnswerCoordinates.MainBlank.QStartPoint.Key +
                            AnswerCoordinates.MainBlank.QWidth
                            && x.BoundingBox[1] < currentY + AnswerCoordinates.MainBlank.QyStep)
                        .BoundingBox[1] - shift;
                }

                var contains = text.Where(x =>
                        x.BoundingBox[0] > AnswerCoordinates.MainBlank.QStartPoint.Key
                        && x.BoundingBox[1] >= currentY
                        && x.BoundingBox[4] < AnswerCoordinates.MainBlank.QStartPoint.Key +
                        AnswerCoordinates.MainBlank.QWidth
                        && x.BoundingBox[1] < currentY + AnswerCoordinates.MainBlank.QyStep).Select(x => x.Text)
                    .ToArray();

                var result = contains.Any()
                    ? string.Join(" ", contains)
                    : null;

                if (!(string.IsNullOrWhiteSpace(result) || result.All(char.IsDigit)))
                {
                    questionList.Add(result);
                }

                currentY += counter > AnswerCoordinates.MainBlank.MaxQuestionCount / 2 ? AnswerCoordinates.MainBlank.QyStep + 2 : AnswerCoordinates.MainBlank.QyStep;
            }

            File.Delete(savedImageName);
            
            GC.Collect();

            return questionList.AsEnumerable();
        }

       
    }
}
