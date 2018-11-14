using System.Collections.Generic;

namespace GraduationProjectImageHandler
{
    //Container coordinates of answers.
    public static class AnswerCoordinates
    {
        //Container of types to generate blanks.
        public static class MainBlank
        {
            public const string Questions = nameof(Questions);
            public const int QShift = 50;
            public const int QyStep = 55;
            public const int QWidth = 500;
            public static readonly KeyValuePair<int, int> QStartPoint = new KeyValuePair<int, int>(100, 275);

            public static readonly KeyValuePair<int, int> StartPoint = new KeyValuePair<int, int>(565, 285);
            public static readonly KeyValuePair<int, int> EndPoint = new KeyValuePair<int, int>(670, 310);
            public const int XStep = 142;
            public const int YStep = 55;
            public const int MaxQuestionCount = 17;
        }
    }

    public static class Sides
    {
        public const int Top = 1;
        public const int Bottom = 2;
        public const int Left = 3;
        public const int Right = 4;
    }
}
