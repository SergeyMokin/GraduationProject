using System.Collections.Generic;

namespace GraduationProjectImageHandler
{
    //Container coordinates of answers.
    public class AnswerCoordinates
    {
        //Container of types to generate blanks.
        public static class MainBlank
        {
            public const int QyStep = 55;
            public const int QWidth = 500;
            public const int QHeight = 65;
            public static readonly KeyValuePair<int, int> QStartPoint = new KeyValuePair<int, int>(100, 275);

            public static readonly KeyValuePair<int, int> StartPoint = new KeyValuePair<int, int>(565, 285);
            public static readonly KeyValuePair<int, int> EndPoint = new KeyValuePair<int, int>(670, 310);
            public const int XStep = 142;
            public const int YStep = 55;
            public const int MinQuestionCount = 1;
            public const int MaxQuestionCount = 17;
        }
    }
}
