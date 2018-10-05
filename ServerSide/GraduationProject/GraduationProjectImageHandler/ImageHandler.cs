using System;
using System.Collections.Generic;
using System.Drawing;
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

                    Answers.Add(i, answer);
                }
            }
        }

        public override Task<IEnumerable<string>> GetQuestionsFromBlank(TypeFile typeFile) => throw new Exception();
    }
}
