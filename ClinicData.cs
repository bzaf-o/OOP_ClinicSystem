using System;
using System.Collections.Generic;

namespace ClinicSystem
{
    // Central shared data so every form stays synchronized
    public static class ClinicData
    {
        // ---- Doctors (from DoctorDraft.cs) ----
        public static List<Doctor> Doctors = new List<Doctor>();

        // ---- Patients (from patient.cs / clinic system.cs) ----
        public static List<Patient> Patients = new List<Patient>();

        // ---- Appointments (from appointment.cs) ----
        public static List<Appointment> Appointments = new List<Appointment>();

        // ---- Invoices (from Invoice UML) ----
        public static List<Invoice> Invoices = new List<Invoice>();

        // ---- Inventory (from inventory.cs) ----
        public static List<InventoryItem> Inventory = new List<InventoryItem>();

        // ---- Medical Records (from medicalrecord.cs) ----
        public static Dictionary<string, List<MedicalRecordEntry>> MedicalRecords =
            new Dictionary<string, List<MedicalRecordEntry>>(StringComparer.OrdinalIgnoreCase);

        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            // Dummy doctors
            Doctors.Add(new Doctor("D001", "Rashid", "Family", 8, 17, 10000));
            Doctors.Add(new Doctor("D002", "Hana", "Cardiology", 9, 18, 20000));
            Doctors.Add(new Doctor("D003", "Sarah", "Neurology", 7, 16, 40000));

            // Dummy patients
            Patients.Add(new Patient("P001", "Nur Aina Sofa", 21, "Female", "0123456789"));
            Patients.Add(new Patient("P002", "Ahmad Hakim", 34, "Male", "0121112222"));
            Patients.Add(new Patient("P003", "Siti Mariam", 8, "Female", "0133334444"));
            Patients.Add(new Patient("P004", "Lim Wei Jie", 45, "Male", "0145556666"));

            // Dummy inventory
            Inventory.Add(new InventoryItem("M001", "Paracetamol", "Tablet", 150, 20));
            Inventory.Add(new InventoryItem("M002", "Cough Syrup", "Syrup", 50, 10));
            Inventory.Add(new InventoryItem("M003", "Vitamin C", "Tablet", 80, 15));
            Inventory.Add(new InventoryItem("M004", "Amoxicillin 250mg", "Capsule", 13, 20));

            // Dummy invoice
            Invoices.Add(new Invoice("INV001", Patients[0], 30.00,
                new[] { "Paracetamol x1" }, new[] { 8.50 }, "Cash", true));
        }
    }

    // ---- Patient model ----
    public class Patient
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Contact { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }

        public Patient(string id, string name, int age, string gender, string contact)
        {
            PatientId = id; Name = name; Age = age; Gender = gender; Contact = contact;
        }

        public override string ToString() => $"{PatientId} - {Name}";
    }

    // ---- MedicalRecordEntry ----
    public class MedicalRecordEntry
    {
        public string RecordId { get; set; }
        public string PatientName { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}