using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraduationProjectInterfaces.ImageHandler;

namespace GraduationProjectImageHandler
{
    public static class ImageHandlerFactory
    {
        public static IImageHandler GetImageHanlderByType(string typeName)
        {
            return (IImageHandler)Activator.CreateInstance(Assembly.GetExecutingAssembly().DefinedTypes
                .FirstOrDefault(x => x.Name.Equals(typeName + nameof(ImageHandler), StringComparison.OrdinalIgnoreCase)))
                ?? throw new ArgumentNullException();
        }

        public static IEnumerable<string> GetTypes()
        {
            return Assembly.GetExecutingAssembly().DefinedTypes
                .FirstOrDefault(x => x.Name.Equals(nameof(AnswerCoordinates)))
                ?.DeclaredNestedTypes.Select(x => x.Name)
                ?? throw new ArgumentNullException();
        }
    }
}
