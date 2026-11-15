using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicSystem
{
    // Appointment model (matches appointment.cs UML, with Patient link added)
    public class Appointment
    {
        public string AppointmentId { get; set; }
        public Doctor AssignedDoctor { get; set; }
        public Patient Patient { get; set; }          // recommended addition
        public string DoctorName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string Status { get; set; } = "Pending";

        public Appointment(string id, Doctor doc, Patient pat, string doctorName, DateTime time)
        {
            AppointmentId = id;
            AssignedDoctor = doc;
            Patient = pat;
            DoctorName = doctorName;
            ScheduleTime = time;
        }

        public void confirmAppointment() => Status = "Confirmed";
    }

    public partial class AppointmentForm : Form
    {
        private TextBox txtId;
        private ComboBox cmbPatient;
        private ComboBox cmbDoctor;
        private DateTimePicker dtp;
        private ListBox lst;

        public AppointmentForm()
        {
            ClinicData.Initialize();
            Text = "Appointment Management";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            RefreshList();
        }

        private void BuildUI()
        {
            Controls.Add(new Label { Text = "APPOINTMENT MANAGEMENT", Font = new Font("Arial", 18, FontStyle.Bold), Location = new Point(20, 15), Size = new Size(500, 35) });

            // ---- Appointment ID ----
            Controls.Add(new Label { Text = "Appointment ID:", Location = new Point(20, 70), Size = new Size(110, 25) });
            txtId = new TextBox { Location = new Point(140, 65), Width = 200 };
            Controls.Add(txtId);

            // ---- Patient dropdown ----
            Controls.Add(new Label { Text = "Patient:", Location = new Point(20, 110), Size = new Size(110, 25) });
            cmbPatient = new ComboBox { Location = new Point(140, 105), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var p in ClinicData.Patients)
                cmbPatient.Items.Add(p);
            if (cmbPatient.Items.Count > 0) cmbPatient.SelectedIndex = 0;
            Controls.Add(cmbPatient);

            // ---- Doctor dropdown ----
            Controls.Add(new Label { Text = "Doctor:", Location = new Point(20, 150), Size = new Size(110, 25) });
            cmbDoctor = new ComboBox { Location = new Point(140, 145), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var d in ClinicData.Doctors)
                cmbDoctor.Items.Add($"{d.EmployeeID} - Dr. {d.Name} ({d.Specialisation})");
            if (cmbDoctor.Items.Count > 0) cmbDoctor.SelectedIndex = 0;
            Controls.Add(cmbDoctor);

            // ---- Schedule time ----
            Controls.Add(new Label { Text = "Schedule Time:", Location = new Point(20, 190), Size = new Size(110, 25) });
            dtp = new DateTimePicker
            {
                Location = new Point(140, 185),
                Width = 200,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm"
            };
            Controls.Add(dtp);

            // ---- Buttons ----
            var btnSchedule = new Button { Text = "SCHEDULE", Location = new Point(140, 225), Size = new Size(120, 35), BackColor = Color.LightBlue };
            btnSchedule.Click += ScheduleClick;
            Controls.Add(btnSchedule);

            var btnConfirm = new Button { Text = "CONFIRM", Location = new Point(270, 225), Size = new Size(120, 35), BackColor = Color.LightGreen };
            btnConfirm.Click += ConfirmClick;
            Controls.Add(btnConfirm);

            var btnCancel = new Button { Text = "CANCEL", Location = new Point(400, 225), Size = new Size(120, 35), BackColor = Color.LightCoral };
            btnCancel.Click += CancelClick;
            Controls.Add(btnCancel);

            // ---- Appointment list ----
            lst = new ListBox { Location = new Point(20, 280), Size = new Size(840, 260), Font = new Font("Consolas", 10) };
            Controls.Add(lst);
        }

        private void RefreshList()
        {
            lst.Items.Clear();
            foreach (var a in ClinicData.Appointments)
            {
                string patName = a.Patient != null ? a.Patient.Name : "(unassigned)";
                lst.Items.Add($"{a.AppointmentId} | {patName} | {a.DoctorName} | {a.ScheduleTime:dd/MM/yyyy HH:mm} | {a.Status}");
            }
        }

        private void ScheduleClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Enter Appointment ID.");
                return;
            }
            if (cmbPatient.SelectedIndex < 0 || cmbDoctor.SelectedIndex < 0)
            {
                MessageBox.Show("Select patient and doctor.");
                return;
            }

            var doc = ClinicData.Doctors[cmbDoctor.SelectedIndex];
            var pat = ClinicData.Patients[cmbPatient.SelectedIndex];

            ClinicData.Appointments.Add(new Appointment(
                txtId.Text.Trim(), doc, pat, $"Dr. {doc.Name}", dtp.Value));

            txtId.Clear();
            RefreshList();
        }

        private void ConfirmClick(object sender, EventArgs e)
        {
            if (lst.SelectedIndex < 0)
            {
                MessageBox.Show("Select an appointment.");
                return;
            }
            var a = ClinicData.Appointments[lst.SelectedIndex];
            if (a.Status == "Cancelled")
            {
                MessageBox.Show("Cannot confirm a cancelled appointment.");
                return;
            }
            a.confirmAppointment();
            RefreshList();
        }

        private void CancelClick(object sender, EventArgs e)
        {
            if (lst.SelectedIndex < 0)
            {
                MessageBox.Show("Select an appointment.");
                return;
            }
            ClinicData.Appointments[lst.SelectedIndex].Status = "Cancelled";
            RefreshList();
        }
    }
}