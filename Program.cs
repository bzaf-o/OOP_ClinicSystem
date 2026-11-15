using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicSystem
{
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            ClinicData.Initialize();
            Text = "Clinic Management System";
            Size = new Size(600, 500);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;

            AddMenuButton("Doctor Management", 60, () => new DoctorForm().Show());
            AddMenuButton("Patient Registration", 130, () => new PatientForm().Show());
            AddMenuButton("Appointments", 200, () => new AppointmentForm().Show());
            AddMenuButton("Inventory", 270, () => new InventoryForm().Show());
            AddMenuButton("Medical Records", 340, () => new MedicalRecordForm().Show());
            AddMenuButton("Billing / Invoices", 410, () => new InvoiceForm().Show());

            Controls.Add(new Label
            {
                Text = "CLINIC MANAGEMENT SYSTEM",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(30, 15),
                Size = new Size(540, 35)
            });
        }

        private void AddMenuButton(string text, int y, Action onClick)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(130, y),
                Size = new Size(320, 45),
                BackColor = Color.LightSteelBlue,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            b.Click += (s, e) => onClick();
            Controls.Add(b);
        }

        [STAThread]
        static void Main() => Application.Run(new MainMenuForm());
    }
}