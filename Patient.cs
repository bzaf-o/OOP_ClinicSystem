using System;

public class Patient{
    private string patientID, patientName, gender;
    private int age, contactNum;
    private MedicalRecord medrecord;
    
    public Patient(string patientID, string patientName, string gender, 
    int age, int contactNum, MedicalRecord medrecord){
        this.patientID = patientID;
        this.patientName = patientName;
        this.age = age;
        this.gender = gender;
        this.contactNum = contactNum;
        this.medrecord = medrecord;
    }
    
    public void setAppointment(){
        // operations here
        Console.WriteLine("Appointment has been set.");
    }
}