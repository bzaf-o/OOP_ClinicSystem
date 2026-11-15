using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicSystem
{
    public partial class PatientCareForm : Form
    {
        private readonly Doctor doctor;
        private ListBox lstPatients;
        private TextBox txtSymptoms;
        private ComboBox cmbDiagnosis, cmbMedicine;
        private ListBox lstPrescriptions;
        private TextBox txtQty;

        private Patient currentPatient;

        public PatientCareForm(Doctor doc)
        {
            doctor = doc;
            ClinicData.Initialize();
            Text = $"Patient Care - Dr. {doctor.Name}";
            Size = new Size(950, 620);
            StartPosition = FormStartPosition.CenterParent;
            BuildUI();
            LoadAssignedPatients();
        }

        private void BuildUI()
        {
            Controls.Add(new Label
            {
                Text = $"PATIENT CARE — Dr. {doctor.Name} ({doctor.Specialisation})",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 15),
                Size = new Size(880, 30)
            });

            Controls.Add(new Label { Text = "Assigned patients:", Location = new Point(20, 60), Size = new Size(120, 25) });
            lstPatients = new ListBox { Location = new Point(20, 85), Size = new Size(280, 480) };
            lstPatients.SelectedIndexChanged += (s, e) => LoadSelectedPatient();
            Controls.Add(lstPatients);

            Controls.Add(new Label { Text = "Symptoms:", Location = new Point(320, 60), Size = new Size(90, 25) });
            txtSymptoms = new TextBox
            {
                Location = new Point(320, 85),
                Size = new Size(400, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            Controls.Add(txtSymptoms);

            Controls.Add(new Label { Text = "Diagnosis:", Location = new Point(320, 195), Size = new Size(90, 25) });
            cmbDiagnosis = new ComboBox { Location = new Point(320, 220), Width = 400, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDiagnosis.Items.AddRange(new object[]
            {
                "Common cold or flu",
                "Tension headache or migraine",
                "Possible cardiac issue – refer to cardiology",
                "Neurological assessment needed",
                "Gastritis",
                "General check-up required"
            });
            cmbDiagnosis.SelectedIndex = 0;
            Controls.Add(cmbDiagnosis);

            Controls.Add(new Label { Text = "Prescribe medicine:", Location = new Point(320, 265), Size = new Size(130, 25) });
            cmbMedicine = new ComboBox { Location = new Point(450, 260), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var m in ClinicData.Inventory)
                cmbMedicine.Items.Add(m.MedicineName);
            if (cmbMedicine.Items.Count > 0) cmbMedicine.SelectedIndex = 0;
            Controls.Add(cmbMedicine);

            Controls.Add(new Label { Text = "Qty:", Location = new Point(660, 265), Size = new Size(35, 25) });
            txtQty = new TextBox { Location = new Point(695, 260), Width = 60, Text = "1" };
            Controls.Add(txtQty);

            var btnAddMed = new Button { Text = "ADD", Location = new Point(765, 258), Size = new Size(80, 28), BackColor = Color.LightBlue };
            btnAddMed.Click += AddPrescription;
            Controls.Add(btnAddMed);

            lstPrescriptions = new ListBox { Location = new Point(320, 300), Size = new Size(525, 150) };
            Controls.Add(lstPrescriptions);

            var btnSave = new Button
            {
                Text = "SAVE CARE",
                Location = new Point(320, 470),
                Size = new Size(180, 45),
                BackColor = Color.LightGreen
            };
            btnSave.Click += SaveCare;
            Controls.Add(btnSave);
        }

        private void LoadAssignedPatients()
        {
            lstPatients.Items.Clear();
            // Patients whose appointments were assigned to this doctor
            var ids = ClinicData.Appointments
                .Where(a => a.AssignedDoctor != null && a.AssignedDoctor.EmployeeID == doctor.EmployeeID)
                .Select(a => a.DoctorName)
                .Distinct()
                .ToList();

            // Fallback: if no assigned appointments, allow all patients
            var patients = ids.Count > 0
                ? ClinicData.Patients.Where(p => ids.Contains(p.Name))
                : ClinicData.Patients;

            foreach (var p in patients)
                lstPatients.Items.Add(p);
        }

        private void LoadSelectedPatient()
        {
            currentPatient = lstPatients.SelectedItem as Patient;
            if (currentPatient == null) return;

            txtSymptoms.Text = currentPatient.Symptoms ?? "";
            if (currentPatient.Diagnosis != null && cmbDiagnosis.Items.Contains(currentPatient.Diagnosis))
                cmbDiagnosis.SelectedItem = currentPatient.Diagnosis;
            else
                cmbDiagnosis.SelectedIndex = 0;

            lstPrescriptions.Items.Clear();
        }

        private void AddPrescription(object s, EventArgs e)
        {
            if (cmbMedicine.SelectedIndex < 0) return;
            if (!int.TryParse(txtQty.Text, out int q) || q <= 0) return;

            var med = ClinicData.Inventory[cmbMedicine.SelectedIndex];
            if (med.StockQty < q) { MessageBox.Show("Insufficient stock."); return; }

            med.StockQty -= q;
            lstPrescriptions.Items.Add($"{med.MedicineName} x{q}");

            // Touch shared data so Inventory form sees updated stock
            ClinicData.Inventory[cmbMedicine.SelectedIndex] = med;

            // Update the combo display text
            cmbMedicine.Items[cmbMedicine.SelectedIndex] = med.MedicineName;
        }

        private void SaveCare(object s, EventArgs e)
        {
            if (currentPatient == null) { MessageBox.Show("Select a patient."); return; }

            currentPatient.Symptoms = txtSymptoms.Text.Trim();
            currentPatient.Diagnosis = cmbDiagnosis.SelectedItem?.ToString();

            // Log to medical record history
            string rid = currentPatient.PatientId;
            if (!ClinicData.MedicalRecords.ContainsKey(rid))
                ClinicData.MedicalRecords[rid] = new System.Collections.Generic.List<MedicalRecordEntry>();

            string prescribed = lstPrescriptions.Items.Count > 0
                ? string.Join(", ", lstPrescriptions.Items.Cast<string>())
                : "None";

            ClinicData.MedicalRecords[rid].Add(new MedicalRecordEntry
            {
                RecordId = rid,
                PatientName = currentPatient.Name,
                Symptoms = currentPatient.Symptoms,
                Diagnosis = currentPatient.Diagnosis,
                TreatmentPlan = "Prescribed: " + prescribed,
                UpdatedAt = DateTime.Now
            });

            MessageBox.Show($"Care saved for {currentPatient.Name}.",
                "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}