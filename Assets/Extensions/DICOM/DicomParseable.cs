// Copyright (c) 2012-2017 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).


namespace Dicom
{
    using System.Linq;
    using System.Reflection;

    public abstract class DicomParseable
    {
        public static T Parse<T>(string value)
        {
            if (!typeof(T).IsSubclassOf(typeof(DicomParseable)))
            {
                throw new DicomDataException("DicomParseable.Parse expects a class derived from DicomParseable");
            }

            var method = typeof(T).GetDeclaredMethods("Parse").Single(m => m.IsPublic && m.IsStatic);
            return (T)method.Invoke(null, new object[] { value });
        }
    }
}
