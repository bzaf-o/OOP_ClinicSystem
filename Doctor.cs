using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicSystem
{
    public class Doctor
    {
        public string EmployeeID { get; private set; }
        public string Name { get; private set; }
        public string Specialisation { get; private set; }
        public int StartWorkingHour { get; private set; }
        public int EndWorkingHour { get; private set; }
        public int MonthlySalary { get; private set; }
        public string Status { get; set; } = "Off duty";

        public Doctor(string employeeID, string name, string specialisation,
                      int start, int end, int salary)
        {
            EmployeeID = employeeID; Name = name; Specialisation = specialisation;
            StartWorkingHour = start; EndWorkingHour = end; MonthlySalary = salary;
        }

        public void clockIn() { Status = "Clocked in"; }
        public void clockOut() { Status = "Off duty"; }

        public string diagnosePatient(string[] symptoms)
        {
            if (symptoms == null || symptoms.Length == 0)
                return "No symptoms provided. Unable to diagnose.";
            string diagnosis = "General check-up required";
            foreach (var s in symptoms)
            {
                switch (s.ToLower())
                {
                    case "fever": case "cough": diagnosis = "Common cold or flu"; break;
                    case "headache": diagnosis = "Tension headache or migraine"; break;
                    case "chest pain": diagnosis = "Possible cardiac issue – refer to cardiology"; break;
                    case "numbness": diagnosis = "Neurological assessment needed"; break;
                }
            }
            return diagnosis;
        }

        public void treatPatient() => MessageBox.Show($"{Name} is treating a patient.");
        public void prescribeMedicine() => MessageBox.Show($"{Name} is prescribing medicine.");
    }

    // ===== Doctor Form =====
    public partial class DoctorForm : Form
    {
        private DataGridView dgv;
        private TextBox txtName, txtSpecialty, txtStart, txtEnd, txtSalary;

        public DoctorForm()
        {
            ClinicData.Initialize();
            Text = "Doctor Management";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            RefreshGrid();
        }

        private void BuildUI()
        {
            var title = new Label { Text = "DOCTOR MANAGEMENT", Font = new Font("Arial", 18, FontStyle.Bold), Location = new Point(20, 15), Size = new Size(500, 35) };
            Controls.Add(title);

            Controls.Add(new Label { Text = "Name:", Location = new Point(20, 70), Size = new Size(90, 25) });
            txtName = new TextBox { Location = new Point(120, 65), Width = 220 };
            Controls.Add(txtName);

            Controls.Add(new Label { Text = "Specialisation:", Location = new Point(20, 110), Size = new Size(90, 25) });
            txtSpecialty = new TextBox { Location = new Point(120, 105), Width = 220 };
            Controls.Add(txtSpecialty);

            Controls.Add(new Label { Text = "Start Hour:", Location = new Point(370, 70), Size = new Size(80, 25) });
            txtStart = new TextBox { Location = new Point(450, 65), Width = 60 };
            Controls.Add(txtStart);

            Controls.Add(new Label { Text = "End Hour:", Location = new Point(370, 110), Size = new Size(80, 25) });
            txtEnd = new TextBox { Location = new Point(450, 105), Width = 60 };
            Controls.Add(txtEnd);

            Controls.Add(new Label { Text = "Salary:", Location = new Point(540, 70), Size = new Size(60, 25) });
            txtSalary = new TextBox { Location = new Point(600, 65), Width = 100 };
            Controls.Add(txtSalary);

            AddBtn("ADD", 20, 150, AddDoctor);
            AddBtn("UPDATE", 130, 150, UpdateDoctor);
            AddBtn("CLOCK IN", 240, 150, (s, e) => SetStatus("Clocked in"));
            AddBtn("CLOCK OUT", 350, 150, (s, e) => SetStatus("Off duty"));
            AddBtn("PATIENT CARE", 460, 150, OpenPatientCare);   // replaces DIAGNOSE

            dgv = new DataGridView
            {
                Location = new Point(20, 200),
                Size = new Size(950, 350),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgv.Columns.Add("id", "ID");
            dgv.Columns.Add("name", "Name");
            dgv.Columns.Add("spec", "Specialisation");
            dgv.Columns.Add("hours", "Working Hours");
            dgv.Columns.Add("salary", "Salary");
            dgv.Columns.Add("status", "Status");
            dgv.CellClick += (s, e) => LoadSelected();
            Controls.Add(dgv);
        }

        private void AddBtn(string text, int x, int y, EventHandler h)
        {
            var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(100, 32), BackColor = Color.LightBlue };
            b.Click += h;
            Controls.Add(b);
        }

        private Doctor Selected() =>
            dgv.CurrentRow == null ? null :
            ClinicData.Doctors.Find(d => d.EmployeeID == (string)dgv.CurrentRow.Cells[0].Value);

        private void RefreshGrid()
        {
            dgv.Rows.Clear();
            foreach (var d in ClinicData.Doctors)
                dgv.Rows.Add(d.EmployeeID, d.Name, d.Specialisation,
                    $"{d.StartWorkingHour:00}:00 - {d.EndWorkingHour:00}:00",
                    d.MonthlySalary, d.Status);
        }

        private void LoadSelected()
        {
            var d = Selected();
            if (d == null) return;
            txtName.Text = d.Name; txtSpecialty.Text = d.Specialisation;
            txtStart.Text = d.StartWorkingHour.ToString();
            txtEnd.Text = d.EndWorkingHour.ToString();
            txtSalary.Text = d.MonthlySalary.ToString();
        }

        private void AddDoctor(object s, EventArgs e)
        {
            if (!int.TryParse(txtStart.Text, out int sh) || !int.TryParse(txtEnd.Text, out int eh)
                || !int.TryParse(txtSalary.Text, out int sal) || sh >= eh)
            {
                MessageBox.Show("Enter valid working hours and salary."); return;
            }
            ClinicData.Doctors.Add(new Doctor("D" + (ClinicData.Doctors.Count + 1).ToString("000"),
                txtName.Text, txtSpecialty.Text, sh, eh, sal));
            RefreshGrid();
        }

        private void UpdateDoctor(object s, EventArgs e)
        {
            var d = Selected();
            if (d == null) { MessageBox.Show("Select a doctor first."); return; }
            if (!int.TryParse(txtStart.Text, out int sh) || !int.TryParse(txtEnd.Text, out int eh)
                || !int.TryParse(txtSalary.Text, out int sal)) return;
            ClinicData.Doctors[ClinicData.Doctors.IndexOf(d)] =
                new Doctor(d.EmployeeID, txtName.Text, txtSpecialty.Text, sh, eh, sal) { Status = d.Status };
            RefreshGrid();
        }

        private void SetStatus(string status)
        {
            var d = Selected();
            if (d == null) { MessageBox.Show("Select a doctor first."); return; }
            d.Status = status;
            RefreshGrid();
        }

        // Patient Care
        private void OpenPatientCare(object s, EventArgs e)
        {
            var d = Selected();
            if (d == null) { MessageBox.Show("Select a doctor first."); return; }

            if (d.Status != "Clocked in")
            {
                MessageBox.Show($"{d.Name} is not clocked in. Please clock in first.",
                    "Not on duty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            new PatientCareForm(d).ShowDialog(this);
        }
    }
}