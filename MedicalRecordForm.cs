using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicSystem
{
    public partial class MedicalRecordForm : Form
    {
        private ComboBox cmbPatient;               // ← new
        private TextBox txtRecordId, txtPatientName, txtSymptoms, txtDiagnosis, txtPlan;

        public MedicalRecordForm()
        {
            ClinicData.Initialize();
            Text = "Medical Record";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
        }

        private void BuildUI()
        {
            Controls.Add(new Label { Text = "MEDICAL RECORD", Font = new Font("Arial", 18, FontStyle.Bold), Location = new Point(20, 15), Size = new Size(500, 35) });

            // Patient dropdown
            Controls.Add(new Label { Text = "Patient:", Location = new Point(20, 65), Size = new Size(110, 25) });
            cmbPatient = new ComboBox { Location = new Point(140, 60), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var p in ClinicData.Patients)
                cmbPatient.Items.Add(p);
            if (cmbPatient.Items.Count > 0) cmbPatient.SelectedIndex = 0;
            cmbPatient.SelectedIndexChanged += (s, e) => SyncPatientSelection();
            Controls.Add(cmbPatient);

            // Record ID + patient name below
            Controls.Add(new Label { Text = "Record ID:", Location = new Point(20, 105), Size = new Size(110, 25) });
            txtRecordId = new TextBox { Location = new Point(140, 100), Width = 200 };
            Controls.Add(txtRecordId);

            Controls.Add(new Label { Text = "Patient Name:", Location = new Point(20, 145), Size = new Size(110, 25) });
            txtPatientName = new TextBox { Location = new Point(140, 140), Width = 200 };
            Controls.Add(txtPatientName);

            Controls.Add(new Label { Text = "Symptoms:", Location = new Point(20, 185), Size = new Size(110, 25) });
            txtSymptoms = new TextBox { Location = new Point(140, 180), Width = 500, Height = 80, Multiline = true, ScrollBars = ScrollBars.Vertical };
            Controls.Add(txtSymptoms);

            Controls.Add(new Label { Text = "Diagnosis:", Location = new Point(20, 275), Size = new Size(110, 25) });
            txtDiagnosis = new TextBox { Location = new Point(140, 270), Width = 500, Height = 80, Multiline = true, ScrollBars = ScrollBars.Vertical };
            Controls.Add(txtDiagnosis);

            Controls.Add(new Label { Text = "Treatment Plan:", Location = new Point(20, 365), Size = new Size(110, 25) });
            txtPlan = new TextBox { Location = new Point(140, 360), Width = 500, Height = 80, Multiline = true, ScrollBars = ScrollBars.Vertical };
            Controls.Add(txtPlan);

            var btnSave = new Button { Text = "UPDATE RECORD", Location = new Point(680, 180), Size = new Size(200, 60), BackColor = Color.LightGreen };
            btnSave.Click += SaveRecord; Controls.Add(btnSave);

            var btnHistory = new Button { Text = "VIEW HISTORY", Location = new Point(680, 260), Size = new Size(200, 60), BackColor = Color.LightBlue };
            btnHistory.Click += ViewHistory; Controls.Add(btnHistory);

            SyncPatientSelection();
        }

        private void SyncPatientSelection()
        {
            var p = cmbPatient.SelectedItem as Patient;
            if (p == null) return;

            txtPatientName.Text = p.Name;

            // If a record already exists for this patient, show its ID
            if (ClinicData.MedicalRecords.ContainsKey(p.PatientId)
                && ClinicData.MedicalRecords[p.PatientId].Count > 0)
            {
                var latest = ClinicData.MedicalRecords[p.PatientId]
                    .OrderByDescending(x => x.UpdatedAt).First();
                txtRecordId.Text = latest.RecordId;
                txtSymptoms.Text = latest.Symptoms ?? "";
                txtDiagnosis.Text = latest.Diagnosis ?? "";
                txtPlan.Text = latest.TreatmentPlan ?? "";
            }
            else
            {
                txtRecordId.Text = "";          // blank
                txtSymptoms.Text = "";
                txtDiagnosis.Text = "";
                txtPlan.Text = "";
            }
        }

        // SaveRecord and ViewHistory unchanged except they use the record ID as typed
        private void SaveRecord(object sender, EventArgs e)
        {
            string rid = txtRecordId.Text.Trim();
            string pname = txtPatientName.Text.Trim();
            if (rid.Length == 0) { MessageBox.Show("Enter a Record ID."); return; }
            if (pname.Length == 0) { MessageBox.Show("Enter a Patient Name."); return; }

            if (!ClinicData.MedicalRecords.ContainsKey(rid))
                ClinicData.MedicalRecords[rid] = new List<MedicalRecordEntry>();

            ClinicData.MedicalRecords[rid].Add(new MedicalRecordEntry
            {
                RecordId = rid,
                PatientName = pname,
                Symptoms = txtSymptoms.Text.Trim(),
                Diagnosis = txtDiagnosis.Text.Trim(),
                TreatmentPlan = txtPlan.Text.Trim(),
                UpdatedAt = DateTime.Now
            });

            MessageBox.Show($"Record saved. Revision {ClinicData.MedicalRecords[rid].Count} now in history.",
                "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ViewHistory(object sender, EventArgs e)
        {
            string rid = txtRecordId.Text.Trim();
            if (rid.Length == 0) { MessageBox.Show("Enter a Record ID."); return; }
            if (!ClinicData.MedicalRecords.TryGetValue(rid, out var hist) || hist.Count == 0)
            { MessageBox.Show($"No history for record {rid}."); return; }

            string output = string.Join("\n\n=============\n\n",
                hist.OrderByDescending(h => h.UpdatedAt).Select(h =>
                    $"Updated: {h.UpdatedAt:f}\nPatient: {h.PatientName}\n" +
                    $"Symptoms: {h.Symptoms}\nDiagnosis: {h.Diagnosis}\nPlan: {h.TreatmentPlan}"));

            MessageBox.Show(output, $"History - {rid}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}