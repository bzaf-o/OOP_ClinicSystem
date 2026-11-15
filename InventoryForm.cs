using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicSystem
{
    // InventoryItem (matches inventory.cs)
    public class InventoryItem
    {
        public string MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string MedicineType { get; set; }
        public int StockQty { get; set; }
        public int ReorderQty { get; set; }

        public InventoryItem(string id, string name, string type, int stock, int reorder)
        {
            MedicineId = id; MedicineName = name; MedicineType = type;
            StockQty = stock; ReorderQty = reorder;
        }

        public int restockOrder(int quantity) { StockQty += quantity; return StockQty; }
        public int updateOrderQuantity(int q) { ReorderQty = q; return ReorderQty; }
        public int updateStock(int q) { StockQty = q; return StockQty; }
        public string searchMedicine() => MedicineName;
    }

    public partial class InventoryForm : Form
    {
        private TextBox txtId, txtName, txtType, txtStock, txtReorder, txtSearch;
        private DataGridView dgv;

        public InventoryForm()
        {
            ClinicData.Initialize();
            Text = "Inventory Management";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            RefreshGrid();
        }

        private void BuildUI()
        {
            Controls.Add(new Label { Text = "INVENTORY MANAGEMENT", Font = new Font("Arial", 18, FontStyle.Bold), Location = new Point(30, 15), Size = new Size(500, 35) });

            AddField("Medicine ID:", 30, 70, out txtId);
            AddField("Medicine Name:", 30, 110, out txtName);
            AddField("Medicine Type:", 30, 150, out txtType);
            AddField("Stock Quantity:", 30, 190, out txtStock);
            AddField("Reorder Quantity:", 30, 230, out txtReorder);

            AddBtn("ADD", 450, 70, AddMedicine);
            AddBtn("UPDATE", 560, 70, UpdateMedicine);
            AddBtn("DELETE", 670, 70, DeleteMedicine);
            AddBtn("CLEAR", 780, 70, (s, e) => ClearAll());
            AddBtn("RESTOCK", 450, 115, RestockMedicine);

            Controls.Add(new Label { Text = "Search:", Location = new Point(450, 165), Size = new Size(60, 25) });
            txtSearch = new TextBox { Location = new Point(515, 160), Width = 200 }; Controls.Add(txtSearch);
            AddBtn("SEARCH", 725, 158, SearchMedicine);

            dgv = new DataGridView
            {
                Location = new Point(30, 220),
                Size = new Size(930, 370),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgv.Columns.Add("id", "Medicine ID");
            dgv.Columns.Add("name", "Medicine Name");
            dgv.Columns.Add("type", "Medicine Type");
            dgv.Columns.Add("stock", "Stock Qty");
            dgv.Columns.Add("reorder", "Reorder Qty");
            dgv.CellClick += (s, e) => LoadSelected();
            Controls.Add(dgv);
        }

        private void AddField(string label, int x, int y, out TextBox tb)
        {
            Controls.Add(new Label { Text = label, Location = new Point(x, y), Size = new Size(110, 25) });
            tb = new TextBox { Location = new Point(x + 120, y - 5), Width = 260 };
            Controls.Add(tb);
        }

        private void AddBtn(string text, int x, int y, EventHandler h)
        {
            var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(100, 35), BackColor = Color.LightBlue };
            b.Click += h; Controls.Add(b);
        }

        private void RefreshGrid()
        {
            dgv.Rows.Clear();
            foreach (var i in ClinicData.Inventory)
            {
                int row = dgv.Rows.Add(i.MedicineId, i.MedicineName, i.MedicineType, i.StockQty, i.ReorderQty);
                if (i.StockQty <= i.ReorderQty)
                    dgv.Rows[row].DefaultCellStyle.ForeColor = Color.DarkRed;
            }
        }

        private void LoadSelected()
        {
            if (dgv.CurrentRow == null) return;
            txtId.Text = dgv.CurrentRow.Cells[0].Value.ToString();
            txtName.Text = dgv.CurrentRow.Cells[1].Value.ToString();
            txtType.Text = dgv.CurrentRow.Cells[2].Value.ToString();
            txtStock.Text = dgv.CurrentRow.Cells[3].Value.ToString();
            txtReorder.Text = dgv.CurrentRow.Cells[4].Value.ToString();
        }

        private InventoryItem Selected() =>
            dgv.CurrentRow == null ? null :
            ClinicData.Inventory.FirstOrDefault(i => i.MedicineId == (string)dgv.CurrentRow.Cells[0].Value);

        private void ClearAll()
        {
            txtId.Clear(); txtName.Clear(); txtType.Clear(); txtStock.Clear(); txtReorder.Clear(); txtSearch.Clear();
        }

        private void AddMedicine(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text)
                || !int.TryParse(txtStock.Text, out int st) || !int.TryParse(txtReorder.Text, out int ro))
            { MessageBox.Show("Enter valid data."); return; }

            if (ClinicData.Inventory.Any(i => i.MedicineId.Equals(txtId.Text, StringComparison.OrdinalIgnoreCase)))
            { MessageBox.Show("Medicine ID exists."); return; }

            ClinicData.Inventory.Add(new InventoryItem(txtId.Text.Trim(), txtName.Text.Trim(),
                txtType.Text.Trim(), st, ro));
            RefreshGrid(); ClearAll();
        }

        private void UpdateMedicine(object s, EventArgs e)
        {
            var item = Selected();
            if (item == null) { MessageBox.Show("Select a medicine."); return; }
            if (!int.TryParse(txtStock.Text, out int st) || !int.TryParse(txtReorder.Text, out int ro))
            { MessageBox.Show("Enter valid quantities."); return; }
            item.MedicineName = txtName.Text.Trim();
            item.MedicineType = txtType.Text.Trim();
            item.updateStock(st); item.updateOrderQuantity(ro);
            RefreshGrid();
        }

        private void DeleteMedicine(object s, EventArgs e)
        {
            var item = Selected();
            if (item == null) { MessageBox.Show("Select a medicine."); return; }
            if (MessageBox.Show("Delete this medicine?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ClinicData.Inventory.Remove(item);
                RefreshGrid(); ClearAll();
            }
        }

        private void RestockMedicine(object s, EventArgs e)
        {
            var item = Selected();
            if (item == null) { MessageBox.Show("Select a medicine."); return; }
            var input = Microsoft.VisualBasic.Interaction.InputBox("Enter restock quantity:", "Restock");
            if (int.TryParse(input, out int q) && q > 0)
            {
                item.restockOrder(q);
                RefreshGrid();
            }
        }

        private void SearchMedicine(object s, EventArgs e)
        {
            string key = txtSearch.Text.Trim();
            dgv.Rows.Clear();
            foreach (var i in ClinicData.Inventory.Where(x =>
                x.MedicineId.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0 ||
                x.MedicineName.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                dgv.Rows.Add(i.MedicineId, i.MedicineName, i.MedicineType, i.StockQty, i.ReorderQty);
            }
        }
    }
}