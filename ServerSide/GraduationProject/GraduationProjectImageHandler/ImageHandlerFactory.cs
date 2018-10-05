using System;
using GraduationProjectInterfaces.ImageHandler;

namespace GraduationProjectImageHandler
{
    public static class ImageHandlerFactory
    {
        public static IImageHandler GetImageHanlderByType(string typeName)
        {
            switch (typeName)
            {
                case nameof(AnswerCoordinates.MainBlank): return new ImageHandler();
                default: throw new ArgumentNullException();
            }
        }
    }
}
