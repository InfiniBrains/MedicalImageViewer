using System;
using System.Collections.Generic;
using System.IO;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;
//using ICSharpCode.SharpZipLib.Core;
//using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;
using Object = UnityEngine.Object;

public class ReportLoaderImage
{
    public int width, height;
    public byte[] rgbdata;
    public IPixelData dicomImage;
    public bool isDicomUnsupported = false;
    public DcmMetadata Metadata;
    public double minValue;
    public double maxValue;

    //public ReportLoaderImage(string path)
    //{
    //    InfiniThread.Run(() =>
    //    {
    //        lock (InfiniWWW._lock)
    //        {
    //            LoadFromBytes(File.ReadAllBytes(path));
    //        }
    //    });
    //}

    public ReportLoaderImage()
    {

    }

    public ReportLoaderImage(byte[] fileBytes)
    {
        LoadFromBytes(fileBytes);
    }

    //public static void LoadAsync(byte[] bytes, Action<ReportLoaderImage> onFinishLoading)
    //{
    //    InfiniThread.Run(() =>
    //    {
    //        var img = new ReportLoaderImage(bytes);
    //        InfiniThread.RunOnUnityThread(() => { onFinishLoading(img); });
    //    });
    //}

    //public static void LoadAsync(string path, Action<ReportLoaderImage> onFinishLoading)
    //{
    //    InfiniThread.Run(() =>
    //    {
    //        var img = new ReportLoaderImage(File.ReadAllBytes(path));
    //        InfiniThread.RunOnUnityThread(() => { onFinishLoading(img); });
    //    });
    //}

    private void LoadFromBytes(byte[] bytes)
    {
        //var unzippedBytes = unzipFirstFileFromArchive(bytes);

        // check if bytes wer zipped or not
        //if (unzippedBytes != null)
            //ExtractDataFromUnzippedBytes(unzippedBytes);
        // if it isnt a zip
        //else
            ExtractDataFromUnzippedBytes(bytes);
    }

    private void ExtractDataFromUnzippedBytes(byte[] bytes)
    {
        // if the zip contains a dicom
        var dicomObject = FileBytesToDicomImage(bytes);
        
        if (dicomObject != null)
        {
            ExtractDataFromDicomImage(dicomObject);
        }
        // if the zip does not contains a dicom
        //else
            //InfiniThread.RunOnUnityThread(() => { ExtractDataFromPngJpgImage(bytes); });
    }

    public void ExtractDataFromDicomImage(DicomImage dicomImage)
    {
        var pix = dicomImage.GetPixelData();
        
        this.dicomImage = pix;
        this.height = dicomImage.Height;
        this.width = dicomImage.Width;

        if (pix is GrayscalePixelDataS32)
        {
            var pixeldata = (GrayscalePixelDataS32) pix;
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (var elem in pixeldata.Data)
            {
                if (elem < min)
                    min = elem;
                if (elem > max)
                    max = elem;
            }
            Debug.Log("GrayscalePixelDataS32 min: " + min + ", max: " + max);
            this.minValue = min;
            this.maxValue = max;
        } 
        else if (pix is GrayscalePixelDataU32)
        {
            var pixeldata = (GrayscalePixelDataU32)pix;
            Debug.Log("pixeldata: " + pixeldata.Data.Length);
            Debug.Log("asbytes: " + dicomImage.RenderImage().AsBytes().Length);
            uint min = uint.MaxValue;
            uint max = uint.MinValue;
            foreach (var elem in pixeldata.Data)
            {
                if (elem < min)
                    min = elem;
                if (elem > max)
                    max = elem;
            }
            Debug.Log("GrayscalePixelDataU32 min: " + min + ", max: " + max);

            minValue = min;
            maxValue = max;
        }
        else if (pix is GrayscalePixelDataS16)
        {
            //var pixelbytes = dicomImage.RenderImage().AsBytes();
            //short [] pixels = new short[pixelbytes.Length/2];
            //Buffer.BlockCopy(pixelbytes,0,pixels,0,pixelbytes.Length);
            var pixeldata = (GrayscalePixelDataS16)pix;
            
            short min = short.MaxValue;
            short max = short.MinValue;
            foreach (var elem in pixeldata.Data)
            {
                if (elem < min)
                    min = elem;
                if (elem > max)
                    max = elem;
            }
            Debug.Log("GrayscalePixelDataS16 min: " + min + ", max: " + max);
            minValue = min;
            maxValue = max;
        }
        else if (pix is GrayscalePixelDataU16)
        {
            var pixeldata = (GrayscalePixelDataU16)pix;
            //Debug.Log("pixeldata: " + pixeldata.Data.Length);
            //Debug.Log("asbytes: " + dicomImage.RenderImage().AsBytes().Length);
            ushort min = ushort.MaxValue;
            ushort max = ushort.MinValue;
            foreach (var elem in pixeldata.Data)
            {
                if (elem < min)
                    min = elem;
                if (elem > max)
                    max = elem;
            }
            Debug.Log("GrayscalePixelDataU16 min: " + min + ", max: " + max);
            minValue = min;
            maxValue = max;
        }
        else if (pix is GrayscalePixelDataU8)
        {
            var pixeldata = (GrayscalePixelDataU8)pix;
            Debug.Log("pixeldata: " + pixeldata.Data.Length);
            Debug.Log("asbytes: " + dicomImage.RenderImage().AsBytes().Length);
            byte min = byte.MaxValue;
            byte max = byte.MinValue;
            foreach (var elem in pixeldata.Data)
            {
                if (elem < min)
                    min = elem;
                if (elem > max)
                    max = elem;
            }
            Debug.Log("GrayscalePixelDataU8 min: " + min + ", max: " + max);
            minValue = min;
            maxValue = max;
        }
    }

    private void ExtractDataFromPngJpgImage(byte[] bytes)
    {
        var tex = new Texture2D(1, 1, (TextureFormat) 14, false);
        tex.LoadImage(bytes);
        tex.Apply();

        height = tex.height;
        width = tex.width;
        rgbdata = tex.GetRawTextureData();
        Object.Destroy(tex);
    }

    private DicomImage FileBytesToDicomImage(byte[] bytes)
    {
        using (MemoryStream ms = new MemoryStream(bytes))
        {
            DicomFile file = null;
            try
            {
                file = DicomFile.Open(ms);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
            DicomImage image = null;
            
            try
            {
                image = new DicomImage(file.Dataset);
                return image;
            }
            catch (Exception e)
            {
                isDicomUnsupported = true;
                //Debug.LogError(e.Message);
                return null;
            }
	    }
    }

	/*byte[] unzipFirstFileFromArchive(byte[] bytes)
	{
		try
		{
			using (MemoryStream ms = new MemoryStream(bytes))
			{
				ZipFile zf = new ZipFile(ms);

				foreach (ZipEntry zipEntry in zf)
				{
					if (!zipEntry.IsFile)
						continue;

					using (Stream zipStream = zf.GetInputStream(zipEntry))
					{
						using (MemoryStream memStream = new MemoryStream())
						{
							StreamUtils.Copy(zipStream, memStream, new byte[4096]);
							memStream.Position = 0;
							return memStream.ToArray();
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			Debug.Log("not a zip: " +e.Message);
			return null;
		}

		return null;
	}*/
}

public static class ReportLoaderExtensions
{
	/*
    private static void loadImageWindow(Texture2D tex, object [child, float windowPosition, float windowWidth)
    {
        double? maxValue = child[0].GetType().GetField("MaxValue").GetValue(child[0]) as double?;
        double? minValue = child[0].GetType().GetField("MinValue").GetValue(child[0]) as double?;

        Debug.Log(maxValue + ", " + minValue);
    } */
    
    static byte minimum(byte x, byte y)
    {
        if (x < y)
            return x;
        else return y;
    }

    static sbyte minimum(sbyte x, sbyte y)
    {
        if (x < y)
            return x;
        else return y;
    }

    static short minimum(short x, short y)
    {
        if (x < y)
            return x;
        else return y;
    }

    static ushort minimum(ushort x, ushort y)
    {
        if (x < y)
            return x;
        else return y;
    }

    static int minimum(int x, int y)
    {
        if (x < y)
            return x;
        else return y;
    }

    static uint minimum(uint x, uint y)
    {
        if (x < y)
            return x;
        else return y;
    }

    static byte minimum(byte x, byte y, byte z)
    {
        return minimum(minimum(x, y), z);
    }

    static sbyte minimum(sbyte x, sbyte y, sbyte z)
    {
        return minimum(minimum(x, y), z);
    }

    static short minimum(short x, short y, short z)
    {
        return minimum(minimum(x, y), z);
    }

    static ushort minimum(ushort x, ushort y, ushort z)
    {
        return minimum(minimum(x, y), z);
    }

    static int minimum(int x, int y, int z)
    {
        return minimum(minimum(x, y), z);
    }

    static uint minimum(uint x, uint y, uint z)
    {
        return minimum(minimum(x, y), z);
    }

    static byte maximum(byte x, byte y)
    {
        if (x > y)
            return x;
        else return y;
    }

    static sbyte maximum(sbyte x, sbyte y)
    {
        if (x > y)
            return x;
        else return y;
    }

    static short maximum(short x, short y)
    {
        if (x > y)
            return x;
        else return y;
    }

    static ushort maximum(ushort x, ushort y)
    {
        if (x > y)
            return x;
        else return y;
    }

    static int maximum(int x, int y)
    {
        if (x > y)
            return x;
        else return y;
    }

    static uint maximum(uint x, uint y)
    {
        if (x > y)
            return x;
        else return y;
    }

    static byte maximum(byte x, byte y, byte z)
    {
        return maximum(maximum(x, y), z);
    }

    static sbyte maximum(sbyte x, sbyte y, sbyte z)
    {
        return maximum(maximum(x, y), z);
    }

    static short maximum(short x, short y, short z)
    {
        return maximum(maximum(x, y), z);
    }

    static ushort maximum(ushort x, ushort y, ushort z)
    {
        return maximum(maximum(x, y), z);
    }

    static int maximum(int x, int y, int z)
    {
        return maximum(maximum(x, y), z);
    }

    static uint maximum(uint x, uint y, uint z)
    {
        return maximum(maximum(x, y), z);
    }
    

    static float hue2rgb(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1 / 2.0) return q;
        if (t < 2 / 3.0) return p + (q - p) * (2 / 3.0f - t) * 6;
        return p;
    }

    static byte [] hslToRgb(float h, float s, float l)
    {
        byte[] ret = new byte[3]; 

        h = h - Mathf.Floor(h);

        if (s == 0)
            ret[0] = ret[1] = ret[2] = (byte)(255*l); 
        else
        {
            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;
            ret[0] = (byte)(hue2rgb(p, q, h + 1 / 3.0f)*255);
            ret[1] = (byte)(hue2rgb(p, q, h) * 255);
            ret[2] = (byte)(hue2rgb(p, q, h - 1 / 3.0f) * 255);
        }

        return ret;
    }

    public static void LoadImageWindow(this Texture2D tex, ReportLoaderImage img, float windowPosition,
        float windowWidth, ref double min, ref double max)
    {
        if (img.width != 0 && img.height != 0 && img.height != tex.height && img.width != tex.width)
            tex.Reinitialize(img.width, img.height);
        if (img.minValue < min)
            min = img.minValue;
        if (img.maxValue > max)
            max = img.maxValue;

        if (img.rgbdata != null)
        {
            byte minInput = byte.MaxValue, maxInput = byte.MinValue;
            foreach (byte t in img.rgbdata)
            {
                if (minInput > t)
                    minInput = t;
                if (maxInput < t)
                    maxInput = t;
            }

            // TODO: Simplify this
            var deltaInput = maxInput - minInput;
            var centerInput = windowPosition * deltaInput + minInput;
            var minOutput = centerInput - windowWidth * deltaInput / 2;
            var maxOutput = centerInput + windowWidth * deltaInput / 2;
            var deltaOutput = maxOutput - minOutput;
            var centerOutput = (maxOutput + minOutput) / 2;

            // f(x) = a(x) + b;
            var a = byte.MaxValue / deltaOutput;
            var b = (byte.MaxValue / 2f) - (byte.MaxValue * centerOutput) / deltaOutput;

            var arr = new byte[img.width * img.height * 4];
            for (int i = 0; i < arr.Length; i += 4)
            {
                var red = img.rgbdata[i] * a + b;
                if (red > byte.MaxValue)
                    red = byte.MaxValue;
                else if (red < byte.MinValue)
                    red = byte.MinValue;

                var green = img.rgbdata[i + 1] * a + b;
                if (green > byte.MaxValue)
                    green = byte.MaxValue;
                else if (green < byte.MinValue)
                    green = byte.MinValue;

                var blue = img.rgbdata[i + 2] * a + b;
                if (blue > byte.MaxValue)
                    blue = byte.MaxValue;
                else if (blue < byte.MinValue)
                    blue = byte.MinValue;

                arr[i] = (byte)red;
                arr[i + 1] = (byte)green;
                arr[i + 2] = (byte)blue;
                arr[i + 3] = 255;
            }

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataU8)
        {
            var data = (GrayscalePixelDataU8)img.dicomImage;

            var gmin = byte.MaxValue;
            var gmax = byte.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];

            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    value *= 255;
                    arr[i] = (byte)value;
                    arr[i + 1] = arr[i];
                    arr[i + 2] = arr[i];
                    arr[i + 3] = 255;
                }
            }

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataS16)
        {
            var data = (GrayscalePixelDataS16)img.dicomImage;

            var gmin = short.MaxValue;
            var gmax = short.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];

            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    value *= 255;
                    arr[i] = (byte)value;
                    arr[i + 1] = arr[i];
                    arr[i + 2] = arr[i];
                    arr[i + 3] = 255;
                }
            }

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataU16)
        {
            var data = (GrayscalePixelDataU16)img.dicomImage;

            var gmin = ushort.MaxValue;
            var gmax = ushort.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];

            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    value *= 255;
                    arr[i] = (byte)value;
                    arr[i + 1] = arr[i];
                    arr[i + 2] = arr[i];
                    arr[i + 3] = 255;
                }
            }

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataS32)
        {
            var data = (GrayscalePixelDataS32)img.dicomImage;

            var gmin = int.MaxValue;
            var gmax = int.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];

            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    value *= 255;
                    arr[i] = (byte)value;
                    arr[i + 1] = arr[i];
                    arr[i + 2] = arr[i];
                    arr[i + 3] = 255;
                }
            }

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataU32)
        {
            var data = (GrayscalePixelDataU32)img.dicomImage;

            var gmin = uint.MaxValue;
            var gmax = uint.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];

            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    value *= 255;
                    arr[i] = (byte)value;
                    arr[i + 1] = arr[i];
                    arr[i + 2] = arr[i];
                    arr[i + 3] = 255;
                }
            }

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
    }

    private static byte[] IPixelDataToIntArray(IPixelData pix)
    {
        var lenght = pix.Height*pix.Width*4;
        var ret = new byte[lenght];
        //Debug.Log(pix.GetType());
        if (pix is GrayscalePixelDataS32)
        {
            var vet = (GrayscalePixelDataS32) pix;
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (var elem in vet.Data)
            {
                if (elem > max)
                    max = elem;
                if (elem < min)
                    min = elem;
            }

            for (int y = pix.Height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < pix.Width; x++, i += 4)
                {
                    int id = y * pix.Height + x;
                    var elem = vet.Data[id] - min;
                    ret[i] = (byte)((elem >> 24) & 255);
                    ret[i + 1] = (byte)((elem >> 16) & 255);
                    ret[i + 2] = (byte)((elem >> 8) & 255);
                    ret[i + 3] = (byte)(elem & 255);
                }
            }
            return ret;
        }
        else if (pix is GrayscalePixelDataU32)
        {
            var vet = (GrayscalePixelDataU32)pix;
            uint min = uint.MaxValue;
            uint max = uint.MinValue;
            foreach (var elem in vet.Data)
            {
                if (elem > max)
                    max = elem;
                if (elem < min)
                    min = elem;
            }

            for (int y = pix.Height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < pix.Width; x++, i += 4)
                {
                    int id = y * pix.Height + x;
                    var elem = vet.Data[id] - min;
                    ret[i] = (byte)((elem >> 24) & 255);
                    ret[i + 1] = (byte)((elem >> 16) & 255);
                    ret[i + 2] = (byte)((elem >> 8) & 255);
                    ret[i + 3] = (byte)(elem & 255);
                }
            }
            return ret;
        }
        else if (pix is GrayscalePixelDataS16)
        {
            var vet = (GrayscalePixelDataS16)pix;

            short min = short.MaxValue;
            short max = short.MinValue;
            foreach (var elem in vet.Data)
            {
                if (elem > max)
                    max = elem;
                if (elem < min)
                    min = elem;
            }

            for (int y = pix.Height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < pix.Width; x++, i += 4)
                {
                    int id = y * pix.Height + x;
                    var elem = vet.Data[id] - min;
                    ret[i] = (byte) ((elem >> 24) & 255);
                    ret[i + 1] = (byte) ((elem >> 16) & 255);
                    ret[i + 2] = (byte) ((elem >> 8) & 255);
                    ret[i + 3] = (byte) (elem & 255);
                }
            }
            return ret;
        }
        else if (pix is GrayscalePixelDataU16)
        {
            var vet = (GrayscalePixelDataU16)pix;

            ushort min = ushort.MaxValue;
            ushort max = ushort.MinValue;
            foreach (var elem in vet.Data)
            {
                if (elem > max)
                    max = elem;
                if (elem < min)
                    min = elem;
            }

            for (int y = pix.Height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < pix.Width; x++, i += 4)
                {
                    int id = y * pix.Height + x;
                    var elem = vet.Data[id] - min;
                    ret[i] = (byte)((elem >> 24) & 255);
                    ret[i + 1] = (byte)((elem >> 16) & 255);
                    ret[i + 2] = (byte)((elem >> 8) & 255);
                    ret[i + 3] = (byte)(elem & 255);
                }
            }
            return ret;
        }
        else if (pix is GrayscalePixelDataU8)
        {
            var vet = (GrayscalePixelDataU8)pix;
            byte min = byte.MaxValue;
            byte max = byte.MinValue;
            foreach (var elem in vet.Data)
            {
                if (elem > max)
                    max = elem;
                if (elem < min)
                    min = elem;
            }

            for (int y = pix.Height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < pix.Width; x++, i += 4)
                {
                    int id = y * pix.Height + x;
                    var elem = vet.Data[id] - min;
                    ret[i] = (byte)((elem >> 24) & 255);
                    ret[i + 1] = (byte)((elem >> 16) & 255);
                    ret[i + 2] = (byte)((elem >> 8) & 255);
                    ret[i + 3] = (byte)(elem & 255);
                }
            }
            return ret;
        }
        return null;
    }

    public static void LoadImageWindowShader(this Texture2D tex, ReportLoaderImage img, float windowPosition,
        float windowWidth, ref double min, ref double max, Material mat)
    {
        if (img.width != 0 && img.height != 0 && img.height != tex.height && img.width != tex.width)
            tex.Reinitialize(img.width, img.height);

        if (img.minValue < min)
            min = img.minValue;
        if (img.maxValue > max)
            max = img.maxValue;

        //Debug.Log(min + "," + max);
		if (img.dicomImage != null) {
			var bytearraytex = IPixelDataToIntArray (img.dicomImage);
			if (bytearraytex != null && bytearraytex.Length > 1) {
				tex.LoadRawTextureData (bytearraytex);
				mat.SetFloat ("_Width", windowWidth);
				mat.SetFloat ("_Position", windowPosition);
				mat.SetFloat ("_MaxValue", (float)(max - min));
				mat.SetFloat ("_MinValue", (float)-min);
				tex.Apply ();
			}
			else{
				tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
				tex.Apply();
			}
		} 
    }

    public static void LoadImageWindowRainbow(this Texture2D tex, ReportLoaderImage img, float windowPosition,
        float windowWidth, ref double min, ref double max)
    {
        if (img.width != 0 && img.height != 0 && img.height != tex.height && img.width != tex.width)
            tex.Reinitialize(img.width, img.height);

        if (img.rgbdata != null)
        {
            byte minInput = byte.MaxValue, maxInput = byte.MinValue;
            foreach (byte t in img.rgbdata)
            {
                if (minInput > t)
                    minInput = t;
                if (maxInput < t)
                    maxInput = t;
            }

            // TODO: Simplify this
            var deltaInput = maxInput - minInput;
            var centerInput = windowPosition * deltaInput + minInput;
            var minOutput = centerInput - windowWidth * deltaInput / 2;
            var maxOutput = centerInput + windowWidth * deltaInput / 2;
            var deltaOutput = maxOutput - minOutput;
            var centerOutput = (maxOutput + minOutput) / 2;

            // f(x) = a(x) + b;
            var a = byte.MaxValue / deltaOutput;
            var b = (byte.MaxValue / 2f) - (byte.MaxValue * centerOutput) / deltaOutput;

            var arr = new byte[img.width * img.height * 4];
            for (int i = 0; i < arr.Length; i += 4)
            {
                var red = img.rgbdata[i] * a + b;
                if (red > byte.MaxValue)
                    red = byte.MaxValue;
                else if (red < byte.MinValue)
                    red = byte.MinValue;

                var green = img.rgbdata[i + 1] * a + b;
                if (green > byte.MaxValue)
                    green = byte.MaxValue;
                else if (green < byte.MinValue)
                    green = byte.MinValue;

                var blue = img.rgbdata[i + 2] * a + b;
                if (blue > byte.MaxValue)
                    blue = byte.MaxValue;
                else if (blue < byte.MinValue)
                    blue = byte.MinValue;

                arr[i] = (byte)red;
                arr[i + 1] = (byte)green;
                arr[i + 2] = (byte)blue;
                arr[i + 3] = 255;
            }

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataU8)
        {
            var data = (GrayscalePixelDataU8)img.dicomImage;

            var gmin = byte.MaxValue;
            var gmax = byte.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth*delta;
            var center = windowPosition*delta;
            var left = center - width/2;
            var right = center + width/2;

            var arr = new byte[img.width * img.height * 4];
            var log = Mathf.Log(delta, 2)/2;
            FastSqrt fs = new FastSqrt();
            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left)/(width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    var colorx2 = value * 2;
                    var sqrtcolorx2 = 0f;
                    if (colorx2 <= 1)
                        sqrtcolorx2 = fs.Sqrt(colorx2);
                    else
                        sqrtcolorx2 = 2 - fs.Sqrt(2 - colorx2);
                    sqrtcolorx2 /= 2;
                    var rgb = hslToRgb(sqrtcolorx2 * log, 1, value);

                    arr[  i  ] = rgb[0];
                    arr[i + 1] = rgb[1];
                    arr[i + 2] = rgb[2];
                    arr[i + 3] = 255;
                }
            }
            fs.Clear();
            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataS16)
        {
            var data = (GrayscalePixelDataS16)img.dicomImage;

            var gmin = short.MaxValue;
            var gmax = short.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];
            var log = Mathf.Log(delta, 2) / 2;
            FastSqrt fs = new FastSqrt();
            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    var colorx2 = value * 2;
                    var sqrtcolorx2 = 0f;
                    if (colorx2 <= 1)
                        sqrtcolorx2 = fs.Sqrt(colorx2);
                    else
                        sqrtcolorx2 = 2 - fs.Sqrt(2 - colorx2);
                    sqrtcolorx2 /= 2;
                    var rgb = hslToRgb(sqrtcolorx2 * log, 1, value);

                    arr[i] = rgb[0];
                    arr[i + 1] = rgb[1];
                    arr[i + 2] = rgb[2];
                    arr[i + 3] = 255;
                }
            }
            fs.Clear();
            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataU16)
        {
            var data = (GrayscalePixelDataU16)img.dicomImage;

            var gmin = ushort.MaxValue;
            var gmax = ushort.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];
            var log = Mathf.Log(delta, 2) / 2;
            FastSqrt fs = new FastSqrt();
            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    var colorx2 = value * 2;
                    var sqrtcolorx2 = 0f;
                    if (colorx2 <= 1)
                        sqrtcolorx2 = fs.Sqrt(colorx2);
                    else
                        sqrtcolorx2 = 2 - fs.Sqrt(2 - colorx2);
                    sqrtcolorx2 /= 2;
                    var rgb = hslToRgb(sqrtcolorx2 * log, 1, value);

                    arr[i] = rgb[0];
                    arr[i + 1] = rgb[1];
                    arr[i + 2] = rgb[2];
                    arr[i + 3] = 255;
                }
            }
            fs.Clear();
            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataS32)
        {
            var data = (GrayscalePixelDataS32)img.dicomImage;

            var gmin = int.MaxValue;
            var gmax = int.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];
            var log = Mathf.Log(delta, 2) / 2;
            FastSqrt fs = new FastSqrt();
            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    var colorx2 = value * 2;
                    var sqrtcolorx2 = 0f;
                    if (colorx2 <= 1)
                        sqrtcolorx2 = fs.Sqrt(colorx2);
                    else
                        sqrtcolorx2 = 2 - fs.Sqrt(2 - colorx2);
                    sqrtcolorx2 /= 2;
                    var rgb = hslToRgb(sqrtcolorx2 * log, 1, value);

                    arr[i] = rgb[0];
                    arr[i + 1] = rgb[1];
                    arr[i + 2] = rgb[2];
                    arr[i + 3] = 255;
                }
            }
            fs.Clear();

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
        else if (img.dicomImage is GrayscalePixelDataU32)
        {
            var data = (GrayscalePixelDataU32)img.dicomImage;

            var gmin = uint.MaxValue;
            var gmax = uint.MinValue;

            foreach (var t in data.Data)
            {
                if (gmin > t)
                    gmin = t;
                if (gmax < t)
                    gmax = t;
            }

            var delta = gmax - gmin;
            var width = windowWidth * delta;
            var center = windowPosition * delta;
            var left = center - width / 2;
            var right = center + width / 2;

            var arr = new byte[img.width * img.height * 4];
            var log = Mathf.Log(delta, 2) / 2;
            FastSqrt fs = new FastSqrt();
            for (int y = img.height - 1, i = 0; y >= 0; y--)
            {
                for (int x = 0; x < img.width; x++, i += 4)
                {
                    int id = y * img.height + x;
                    var value = (data.Data[id] - gmin - left) / (width);

                    if (value > 1)
                        value = 1;
                    else if (value < 0)
                        value = 0;

                    var colorx2 = value * 2;
                    var sqrtcolorx2 = 0f;
                    if (colorx2 <= 1)
                        sqrtcolorx2 = fs.Sqrt(colorx2);
                    else
                        sqrtcolorx2 = 2 - fs.Sqrt(2 - colorx2);
                    sqrtcolorx2 /= 2;
                    var rgb = hslToRgb(sqrtcolorx2 * log, 1, value);

                    arr[i] = rgb[0];
                    arr[i + 1] = rgb[1];
                    arr[i + 2] = rgb[2];
                    arr[i + 3] = 255;
                }
            }
            fs.Clear();

            tex.LoadRawTextureData(arr);
            tex.Apply();
        }
    }

}