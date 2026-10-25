using System;

public class Appointment
{
    private string appointmentId;
    private Doctor assignedDoctor;
    private DateTime scheduleTime;
    private string status;

    public Appointment(string appointmentId, Doctor assignedDoctor, DateTime scheduleTime, string status)
    {
        this.appointmentId = appointmentId;
        this.assignedDoctor = assignedDoctor;
        this.scheduleTime = scheduleTime;
        this.status = status;
    }

    public Appointment() { }

    public void ConfirmAppointment()
    {
        this.status = "Confirmed";
        Console.WriteLine($"Appointment {this.appointmentId} has been confirmed.");
    }

    public string AppointmentId
    {
        get { return appointmentId; }
        set { appointmentId = value; }
    }

    public Doctor AssignedDoctor
    {
        get { return assignedDoctor; }
        set { assignedDoctor = value; }
    }

    public DateTime ScheduleTime
    {
        get { return scheduleTime; }
        set { scheduleTime = value; }
    }

    public string Status
    {
        get { return status; }
        set { status = value; }
    }
}
