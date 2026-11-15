using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicSystem
{
    // ===== Invoice class — matches provided UML exactly =====
    public class Invoice
    {
        // UML attributes
        private string invoiceID;
        private string[] medicine;
        private Patient patient;
        private double consultFee;
        private double[] medicineCost;
        private string paymentMethod;
        private bool paid;

        // UML getters/setters
        public string InvoiceID { get => invoiceID; set => invoiceID = value; }
        public string[] Medicine { get => medicine; set => medicine = value; }
        public Patient Patient { get => patient; set => patient = value; }
        public double ConsultFee { get => consultFee; set => consultFee = value; }
        public double[] MedicineCost { get => medicineCost; set => medicineCost = value; }
        public string PaymentMethod { get => paymentMethod; set => paymentMethod = value; }

        public Invoice(string invoiceID, Patient patient, double consultFee,
                       string[] medicine, double[] medicineCost, string paymentMethod, bool paid)
        {
            this.invoiceID = invoiceID;
            this.patient = patient;
            this.consultFee = consultFee;
            this.medicine = medicine;
            this.medicineCost = medicineCost;
            this.paymentMethod = paymentMethod;
            this.paid = paid;
        }

        // UML methods
        public bool isPaid() => paid;
        public void markPaid() => paid = true;

        public double calculatePayment()
        {
            double total = consultFee;
            if (medicineCost != null)
                foreach (var c in medicineCost) total += c;
            return total;
        }

        public string MedicineSummary =>
            (medicine == null || medicine.Length == 0) ? "None" : string.Join(", ", medicine);
    }

    // ===== Invoice Form =====
    public partial class InvoiceForm : Form
    {
        // Fixed price per unit — not editable from the UI
        private static readonly Dictionary<string, double> FixedPrices =
            new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            { "Paracetamol",        8.50  },
            { "Cough Syrup",        12.00 },
            { "Vitamin C",          6.00  },
            { "Amoxicillin 250mg",  25.00 }
        };

        private ComboBox cmbPatient, cmbPayment, cmbMedicine;
        private TextBox txtConsultFee, txtMedQty;
        private Label lblUnitPrice;
        private ListBox lstMedicines;
        private DataGridView dgv;

        private string[] pendingMedicines = new string[0];
        private double[] pendingCosts = new double[0];

        public InvoiceForm()
        {
            ClinicData.Initialize();
            Text = "Billing / Invoices";
            Size = new Size(1050, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            RefreshGrid();
        }

        private void BuildUI()
        {
            Controls.Add(new Label { Text = "BILLING / INVOICES", Font = new Font("Arial", 18, FontStyle.Bold), Location = new Point(20, 15), Size = new Size(500, 35) });

            // ---- Patient ----
            Controls.Add(new Label { Text = "Patient:", Location = new Point(20, 65), Size = new Size(90, 25) });
            cmbPatient = new ComboBox { Location = new Point(120, 60), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var p in ClinicData.Patients) cmbPatient.Items.Add(p);
            if (cmbPatient.Items.Count > 0) cmbPatient.SelectedIndex = 0;
            Controls.Add(cmbPatient);

            // ---- Consult fee ----
            Controls.Add(new Label { Text = "Consult Fee:", Location = new Point(20, 105), Size = new Size(90, 25) });
            txtConsultFee = new TextBox { Location = new Point(120, 100), Width = 100, Text = "30.00" };
            Controls.Add(txtConsultFee);

            // ---- Payment method ----
            Controls.Add(new Label { Text = "Payment:", Location = new Point(240, 105), Size = new Size(70, 25) });
            cmbPayment = new ComboBox { Location = new Point(315, 100), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPayment.Items.AddRange(new object[] { "Cash", "Card", "Online" });
            cmbPayment.SelectedIndex = 0;
            Controls.Add(cmbPayment);

            // ---- Medicine picker ----
            Controls.Add(new Label { Text = "Add Medicine:", Location = new Point(20, 150), Size = new Size(90, 25) });
            cmbMedicine = new ComboBox { Location = new Point(120, 145), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var m in ClinicData.Inventory)
                cmbMedicine.Items.Add(m.MedicineName);
            if (cmbMedicine.Items.Count > 0) cmbMedicine.SelectedIndex = 0;
            Controls.Add(cmbMedicine);

            Controls.Add(new Label { Text = "Qty:", Location = new Point(410, 150), Size = new Size(35, 25) });
            txtMedQty = new TextBox { Location = new Point(445, 145), Width = 60, Text = "1" };
            Controls.Add(txtMedQty);

            // ---- Unit price (read-only label) ----
            Controls.Add(new Label { Text = "Unit Price:", Location = new Point(515, 150), Size = new Size(70, 25) });
            lblUnitPrice = new Label
            {
                Location = new Point(590, 150),
                Size = new Size(90, 25),
                Text = "RM 0.00",
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            Controls.Add(lblUnitPrice);

            // Update label whenever the medicine changes
            cmbMedicine.SelectedIndexChanged += (s, e) => UpdateUnitPriceLabel();
            UpdateUnitPriceLabel();

            // ---- Add-to-invoice button ----
            var btnAdd = new Button
            {
                Text = "ADD TO INVOICE",
                Location = new Point(680, 143),
                Size = new Size(140, 30),
                BackColor = Color.LightBlue
            };
            btnAdd.Click += AddMed;
            Controls.Add(btnAdd);

            // ---- Pending list + action buttons ----
            lstMedicines = new ListBox { Location = new Point(20, 190), Size = new Size(400, 120) };
            Controls.Add(lstMedicines);

            var btnCreate = new Button
            {
                Text = "CREATE INVOICE",
                Location = new Point(440, 190),
                Size = new Size(160, 40),
                BackColor = Color.LightGreen
            };
            btnCreate.Click += CreateInvoice;
            Controls.Add(btnCreate);

            var btnPay = new Button
            {
                Text = "MARK PAID",
                Location = new Point(610, 190),
                Size = new Size(130, 40),
                BackColor = Color.Khaki
            };
            btnPay.Click += MarkPaid;
            Controls.Add(btnPay);

            // ---- Invoice table ----
            dgv = new DataGridView
            {
                Location = new Point(20, 330),
                Size = new Size(1000, 290),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgv.Columns.Add("id", "Invoice ID");
            dgv.Columns.Add("patient", "Patient");
            dgv.Columns.Add("consult", "Consult Fee");
            dgv.Columns.Add("med", "Medicine");
            dgv.Columns.Add("total", "Total");
            dgv.Columns.Add("payment", "Payment");
            dgv.Columns.Add("paid", "Paid");
            Controls.Add(dgv);
        }

        private void UpdateUnitPriceLabel()
        {
            string name = cmbMedicine.SelectedItem?.ToString() ?? "";
            double price = FixedPrices.TryGetValue(name, out double p) ? p : 0.00;
            lblUnitPrice.Text = $"RM {price:0.00}";
        }

        private void RefreshGrid()
        {
            dgv.Rows.Clear();
            foreach (var inv in ClinicData.Invoices)
                dgv.Rows.Add(inv.InvoiceID, inv.Patient?.Name, inv.ConsultFee.ToString("0.00"),
                    inv.MedicineSummary, inv.calculatePayment().ToString("0.00"),
                    inv.PaymentMethod, inv.isPaid() ? "Yes" : "No");
        }

        private void AddMed(object s, EventArgs e)
        {
            if (cmbMedicine.SelectedIndex < 0) return;
            if (!int.TryParse(txtMedQty.Text, out int q) || q <= 0)
            {
                MessageBox.Show("Enter a valid quantity.");
                return;
            }

            string name = cmbMedicine.SelectedItem.ToString();
            if (!FixedPrices.TryGetValue(name, out double price))
            {
                MessageBox.Show($"No fixed price configured for '{name}'.");
                return;
            }

            double cost = q * price;

            var meds = pendingMedicines.ToList();
            meds.Add($"{name} x{q}");
            pendingMedicines = meds.ToArray();

            var costs = pendingCosts.ToList();
            costs.Add(cost);
            pendingCosts = costs.ToArray();

            lstMedicines.Items.Add($"{name} x{q} @ RM {price:0.00} = RM {cost:0.00}");
        }

        private void CreateInvoice(object s, EventArgs e)
        {
            if (cmbPatient.SelectedItem == null)
            {
                MessageBox.Show("Select a patient.");
                return;
            }
            if (!double.TryParse(txtConsultFee.Text, out double cf) || cf < 0)
            {
                MessageBox.Show("Enter valid consult fee.");
                return;
            }

            var inv = new Invoice(
                "INV" + (ClinicData.Invoices.Count + 1).ToString("000"),
                (Patient)cmbPatient.SelectedItem,
                cf,
                pendingMedicines.Length == 0 ? new[] { "None" } : pendingMedicines,
                pendingCosts,
                cmbPayment.SelectedItem?.ToString() ?? "Cash",
                false);

            ClinicData.Invoices.Add(inv);

            pendingMedicines = new string[0];
            pendingCosts = new double[0];
            lstMedicines.Items.Clear();
            RefreshGrid();
        }

        private void MarkPaid(object s, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Select an invoice.");
                return;
            }
            string id = dgv.CurrentRow.Cells[0].Value.ToString();
            var inv = ClinicData.Invoices.FirstOrDefault(x => x.InvoiceID == id);
            if (inv == null) return;
            inv.markPaid();
            RefreshGrid();
        }
    }
}