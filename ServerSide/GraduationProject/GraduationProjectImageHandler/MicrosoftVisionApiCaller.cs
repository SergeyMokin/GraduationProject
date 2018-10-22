using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraduationProjectModels;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace GraduationProjectImageHandler
{
    public class MicrosoftVisionApiCaller
    {
        public static async Task<IEnumerable<Line>> ExtractLocalTextAsync(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new Exception();
            }
            
            var computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(AppSettings.MicrosoftVisionApiKey))
            {
                Endpoint = "https://westeurope.api.cognitive.microsoft.com"
            };

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
        private static async Task<IEnumerable<Line>> GetTextAsync(ComputerVisionClient computerVision, string operationLocation, int count = 0)
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
                return count <= 25
                    ? await GetTextAsync(computerVision, operationLocation, ++count)
                    : throw new InvalidOperationException();
            }
        }
    }
}
