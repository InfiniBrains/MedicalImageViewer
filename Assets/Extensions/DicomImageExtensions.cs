using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Dicom.Imaging;
using Dicom.Imaging.Render;
using UnityEngine;

public static class DicomImageExtensions
{
    public static IPixelData GetPixelData(this DicomImage dcmImage)
    {
        return typeof(DicomImage).GetField("_pixelData", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dcmImage) as IPixelData;
    }
}
