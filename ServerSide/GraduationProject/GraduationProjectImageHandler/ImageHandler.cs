using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraduationProjectModels;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

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

        public override async Task<IEnumerable<string>> GetQuestionsFromBlank(TypeFile typeFile)
        {
            var pathToSave = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                DateTime.Now.Subtract(DateTime.MinValue).TotalSeconds.ToString(CultureInfo.CurrentCulture));

            var savedImageName = pathToSave + "_qadd_img";

            SaveImage(typeFile.Data, savedImageName);

            var questionList = new List<string>();

            var computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials("86716aff0e6544df851dd470f2e8b2ce"));

            computerVision.Endpoint = "https://westcentralus.api.cognitive.microsoft.com";

            var text = (await ExtractLocalTextAsync(computerVision, savedImageName)).ToArray();

            var currentY = AnswerCoordinates.MainBlank.QStartPoint.Value;
            while (currentY + AnswerCoordinates.MainBlank.QyStep < BlankFileSettings.BlankHeight)
            {
                var contains = text.Where(x =>
                        x.BoundingBox[0] >= AnswerCoordinates.MainBlank.QStartPoint.Key
                        && x.BoundingBox[1] >= currentY
                        && x.BoundingBox[4] <=
                        AnswerCoordinates.MainBlank.QStartPoint.Key + AnswerCoordinates.MainBlank.QWidth
                        && x.BoundingBox[5] <= currentY + AnswerCoordinates.MainBlank.QHeight + 25).Select(x => x.Text)
                    .ToArray();

                var result = contains.Any()
                    ? string.Join(" ", contains)
                    : null;

                if (!string.IsNullOrWhiteSpace(result))
                {
                    questionList.Add(result);
                }

                currentY += AnswerCoordinates.MainBlank.QyStep;
            }

            File.Delete(savedImageName);

            return questionList.AsEnumerable();
        }

        private static async Task<IEnumerable<Line>> ExtractLocalTextAsync(ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new Exception();
            }

            RecognizeTextInStreamHeaders textHeaders = null;
            while (textHeaders == null)
            {
                try
                {
                    using (Stream imageStream = File.Open(imagePath, FileMode.OpenOrCreate))
                    {
                        textHeaders = await computerVision.RecognizeTextInStreamAsync(imageStream, TextRecognitionMode.Handwritten);
                    }
                }
                catch
                {
                    Thread.Sleep(5000);
                }
            }

            return await GetTextAsync(computerVision, textHeaders.OperationLocation);
        }

        // Retrieve the recognized text
        private static async Task<IEnumerable<Line>> GetTextAsync(ComputerVisionClient computerVision, string operationLocation)
        {
            if (computerVision == null) throw new ArgumentNullException(nameof(computerVision));

            try
            {
                const int numberOfCharsInOperationId = 36;

                var operationId = operationLocation.Substring(
                    operationLocation.Length - numberOfCharsInOperationId);

                var result =
                    await computerVision.GetTextOperationResultAsync(operationId);

                // Wait for the operation to complete
                var i = 0;
                const int maxRetries = 10;
                while ((result.Status == TextOperationStatusCodes.Running ||
                        result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
                {
                    result = await computerVision.GetTextOperationResultAsync(operationId);
                }

                return result.RecognitionResult.Lines;
            }
            catch
            {
                Thread.Sleep(5000);
                return await GetTextAsync(computerVision, operationLocation);
            }
        }
    }
}
