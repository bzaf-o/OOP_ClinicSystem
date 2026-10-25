using System.Collections.Generic;
public class ClinicSystem
{
    private string clinicName;
    private string clinicAdress; 
    private List<Doctor> doctors;
    private List<Patient> patients;
    private List<Appointment> appointments;
    private List<Inventory> inventory;

    public ClinicSystem(string clinicName, string clinicAdress)
    {
        this.clinicName = clinicName;
        this.clinicAdress = clinicAdress;
        this.doctors = new List<Doctor>();
        this.patients = new List<Patient>();
        this.appointments = new List<Appointment>();
        this.inventory = new List<Inventory>();
    }

    public void registerPatient() {
    }

    public void addDoctor() {
    }

    public void scheduleAppointments() {
    }

    public void cancelAppointments() {
    }

    public List<Patient> searchPatients() {
        return new List<Patient>();
    }

    public List<Doctor> searchDoctor() {
        return new List<Doctor>();
    }

    public void generateReport() {
    }

    public void searchInventory() {
    }
}