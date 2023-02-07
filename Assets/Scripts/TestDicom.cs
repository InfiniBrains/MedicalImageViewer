using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;
using Jint;
using Jint.Native;
using UnityEngine;
using UnityEngine.Networking;

public class TestDicom : MonoBehaviour
{
    // [SerializeField] string filePath = "/DicomFiles/CT-MONO2-16-brain.dcm";
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
        TextAsset textAsset = Resources.Load("CT-MONO2-16-brain") as TextAsset;
        Stream stream = new MemoryStream(textAsset.bytes);

        var dcmfile = DicomFile.Open(stream);

        Debug.Log("dcmfile: " + dcmfile.Format);
             
        var dcmimg = new DicomImage(dcmfile.Dataset);
        Debug.Log("dcmimg: " + dcmimg.PhotometricInterpretation);
             
        var pix = dcmimg.GetPixelData();
        Debug.Log("pix: " + pix.Width + " " + pix.Height);

        ReportLoaderImage loader = new ReportLoaderImage();

        loader.ExtractDataFromDicomImage(dcmimg);

        yield return null;
        /*UnityWebRequest dcmFileRequest = new UnityWebRequest("https://raw.githubusercontent.com/mazatsushi/FYP/master/SCE11-0353/Sample%20DICOM%20Images/CT-MONO2-16-ankle.dcm");
        dcmFileRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return dcmFileRequest.SendWebRequest();
        
        byte[] dcmBytes;
        if (dcmFileRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(dcmFileRequest.error);
            yield break;
        }
        else
             dcmBytes = dcmFileRequest.downloadHandler.data;
        Debug.Log("dcm downloaded");
        
        

        UnityWebRequest gdcmJsRequest = UnityWebRequest.Get("https://raw.githubusercontent.com/InfiniBrains/gdcm-js/master/gdcmconv.js");
        gdcmJsRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return gdcmJsRequest.SendWebRequest();
        
        string gdcmScriptStr;
        if (gdcmJsRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(gdcmJsRequest.error);
            yield break;
        }
        else
             gdcmScriptStr = gdcmJsRequest.downloadHandler.text;
        Debug.Log("gdcm downloaded");
        yield return null;

        var opts = new Options();
        // opts.Modules.RegisterRequire = true;
        // opts.Debugger.Enabled = true;
        var engine = new Engine().SetValue("log", new Action<object>(Debug.Log));

        yield return null;
        engine.Execute(gdcmScriptStr);
        Debug.Log("gdcm-js added");

        engine.SetValue("dicomBytes", dcmBytes);

        yield return null;
        
        engine.Execute("var dicomoriginal = new Uint8Array(dicomBytes);");
        engine.Execute("var memfs = [{name: \"input.dcm\", data: dicomoriginal}];");
        engine.Execute("var args = [\"-i\", \"input.dcm\", \"-o\", \"output.dcm\", \"-w\"]");
        engine.Execute("var result = gdcmFunc({MEMFS:memfs,arguments:args});");
        // yield return null;
        // engine.Execute("log(result);");
        
        engine.Execute("log(\"input size: \" + dicomoriginal.length + \", output size: \" + result.MEMFS[0].data.length);");
        var result = engine.GetValue("result.MEMFS[0].data");
        Debug.Log(result);*/
        

        /*
         *
var fs = require("fs");
var gdcm = require("gdcm-js");
// read dicom bytes
var dicomoriginal = new Uint8Array(fs.readFileSync('data/deflated.dcm')); 
// create a memfs with the file
var memfs = [{name: "input.dcm", data: dicomoriginal}]
// write down the params you have to write into terminal
var args = ["-i", "input.dcm", "-o", "output.dcm", "-w"]
// run the app
var result = gdcm.gdcmconv({MEMFS:memfs,arguments:args});
// get results from result
// output files will be stored into MEMFS variable
console.log("input size: " + dicomoriginal.length + ", output size: " + result.MEMFS[0].data.length);
         */
        // engine.SetValue("dicomoriginal", dcmBytes);


//             UnityWebRequest gdcmJsRequest = UnityWebRequest.Get("https://raw.githubusercontent.com/InfiniBrains/gdcm-js/master/gdcmconv.js");
//             gdcmJsRequest.downloadHandler = new DownloadHandlerBuffer();
//             yield return gdcmJsRequest.SendWebRequest();
//
//             if (gdcmJsRequest.result != UnityWebRequest.Result.Success)
//                 Debug.Log(gdcmJsRequest.error);
//             else
//             {
//                 var scr =gdcmJsRequest.downloadHandler.text;
//                 var engine = new Engine().SetValue("log", new Action<object>(Console.WriteLine));
//                 engine.Execute(@"
//     function hello() { 
//         log('Hello World');
//     };
//  
//     hello();
// ");
//                 
//                 JsValue jsValue = JsValue.FromObject(engine,fileBytes);
//
//
//                 engine.Execute(scr);
//             }
//             
//             
//             
//             MemoryStream ms = new MemoryStream(fileBytes);
//             Debug.Log("ms: " + ms.Length);
//             
//             var dcmfile = DicomFile.Open(ms);
//             Debug.Log("dcmfile: " + dcmfile.Format);
//             
//             var dcmimg = new DicomImage(dcmfile.Dataset);
//             Debug.Log("dcmimg: " + dcmimg.PhotometricInterpretation);
//             
//             var pix = dcmimg.GetPixelData();
//             Debug.Log("pix: " + pix.Width + " " + pix.Height);
//             
//             var height = dcmimg.Height;
//             var width = dcmimg.Width;
//
//             var minValue = long.MaxValue;
//             var maxValue = long.MinValue;
//
//             if (pix is GrayscalePixelDataS32)
//             {
//                 var pixeldata = (GrayscalePixelDataS32) pix;
//                 int min = int.MaxValue;
//                 int max = int.MinValue;
//                 foreach (var elem in pixeldata.Data)
//                 {
//                     if (elem < min)
//                         min = elem;
//                     if (elem > max)
//                         max = elem;
//                 }
//
//                 Debug.Log("min: " + min + ", max: " + max);
//                 minValue = min;
//                 maxValue = max;
//             }
//             else if (pix is GrayscalePixelDataU32)
//             {
//                 var pixeldata = (GrayscalePixelDataU32) pix;
//                 Debug.Log("pixeldata: " + pixeldata.Data.Length);
//                 Debug.Log("asbytes: " + dcmimg.RenderImage().AsBytes().Length);
//                 uint min = uint.MaxValue;
//                 uint max = uint.MinValue;
//                 foreach (var elem in pixeldata.Data)
//                 {
//                     if (elem < min)
//                         min = elem;
//                     if (elem > max)
//                         max = elem;
//                 }
//
//                 Debug.Log("min: " + min + ", max: " + max);
//
//                 minValue = min;
//                 maxValue = max;
//             }
//             else if (pix is GrayscalePixelDataS16)
//             {
//                 //var pixelbytes = dicomImage.RenderImage().AsBytes();
//                 //short [] pixels = new short[pixelbytes.Length/2];
//                 //Buffer.BlockCopy(pixelbytes,0,pixels,0,pixelbytes.Length);
//                 var pixeldata = (GrayscalePixelDataS16) pix;
//
//                 short min = short.MaxValue;
//                 short max = short.MinValue;
//                 foreach (var elem in pixeldata.Data)
//                 {
//                     if (elem < min)
//                         min = elem;
//                     if (elem > max)
//                         max = elem;
//                 }
//
//                 //Debug.Log("min: " + min + ", max: " + max);
//                 minValue = min;
//                 maxValue = max;
//             }
//             else if (pix is GrayscalePixelDataU16)
//             {
//                 var pixeldata = (GrayscalePixelDataU16) pix;
//                 //Debug.Log("pixeldata: " + pixeldata.Data.Length);
//                 //Debug.Log("asbytes: " + dicomImage.RenderImage().AsBytes().Length);
//                 ushort min = ushort.MaxValue;
//                 ushort max = ushort.MinValue;
//                 foreach (var elem in pixeldata.Data)
//                 {
//                     if (elem < min)
//                         min = elem;
//                     if (elem > max)
//                         max = elem;
//                 }
//
//                 //Debug.Log("min: " + min + ", max: " + max);
//                 minValue = min;
//                 maxValue = max;
//             }
//             else if (pix is GrayscalePixelDataU8)
//             {
//                 var pixeldata = (GrayscalePixelDataU8) pix;
//                 Debug.Log("pixeldata: " + pixeldata.Data.Length);
//                 Debug.Log("asbytes: " + dcmimg.RenderImage().AsBytes().Length);
//                 byte min = byte.MaxValue;
//                 byte max = byte.MinValue;
//                 foreach (var elem in pixeldata.Data)
//                 {
//                     if (elem < min)
//                         min = elem;
//                     if (elem > max)
//                         max = elem;
//                 }
//
//                 Debug.Log("min: " + min + ", max: " + max);
//                 minValue = min;
//                 maxValue = max;
//             }
//             Debug.Log("MinValue: " + minValue);
//             Debug.Log("MaxValue: " + maxValue);
//             Debug.Log(dcmimg.PhotometricInterpretation);
//             Debug.Log(pix.GetHistogram(0));
//         }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
