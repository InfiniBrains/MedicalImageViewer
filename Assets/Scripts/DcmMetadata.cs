using System;
using System.IO;
using System.Xml;
using Dicom;
using Dicom.Imaging;
//using LiteDB;
using UnityEngine; 

public class DicomInstance
{
    public int Id { get; set; } 
    public int SerieId { get; set; }

    public string path { get; set; }
    public string transferSyntaxUID { get; set; }
    public bool ContainsPixelData { get; set; }
    public string InstanceNumber { get; set; }
}

public class DicomSerie
{
    public int Id { get; set; }
    public int StudyId { get; set; }

    public string SeriesNumber { get; set; }
    public string SeriesDate { get; set; }
    public string SeriesTime { get; set; }
    public string SeriesDescription { get; set; }
    public string Modality { get; set; }
    public string BodyPartExamined { get; set; }
    public string AcquisitionNumber { get; set; }
    public string RequestedContrastAgent { get; set; }
    public string ScanningSequence { get; set; }
    public string SliceLocation { get; set; }
}

public class DicomStudy
{
    public int Id { get; set; }
    public int PatientId { get; set; }

    public string StudyID { get; set; }
    public string StudyDate { get; set; }
    public string StudyTime { get; set; }
    public string AccessionNumber { get; set; }
    public string ModalitiesInStudy { get; set; }
    public string InstitutionName { get; set; }
    public string ReferringPhysicianName { get; set; }
    public string PerformingPhysicianName { get; set; }
    public string StudyDescription { get; set; }
}

public class DicomPatient
{
    public int Id { get; set; }
    public string PatientsName { get; set; }
    public string PatientId { get; set; }
    public string PatientBirthDate { get; set; }
    public string PatientSex { get; set; }
    public string PatientAge { get; set; }
    public string PatientComments { get; set; }
}

public class DcmMetadata
{
    public int id { get; set; }
    public string PatientsName { get; set; }
    public string SeriesNumber { get; set; }
    public string StudyID { get; set; }
    public string InstanceNumber { get; set; }
    public string path { get; set; }

    public string PatientId{ get; set; }
    public string PatientsBirthDate{ get; set; }
    public string PatientsSex{ get; set; }
    public string PatientsAge{ get; set; }
    public string PatientsComments{ get; set; }
    public string StudyDate{ get; set; }
    public string StudyTime{ get; set; }
    public string AccessionNumber{ get; set; }
    public string ModalitiesInStudy{ get; set; }
    public string InstitutionName{ get; set; }
    public string ReferringPhysicianName{ get; set; }
    public string PerformingPhysicianName{ get; set; }
    public string StudyDescription{ get; set; }
    public string SeriesDate { get; set; }
    public string SeriesTime{ get; set; }
    public string SeriesDescription{ get; set; }
    public string Modality{ get; set; }
    public string BodyPartExamined{ get; set; }
    public string AcquisitionNumber{ get; set; }
    public string RequestedContrastAgent{ get; set; }
    public string ScanningSequence{ get; set; }
    public string SliceLocation { get; set; }
    public string transferSyntaxUID { get; set; }
    public bool ContainsPixelData { get; set; }

    private static T Get<T>(DicomTag dt, DicomDataset ds)
    {
        try
        {
            return ds.Get<T>(dt);
        }
        catch (Exception e)
        {
            return default(T);
        }
    }

    public static DcmMetadata LoadDcmData(byte[] data)
    {
        using (MemoryStream memstream = new MemoryStream(data))
        {
            try
            {
                var dicom = DicomFile.Open(memstream);
                var ds = dicom.Dataset;

                DcmMetadata dcm = new DcmMetadata();

                dcm.PatientsName = Get<string>(DicomTag.PatientName, ds);
                dcm.PatientId = Get<string>(DicomTag.PatientID, ds);
                dcm.PatientsBirthDate = Get<string>(DicomTag.PatientBirthDate, ds);
                dcm.PatientsSex = Get<string>(DicomTag.PatientSex, ds);
                dcm.PatientsAge = Get<string>(DicomTag.PatientAge, ds);
                dcm.PatientsComments = Get<string>(DicomTag.PatientComments, ds);
                dcm.StudyID = Get<string>(DicomTag.StudyID, ds);
                dcm.StudyDate = Get<string>(DicomTag.StudyDate, ds);
                dcm.StudyTime = Get<string>(DicomTag.StudyTime, ds);
                dcm.AccessionNumber = Get<string>(DicomTag.AccessionNumber, ds);
                dcm.ModalitiesInStudy = Get<string>(DicomTag.ModalitiesInStudy, ds);
                dcm.InstitutionName = Get<string>(DicomTag.InstitutionName, ds);
                dcm.ReferringPhysicianName = Get<string>(DicomTag.ReferringPhysicianName, ds);
                dcm.PerformingPhysicianName = Get<string>(DicomTag.PerformingPhysicianName, ds);
                dcm.StudyDescription = Get<string>(DicomTag.StudyDescription, ds);
                dcm.SeriesNumber = Get<string>(DicomTag.SeriesNumber, ds);
                dcm.SeriesDate = Get<string>(DicomTag.SeriesDate, ds);
                dcm.SeriesTime = Get<string>(DicomTag.SeriesTime, ds);
                dcm.SeriesDescription = Get<string>(DicomTag.SeriesDescription, ds);
                dcm.Modality = Get<string>(DicomTag.Modality, ds);
                dcm.BodyPartExamined = Get<string>(DicomTag.BodyPartExamined, ds);
                dcm.AcquisitionNumber = Get<string>(DicomTag.AcquisitionNumber, ds);
                dcm.RequestedContrastAgent = Get<string>(DicomTag.RequestedContrastAgent, ds);
                dcm.ScanningSequence = Get<string>(DicomTag.ScanningSequence, ds);
                dcm.SliceLocation = Get<string>(DicomTag.SliceLocation, ds);
                dcm.InstanceNumber = Get<string>(DicomTag.InstanceNumber, ds);
                dcm.transferSyntaxUID = ds.InternalTransferSyntax.UID.UID.Substring(17);
                dcm.ContainsPixelData = ds.Contains(DicomTag.PixelData);
                return dcm;

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        return null;
    }

    public static DcmMetadata LoadDcmData(string path)
    {
        using (Stream filestream = File.OpenRead(path))
        {
            try
            {
                var dicom = DicomFile.Open(filestream);
                var ds = dicom.Dataset;

                DcmMetadata dcm = new DcmMetadata();
                dcm.path = path;

                dcm.PatientsName = Get<string>(DicomTag.PatientName, ds);
                dcm.PatientId = Get<string>(DicomTag.PatientID, ds);
                dcm.PatientsBirthDate = Get<string>(DicomTag.PatientBirthDate, ds);
                dcm.PatientsSex = Get<string>(DicomTag.PatientSex, ds);
                dcm.PatientsAge = Get<string>(DicomTag.PatientAge, ds);
                dcm.PatientsComments = Get<string>(DicomTag.PatientComments, ds);
                dcm.StudyID = Get<string>(DicomTag.StudyID, ds);
                dcm.StudyDate = Get<string>(DicomTag.StudyDate, ds);
                dcm.StudyTime = Get<string>(DicomTag.StudyTime, ds);
                dcm.AccessionNumber = Get<string>(DicomTag.AccessionNumber, ds);
                dcm.ModalitiesInStudy = Get<string>(DicomTag.ModalitiesInStudy, ds);
                dcm.InstitutionName = Get<string>(DicomTag.InstitutionName, ds);
                dcm.ReferringPhysicianName = Get<string>(DicomTag.ReferringPhysicianName, ds);
                dcm.PerformingPhysicianName = Get<string>(DicomTag.PerformingPhysicianName, ds);
                dcm.StudyDescription = Get<string>(DicomTag.StudyDescription, ds);
                dcm.SeriesNumber = Get<string>(DicomTag.SeriesNumber, ds);
                dcm.SeriesDate = Get<string>(DicomTag.SeriesDate, ds);
                dcm.SeriesTime = Get<string>(DicomTag.SeriesTime, ds);
                dcm.SeriesDescription = Get<string>(DicomTag.SeriesDescription, ds);
                dcm.Modality = Get<string>(DicomTag.Modality, ds);
                dcm.BodyPartExamined = Get<string>(DicomTag.BodyPartExamined, ds);
                dcm.AcquisitionNumber = Get<string>(DicomTag.AcquisitionNumber, ds);
                dcm.RequestedContrastAgent = Get<string>(DicomTag.RequestedContrastAgent, ds);
                dcm.ScanningSequence = Get<string>(DicomTag.ScanningSequence, ds);
                dcm.SliceLocation = Get<string>(DicomTag.SliceLocation, ds);
                dcm.InstanceNumber = Get<string>(DicomTag.InstanceNumber, ds);
                dcm.transferSyntaxUID = ds.InternalTransferSyntax.UID.UID.Substring(17);
                dcm.ContainsPixelData = ds.Contains(DicomTag.PixelData);
                return dcm;

            }
            catch (Exception e)
            {
                Debug.Log(e.Message + " " + path);
            }
        }
        return null;

    }

}
