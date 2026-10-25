using System.Collections.Generic;
using System;

public class MedicalRecord {
    private string recordID;
    private Patient patient; 
    private List<string> symptoms = new List<string>();
    private string diagnosis;
    private string treatmentPlan;
    private DateTime dateCreated;

    public void updateRecord(List<string> newSymptoms, string newDiagnosis) {      
    }
    public List<string> getRecordHistory()
    {
        return new List<string>();
    }
}