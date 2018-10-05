using System.Collections.Generic;

namespace GraduationProjectImageHandler
{
    //Container coordinates of answers.
    public class AnswerCoordinates
    {
        public static class MainBlank
        {
            public static readonly KeyValuePair<int, int> StartPoint = new KeyValuePair<int, int>(580, 230);
            public static readonly KeyValuePair<int, int> EndPoint = new KeyValuePair<int, int>(680, 265);
            public const int XStep = 145;
            public const int YStep = 56;
            public const int MinQuestionCount = 1;
            public const int MaxQuestionCount = 17;
        }
    }
}
