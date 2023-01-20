using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;
using UnityEngine;
using UnityEngine.Networking;

public class TestDicom : MonoBehaviour
{
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
                Debug.LogError(e.Message);
                Debug.LogError("Unsupported dicom format");
                return null;
            }
        }
    }
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        UnityWebRequest www = new UnityWebRequest("https://raw.githubusercontent.com/mazatsushi/FYP/master/SCE11-0353/Sample%20DICOM%20Images/CT-MONO2-16-ankle.dcm");
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.Log(www.error);
        else {
            // Or retrieve results as binary data
            MemoryStream ms = new MemoryStream(www.downloadHandler.data);
            Debug.Log("ms: " + ms.Length);
            
            var dcmfile = DicomFile.Open(ms);
            Debug.Log("dcmfile: " + dcmfile.Format);
            
            var dcmimg = new DicomImage(dcmfile.Dataset);
            Debug.Log("dcmimg: " + dcmimg.PhotometricInterpretation);
            
            var pix = dcmimg.GetPixelData();
            Debug.Log("pix: " + pix.Width + " " + pix.Height);
            
            var height = dcmimg.Height;
            var width = dcmimg.Width;

            var minValue = long.MaxValue;
            var maxValue = long.MinValue;

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

                Debug.Log("min: " + min + ", max: " + max);
                minValue = min;
                maxValue = max;
            }
            else if (pix is GrayscalePixelDataU32)
            {
                var pixeldata = (GrayscalePixelDataU32) pix;
                Debug.Log("pixeldata: " + pixeldata.Data.Length);
                Debug.Log("asbytes: " + dcmimg.RenderImage().AsBytes().Length);
                uint min = uint.MaxValue;
                uint max = uint.MinValue;
                foreach (var elem in pixeldata.Data)
                {
                    if (elem < min)
                        min = elem;
                    if (elem > max)
                        max = elem;
                }

                Debug.Log("min: " + min + ", max: " + max);

                minValue = min;
                maxValue = max;
            }
            else if (pix is GrayscalePixelDataS16)
            {
                //var pixelbytes = dicomImage.RenderImage().AsBytes();
                //short [] pixels = new short[pixelbytes.Length/2];
                //Buffer.BlockCopy(pixelbytes,0,pixels,0,pixelbytes.Length);
                var pixeldata = (GrayscalePixelDataS16) pix;

                short min = short.MaxValue;
                short max = short.MinValue;
                foreach (var elem in pixeldata.Data)
                {
                    if (elem < min)
                        min = elem;
                    if (elem > max)
                        max = elem;
                }

                //Debug.Log("min: " + min + ", max: " + max);
                minValue = min;
                maxValue = max;
            }
            else if (pix is GrayscalePixelDataU16)
            {
                var pixeldata = (GrayscalePixelDataU16) pix;
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

                //Debug.Log("min: " + min + ", max: " + max);
                minValue = min;
                maxValue = max;
            }
            else if (pix is GrayscalePixelDataU8)
            {
                var pixeldata = (GrayscalePixelDataU8) pix;
                Debug.Log("pixeldata: " + pixeldata.Data.Length);
                Debug.Log("asbytes: " + dcmimg.RenderImage().AsBytes().Length);
                byte min = byte.MaxValue;
                byte max = byte.MinValue;
                foreach (var elem in pixeldata.Data)
                {
                    if (elem < min)
                        min = elem;
                    if (elem > max)
                        max = elem;
                }

                Debug.Log("min: " + min + ", max: " + max);
                minValue = min;
                maxValue = max;
            }
            Debug.Log("MinValue: " + minValue);
            Debug.Log("MaxValue: " + maxValue);
            Debug.Log(dcmimg.PhotometricInterpretation);
            Debug.Log(pix.GetHistogram(0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
