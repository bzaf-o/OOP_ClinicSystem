using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ClinicSystem
{
    public partial class PatientForm : Form
    {
        private TextBox txtName, txtAge, txtContact, txtSymptoms, txtDiagnosis;
        private RadioButton rbMale, rbFemale;

        private void PatientForm_Load(object sender, EventArgs e)
        {

        }

        private DataGridView dgv;

        public PatientForm()
        {
            ClinicData.Initialize();
            Text = "Patient Registration";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            RefreshGrid();
        }

        private void BuildUI()
        {
            Controls.Add(new Label { Text = "PATIENT REGISTRATION", Font = new Font("Arial", 18, FontStyle.Bold), Location = new Point(20, 15), Size = new Size(500, 35) });

            Controls.Add(new Label { Text = "Name:", Location = new Point(20, 70), Size = new Size(90, 25) });
            txtName = new TextBox { Location = new Point(120, 65), Width = 240 }; Controls.Add(txtName);

            Controls.Add(new Label { Text = "Age:", Location = new Point(20, 110), Size = new Size(90, 25) });
            txtAge = new TextBox { Location = new Point(120, 105), Width = 80 }; Controls.Add(txtAge);

            Controls.Add(new Label { Text = "Gender:", Location = new Point(20, 150), Size = new Size(90, 25) });
            rbMale = new RadioButton { Text = "Male", Location = new Point(120, 150), Size = new Size(70, 25) };
            rbFemale = new RadioButton { Text = "Female", Location = new Point(200, 150), Size = new Size(90, 25) };
            Controls.Add(rbMale); Controls.Add(rbFemale);

            Controls.Add(new Label { Text = "Contact:", Location = new Point(20, 190), Size = new Size(90, 25) });
            txtContact = new TextBox { Location = new Point(120, 185), Width = 240 }; Controls.Add(txtContact);

            Controls.Add(new Label { Text = "Symptoms:", Location = new Point(20, 230), Size = new Size(90, 25) });
            txtSymptoms = new TextBox { Location = new Point(120, 225), Width = 240 }; Controls.Add(txtSymptoms);

            Controls.Add(new Label { Text = "Diagnosis:", Location = new Point(20, 270), Size = new Size(90, 25) });
            txtDiagnosis = new TextBox { Location = new Point(120, 265), Width = 240 }; Controls.Add(txtDiagnosis);

            var btn = new Button { Text = "REGISTER", Location = new Point(120, 310), Size = new Size(120, 35), BackColor = Color.LightGreen };
            btn.Click += RegisterPatient; Controls.Add(btn);

            dgv = new DataGridView
            {
                Location = new Point(20, 360),
                Size = new Size(940, 240),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,   // was Fill
                ScrollBars = ScrollBars.Both,                                 // both scrollbars
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "name", HeaderText = "Name", Width = 220 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "age", HeaderText = "Age", Width = 70 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "gender", HeaderText = "Gender", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "contact", HeaderText = "Contact", Width = 160 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "diag", HeaderText = "Diagnosis", Width = 300 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "sym", HeaderText = "Symptoms", Width = 300 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "plan", HeaderText = "Treatment", Width = 300 });
            Controls.Add(dgv);
        }

        private void RefreshGrid()
        {
            dgv.Rows.Clear();
            foreach (var p in ClinicData.Patients)
                dgv.Rows.Add(p.PatientId, p.Name, p.Age, p.Gender, p.Contact,
                             p.Diagnosis, p.Symptoms, "");
        }

        private void RegisterPatient(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("Please enter patient's name."); return; }

            if (!int.TryParse(txtAge.Text, out int age) || age < 0)
            { MessageBox.Show("Please enter a valid age."); return; }

            if (!rbMale.Checked && !rbFemale.Checked)
            { MessageBox.Show("Please select patient's gender."); return; }

            if (string.IsNullOrWhiteSpace(txtContact.Text))
            { MessageBox.Show("Please enter a contact number."); return; }

            string gender = rbFemale.Checked ? "Female" : "Male";
            var p = new Patient("P" + (ClinicData.Patients.Count + 1).ToString("000"),
                txtName.Text.Trim(), age, gender, txtContact.Text.Trim())
            {
                Symptoms = txtSymptoms.Text.Trim(),
                Diagnosis = txtDiagnosis.Text.Trim()
            };
            ClinicData.Patients.Add(p);

            // Save to file (as original)
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "PatientDetails.txt");
            File.AppendAllText(filePath,
                $"PATIENT REGISTRATION DETAILS\n------------------------------\n" +
                $"Name: {p.Name}\nAge: {p.Age}\nGender: {p.Gender}\nContact: {p.Contact}\n\n");

            RefreshGrid();
            MessageBox.Show("Patient Registered Successfully!\n\nPatient Name: " + p.Name,
                "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}