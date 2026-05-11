using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HousingApp
{
    public partial class Form1 : Form
    {
        private const string ConnStr =
            "Data Source=Hesham;Initial Catalog=HousingDB;Integrated Security=True;TrustServerCertificate=True;";

        // ═══════════════ LUXURY DARK PALETTE ═══════════════
        private static readonly Color C_BG = Color.FromArgb(10, 13, 22);   // Deep void
        private static readonly Color C_SURFACE = Color.FromArgb(17, 22, 38);   // Card base
        private static readonly Color C_SURFACE2 = Color.FromArgb(22, 29, 50);   // Card hover
        private static readonly Color C_BORDER = Color.FromArgb(35, 45, 75);   // Subtle line
        private static readonly Color C_GOLD = Color.FromArgb(197, 160, 80);  // Luxury gold
        private static readonly Color C_GOLD_LT = Color.FromArgb(235, 200, 120);  // Gold highlight
        private static readonly Color C_TEXT = Color.FromArgb(230, 225, 215);  // Warm white
        private static readonly Color C_TEXT2 = Color.FromArgb(110, 125, 160);  // Muted
        private static readonly Color C_SUCCESS = Color.FromArgb(52, 199, 130);
        private static readonly Color C_DANGER = Color.FromArgb(230, 76, 76);
        private static readonly Color C_INPUT_BG = Color.FromArgb(13, 17, 30);

        private TabControl tabs;

        private TextBox txtRepName, txtRepEmail, txtRepPhone, txtRepLicense;
        private GoldButton btnAddRep;
        private Label lblRepResult;

        private TextBox txtClientName, txtClientPhone, txtClientEmail, txtClientNID;
        private GoldButton btnAddClient;
        private Label lblClientResult;

        private TextBox txtStyle, txtLong, txtLat, txtVal, txtRepId;
        private GoldButton btnAddUnit;
        private Label lblUnitResult;

        private TextBox txtDelClientId, txtDelUnitId;
        private GoldButton btnDelClient, btnDelUnit;
        private Label lblDelResult;

        private TextBox txtUpdUnitId, txtUpdStatus, txtUpdRepId, txtUpdRepPhone;
        private GoldButton btnUpdStatus, btnUpdRepPhone;
        private Label lblUpdResult;

        private DataGridView gridUnits;
        private GoldButton btnViewUnits;

        private DataGridView gridTours;
        private GoldButton btnViewTours;

        public Form1()
        {
            InitUI();
        }

        // ═══════════════ MAIN SHELL ═══════════════
        private void InitUI()
        {
            this.Text = "PRESTIGE  ·  Property Marketplace";
            this.Size = new Size(940, 660);
            this.MinimumSize = new Size(860, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = C_BG;
            this.Font = new Font("Segoe UI", 9.5f);
            this.ForeColor = C_TEXT;

            // ── Header strip ──
            var header = new HeaderPanel();
            header.Dock = DockStyle.Top;
            header.Height = 68;

            var logoLbl = new Label
            {
                Text = "◈  PRESTIGE",
                Font = new Font("Palatino Linotype", 15f, FontStyle.Bold),
                ForeColor = C_GOLD,
                Location = new Point(26, 18),
                AutoSize = true
            };
            var subtitleLbl = new Label
            {
                Text = "RESIDENTIAL PROPERTY MARKETPLACE",
                Font = new Font("Segoe UI", 7f, FontStyle.Bold),
                ForeColor = C_TEXT2,
                Location = new Point(28, 44),
                AutoSize = true
            };
            header.Controls.Add(logoLbl);
            header.Controls.Add(subtitleLbl);

            // ── Tab control ──
            tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                DrawMode = TabDrawMode.OwnerDrawFixed,
                ItemSize = new Size(118, 40),
                SizeMode = TabSizeMode.Fixed,
                Appearance = TabAppearance.FlatButtons,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                BackColor = C_BG
            };
            tabs.DrawItem += DrawTab;

            tabs.TabPages.Add(BuildTabAddRep());
            tabs.TabPages.Add(BuildTabAddClient());
            tabs.TabPages.Add(BuildTabAddUnit());
            tabs.TabPages.Add(BuildTabDelete());
            tabs.TabPages.Add(BuildTabUpdate());
            tabs.TabPages.Add(BuildTabViewUnits());
            tabs.TabPages.Add(BuildTabViewTours());

            tabs.SelectedIndexChanged += (s, e) => {
                if (tabs.SelectedIndex == 5) this.BeginInvoke(new Action(LoadUnits));
                if (tabs.SelectedIndex == 6) this.BeginInvoke(new Action(LoadTours));
            };

            this.Controls.Add(tabs);
            this.Controls.Add(header);
        }

        // ═══════════════ CUSTOM TAB DRAWING ═══════════════
        private void DrawTab(object sender, DrawItemEventArgs e)
        {
            bool sel = (tabs.SelectedIndex == e.Index);
            Rectangle r = tabs.GetTabRect(e.Index);

            // Background
            Color bg = sel ? C_SURFACE2 : C_BG;
            using (var b = new SolidBrush(bg))
                e.Graphics.FillRectangle(b, r);

            // Gold underline for selected
            if (sel)
            {
                using (var pen = new Pen(C_GOLD, 2))
                    e.Graphics.DrawLine(pen, r.Left + 6, r.Bottom - 2, r.Right - 6, r.Bottom - 2);
            }

            // Text
            Color fg = sel ? C_GOLD_LT : C_TEXT2;
            var tf = sel ? FontStyle.Bold : FontStyle.Regular;
            TextRenderer.DrawText(e.Graphics, tabs.TabPages[e.Index].Text,
                new Font("Segoe UI", 8.5f, tf), r, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // ═══════════════ FACTORY HELPERS ═══════════════
        private Panel MakeCard(int x, int y, int w, int h)
        {
            var p = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = C_SURFACE
            };
            p.Paint += (s, e) => {
                // Gold left accent bar
                using (var b = new SolidBrush(C_GOLD))
                    e.Graphics.FillRectangle(b, new Rectangle(0, 0, 3, p.Height));
                // Border
                using (var pen = new Pen(C_BORDER, 1))
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, p.Width - 1, p.Height - 1));
            };
            return p;
        }

        private Label MakeSectionTitle(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Palatino Linotype", 11f, FontStyle.Bold),
                ForeColor = C_GOLD
            };
        }

        private Label MakeFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text.ToUpper(),
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = C_TEXT2
            };
        }

        private TextBox MakeBox(int x, int y, int w = 260)
        {
            var tb = new TextBox
            {
                Location = new Point(x, y),
                Width = w,
                Height = 32,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = C_INPUT_BG,
                ForeColor = C_TEXT,
                Font = new Font("Segoe UI", 10f)
            };
            // Highlight on focus
            tb.Enter += (s, e) => tb.BackColor = Color.FromArgb(20, 26, 46);
            tb.Leave += (s, e) => tb.BackColor = C_INPUT_BG;
            return tb;
        }

        private GoldButton MakeBtn(string text, int x, int y, bool danger = false)
        {
            return new GoldButton(danger)
            {
                Text = text,
                Location = new Point(x, y),
                Width = 220,
                Height = 40
            };
        }

        private Label MakeResult(int x, int y)
        {
            return new Label
            {
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = C_TEXT2
            };
        }

        private void ShowResult(Label lbl, bool ok, string msg)
        {
            lbl.Text = (ok ? "✔   " : "✖   ") + msg;
            lbl.ForeColor = ok ? C_SUCCESS : C_DANGER;
        }

        private SqlConnection GetConn() => new SqlConnection(ConnStr);

        // ═══════════════ TAB 1 – ADD REP ═══════════════
        private TabPage BuildTabAddRep()
        {
            var tab = new TabPage("Add Rep") { BackColor = C_BG };
            var card = MakeCard(30, 22, 460, 360);
            card.Controls.Add(MakeSectionTitle("◈  Add Representative", 18, 18));

            AddRow(card, "Full Name", 18, 58, out txtRepName);
            AddRow(card, "Email Address", 18, 114, out txtRepEmail);
            AddRow(card, "Phone Number", 18, 170, out txtRepPhone);
            AddRow(card, "License No.", 18, 226, out txtRepLicense);

            btnAddRep = MakeBtn("Add Representative", 18, 290);
            btnAddRep.Click += BtnAddRep_Click;
            card.Controls.Add(btnAddRep);

            lblRepResult = MakeResult(30, 398);
            tab.Controls.Add(card);
            tab.Controls.Add(lblRepResult);
            return tab;
        }

        private void BtnAddRep_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var cmd = new SqlCommand("sp_AddRepresentative", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@name", txtRepName.Text.Trim());
                cmd.Parameters.AddWithValue("@email", txtRepEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@phone", txtRepPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@license", txtRepLicense.Text.Trim());
                cmd.ExecuteNonQuery();
                ShowResult(lblRepResult, true, "Representative added successfully.");
                txtRepName.Clear(); txtRepEmail.Clear(); txtRepPhone.Clear(); txtRepLicense.Clear();
            }
            catch (Exception ex) { ShowResult(lblRepResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ TAB 2 – ADD CLIENT ═══════════════
        private TabPage BuildTabAddClient()
        {
            var tab = new TabPage("Add Client") { BackColor = C_BG };
            var card = MakeCard(30, 22, 460, 360);
            card.Controls.Add(MakeSectionTitle("◈  Register Client", 18, 18));

            AddRow(card, "Full Name", 18, 58, out txtClientName);
            AddRow(card, "Phone Number", 18, 114, out txtClientPhone);
            AddRow(card, "Email Address", 18, 170, out txtClientEmail);
            AddRow(card, "National ID", 18, 226, out txtClientNID);

            btnAddClient = MakeBtn("Register Client", 18, 290);
            btnAddClient.Click += BtnAddClient_Click;
            card.Controls.Add(btnAddClient);

            lblClientResult = MakeResult(30, 398);
            tab.Controls.Add(card);
            tab.Controls.Add(lblClientResult);
            return tab;
        }

        private void BtnAddClient_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var cmd = new SqlCommand("sp_AddClient", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@name", txtClientName.Text.Trim());
                cmd.Parameters.AddWithValue("@phone", txtClientPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@email", txtClientEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@nationalId", txtClientNID.Text.Trim());
                cmd.ExecuteNonQuery();
                ShowResult(lblClientResult, true, "Client registered successfully.");
                txtClientName.Clear(); txtClientPhone.Clear(); txtClientEmail.Clear(); txtClientNID.Clear();
            }
            catch (Exception ex) { ShowResult(lblClientResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ TAB 3 – ADD UNIT ═══════════════
        private TabPage BuildTabAddUnit()
        {
            var tab = new TabPage("Add Unit") { BackColor = C_BG };
            var card = MakeCard(30, 22, 460, 420);
            card.Controls.Add(MakeSectionTitle("◈  Add Housing Unit", 18, 18));

            AddRow(card, "Architectural Style", 18, 58, out txtStyle);
            AddRow(card, "Longitude", 18, 114, out txtLong);
            AddRow(card, "Latitude", 18, 170, out txtLat);
            AddRow(card, "Valuation (EGP)", 18, 226, out txtVal);
            AddRow(card, "Representative ID", 18, 282, out txtRepId);

            btnAddUnit = MakeBtn("Add Unit", 18, 345);
            btnAddUnit.Click += BtnAddUnit_Click;
            card.Controls.Add(btnAddUnit);

            lblUnitResult = MakeResult(30, 458);
            tab.Controls.Add(card);
            tab.Controls.Add(lblUnitResult);
            return tab;
        }

        private void BtnAddUnit_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var cmd = new SqlCommand("sp_AddHousingUnit", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@style", txtStyle.Text.Trim());
                cmd.Parameters.AddWithValue("@longitude", decimal.Parse(txtLong.Text));
                cmd.Parameters.AddWithValue("@latitude", decimal.Parse(txtLat.Text));
                cmd.Parameters.AddWithValue("@valuation", decimal.Parse(txtVal.Text));
                cmd.Parameters.AddWithValue("@represId", int.Parse(txtRepId.Text));
                cmd.ExecuteNonQuery();
                ShowResult(lblUnitResult, true, "Unit added successfully.");
                txtStyle.Clear(); txtLong.Clear(); txtLat.Clear(); txtVal.Clear(); txtRepId.Clear();
            }
            catch (Exception ex) { ShowResult(lblUnitResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ TAB 4 – DELETE ═══════════════
        private TabPage BuildTabDelete()
        {
            var tab = new TabPage("Delete") { BackColor = C_BG };

            // Card 1: Delete Client
            var c1 = MakeCard(30, 22, 500, 158);
            c1.Controls.Add(MakeSectionTitle("◈  Delete Client", 18, 16));
            c1.Controls.Add(MakeFieldLabel("Client ID", 18, 56));
            txtDelClientId = MakeBox(18, 74, 140);
            c1.Controls.Add(txtDelClientId);
            btnDelClient = MakeBtn("Delete Client", 180, 70, true);
            btnDelClient.Width = 180;
            btnDelClient.Click += BtnDelClient_Click;
            c1.Controls.Add(btnDelClient);

            // Card 2: Delete Unit
            var c2 = MakeCard(30, 200, 500, 158);
            c2.Controls.Add(MakeSectionTitle("◈  Delete Unit", 18, 16));
            c2.Controls.Add(MakeFieldLabel("Unit ID", 18, 56));
            txtDelUnitId = MakeBox(18, 74, 140);
            c2.Controls.Add(txtDelUnitId);
            btnDelUnit = MakeBtn("Delete Unit", 180, 70, true);
            btnDelUnit.Width = 180;
            btnDelUnit.Click += BtnDelUnit_Click;
            c2.Controls.Add(txtDelUnitId);
            c2.Controls.Add(btnDelUnit);

            lblDelResult = MakeResult(30, 374);
            tab.Controls.Add(c1);
            tab.Controls.Add(c2);
            tab.Controls.Add(lblDelResult);
            return tab;
        }

        private void BtnDelClient_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtDelClientId.Text, out int id))
            { ShowResult(lblDelResult, false, "Enter a valid Client ID."); return; }
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var c1 = new SqlCommand("DELETE FROM dbo.client_tour WHERE client_id=@id", con);
                c1.Parameters.AddWithValue("@id", id); c1.ExecuteNonQuery();
                var c2 = new SqlCommand("DELETE FROM dbo.client WHERE client_id=@id", con);
                c2.Parameters.AddWithValue("@id", id);
                int rows = c2.ExecuteNonQuery();
                ShowResult(lblDelResult, rows > 0, rows > 0 ? $"Client #{id} removed." : "Client not found.");
            }
            catch (Exception ex) { ShowResult(lblDelResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        private void BtnDelUnit_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtDelUnitId.Text, out int id))
            { ShowResult(lblDelResult, false, "Enter a valid Unit ID."); return; }
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var cmd = new SqlCommand("DELETE FROM dbo.housing_unit WHERE unit_id=@id AND status!='sold'", con);
                cmd.Parameters.AddWithValue("@id", id);
                int rows = cmd.ExecuteNonQuery();
                ShowResult(lblDelResult, rows > 0, rows > 0 ? $"Unit #{id} removed." : "Unit not found or already sold.");
            }
            catch (Exception ex) { ShowResult(lblDelResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ TAB 5 – UPDATE ═══════════════
        private TabPage BuildTabUpdate()
        {
            var tab = new TabPage("Update") { BackColor = C_BG };

            var c1 = MakeCard(30, 22, 580, 188);
            c1.Controls.Add(MakeSectionTitle("◈  Update Unit Status", 18, 16));
            c1.Controls.Add(MakeFieldLabel("Unit ID", 18, 56));
            txtUpdUnitId = MakeBox(18, 74, 100);
            c1.Controls.Add(txtUpdUnitId);
            c1.Controls.Add(MakeFieldLabel("New Status  ( available · under offer · sold )", 138, 56));
            txtUpdStatus = MakeBox(138, 74, 240);
            c1.Controls.Add(txtUpdStatus);
            btnUpdStatus = MakeBtn("Update Status", 18, 122);
            btnUpdStatus.Click += BtnUpdStatus_Click;
            c1.Controls.Add(btnUpdStatus);

            var c2 = MakeCard(30, 230, 580, 170);
            c2.Controls.Add(MakeSectionTitle("◈  Update Rep Phone", 18, 16));
            c2.Controls.Add(MakeFieldLabel("Rep ID", 18, 56));
            txtUpdRepId = MakeBox(18, 74, 100);
            c2.Controls.Add(txtUpdRepId);
            c2.Controls.Add(MakeFieldLabel("New Phone Number", 138, 56));
            txtUpdRepPhone = MakeBox(138, 74, 240);
            c2.Controls.Add(txtUpdRepPhone);
            btnUpdRepPhone = MakeBtn("Update Phone", 18, 116);
            btnUpdRepPhone.Click += BtnUpdRepPhone_Click;
            c2.Controls.Add(btnUpdRepPhone);

            lblUpdResult = MakeResult(30, 416);
            tab.Controls.Add(c1);
            tab.Controls.Add(c2);
            tab.Controls.Add(lblUpdResult);
            return tab;
        }

        private void BtnUpdStatus_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var cmd = new SqlCommand("sp_UpdateUnitStatus", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@unitId", int.Parse(txtUpdUnitId.Text));
                cmd.Parameters.AddWithValue("@newStatus", txtUpdStatus.Text.Trim());
                cmd.ExecuteNonQuery();
                ShowResult(lblUpdResult, true, "Unit status updated successfully.");
            }
            catch (Exception ex) { ShowResult(lblUpdResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        private void BtnUpdRepPhone_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var cmd = new SqlCommand("UPDATE dbo.representative SET phone=@phone WHERE repres_id=@id", con);
                cmd.Parameters.AddWithValue("@phone", txtUpdRepPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@id", int.Parse(txtUpdRepId.Text));
                int rows = cmd.ExecuteNonQuery();
                ShowResult(lblUpdResult, rows > 0, rows > 0 ? "Representative phone updated." : "Rep not found.");
            }
            catch (Exception ex) { ShowResult(lblUpdResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ LUXURY GRID ═══════════════
        private static void SetDoubleBuffered(DataGridView g)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, g, new object[] { true });
        }

        private DataGridView MakeGrid()
        {
            var g = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = C_SURFACE,
                GridColor = C_BORDER,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false
            };
            // Header
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 26, 44);
            g.ColumnHeadersDefaultCellStyle.ForeColor = C_GOLD;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeight = 40;
            // Rows
            g.DefaultCellStyle.BackColor = C_SURFACE;
            g.DefaultCellStyle.ForeColor = C_TEXT;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(197, 160, 80, 40);
            g.DefaultCellStyle.SelectionForeColor = C_GOLD_LT;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            g.DefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            g.RowTemplate.Height = 34;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 28, 48);
            g.AlternatingRowsDefaultCellStyle.ForeColor = C_TEXT;
            SetDoubleBuffered(g);
            return g;
        }

        // ═══════════════ TAB 6 – ALL UNITS ═══════════════
        private TabPage BuildTabViewUnits()
        {
            var tab = new TabPage("All Units") { BackColor = C_BG };
            var top = new Panel { Location = new Point(24, 16), Size = new Size(868, 48), BackColor = Color.Transparent };
            top.Controls.Add(MakeSectionTitle("◈  All Housing Units", 0, 10));
            var btn = MakeBtn("⟳  Refresh", 686, 4); btn.Width = 160;
            btn.Click += BtnViewUnits_Click;
            btnViewUnits = btn;
            top.Controls.Add(btn);
            gridUnits = MakeGrid();
            gridUnits.AutoGenerateColumns = false;
            gridUnits.Columns.Add(MakeCol("unit_id", "Unit ID", 80));
            gridUnits.Columns.Add(MakeCol("architectural_style", "Style", 160));
            gridUnits.Columns.Add(MakeCol("market_valuation", "Valuation", 130));
            gridUnits.Columns.Add(MakeCol("status", "Status", 110));
            gridUnits.Columns.Add(MakeCol("repres_id", "Rep ID", 80));
            gridUnits.Location = new Point(24, 70);
            gridUnits.Size = new Size(868, 460);
            tab.Controls.Add(top);
            tab.Controls.Add(gridUnits);
            return tab;
        }

        private void BtnViewUnits_Click(object sender, EventArgs e) => LoadUnits();

        private void LoadUnits()
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var da = new SqlDataAdapter(new SqlCommand(
                    "SELECT unit_id, architectural_style, market_valuation, status, repres_id " +
                    "FROM dbo.housing_unit ORDER BY unit_id", con));
                var dt = new DataTable();
                da.Fill(dt);
                gridUnits.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            finally { con?.Close(); }
        }

        // ═══════════════ TAB 7 – TOURS ═══════════════
        private TabPage BuildTabViewTours()
        {
            var tab = new TabPage("Tours") { BackColor = C_BG };
            var top = new Panel { Location = new Point(24, 16), Size = new Size(868, 48), BackColor = Color.Transparent };
            top.Controls.Add(MakeSectionTitle("◈  Scheduled Tours", 0, 10));
            var btn = MakeBtn("⟳  Refresh", 686, 4); btn.Width = 160;
            btn.Click += BtnViewTours_Click;
            btnViewTours = btn;
            top.Controls.Add(btn);
            gridTours = MakeGrid();
            gridTours.AutoGenerateColumns = false;
            gridTours.Columns.Add(MakeCol("tour_id", "Tour ID", 70));
            gridTours.Columns.Add(MakeCol("tour_date", "Date", 100));
            gridTours.Columns.Add(MakeCol("tour_time", "Time", 80));
            gridTours.Columns.Add(MakeCol("client_name", "Client", 140));
            gridTours.Columns.Add(MakeCol("architectural_style", "Style", 120));
            gridTours.Columns.Add(MakeCol("unit_status", "Unit Status", 110));
            gridTours.Columns.Add(MakeCol("representative", "Rep", 140));
            gridTours.Location = new Point(24, 70);
            gridTours.Size = new Size(868, 460);
            tab.Controls.Add(top);
            tab.Controls.Add(gridTours);
            return tab;
        }

        private void BtnViewTours_Click(object sender, EventArgs e) => LoadTours();

        private void LoadTours()
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var da = new SqlDataAdapter(new SqlCommand(
                    "SELECT t.tour_id, t.tour_date, t.tour_time, c.name AS client_name, " +
                    "u.architectural_style, u.status AS unit_status, r.repres_name AS representative " +
                    "FROM dbo.tour t " +
                    "JOIN dbo.client_tour ct ON t.tour_id=ct.tour_id " +
                    "JOIN dbo.client c ON ct.client_id=c.client_id " +
                    "JOIN dbo.housing_unit u ON t.unit_id=u.unit_id " +
                    "JOIN dbo.representative r ON t.repres_id=r.repres_id " +
                    "ORDER BY t.tour_date", con));
                var dt = new DataTable();
                da.Fill(dt);
                gridTours.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            finally { con?.Close(); }
        }

        // ═══════════════ COLUMN HELPER ═══════════════
        private DataGridViewTextBoxColumn MakeCol(string prop, string header, int width)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                Width = width,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
        }

        // ═══════════════ LAYOUT HELPER ═══════════════
        private void AddRow(Panel card, string label, int x, int y, out TextBox tb)
        {
            card.Controls.Add(MakeFieldLabel(label, x, y));
            tb = MakeBox(x, y + 18, 300);
            card.Controls.Add(tb);
        }
    }

    // ═══════════════════════════════════════════════════════
    //  CUSTOM CONTROLS
    // ═══════════════════════════════════════════════════════

    /// <summary>Gradient header panel</summary>
    internal class HeaderPanel : Panel
    {
        private static readonly Color C_H1 = Color.FromArgb(14, 19, 34);
        private static readonly Color C_H2 = Color.FromArgb(10, 13, 22);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var br = new LinearGradientBrush(ClientRectangle, C_H1, C_H2, LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(br, ClientRectangle);

            // Bottom gold separator
            using (var p = new Pen(Color.FromArgb(197, 160, 80), 1))
                e.Graphics.DrawLine(p, 0, Height - 1, Width, Height - 1);
        }
    }

    /// <summary>Luxury gold-accented button with hover glow</summary>
    internal class GoldButton : Control
    {
        private bool _hover;
        private bool _danger;
        private static readonly Color C_GOLD = Color.FromArgb(197, 160, 80);
        private static readonly Color C_GOLD_LT = Color.FromArgb(235, 200, 120);
        private static readonly Color C_DANGER = Color.FromArgb(200, 60, 60);
        private static readonly Color C_DAN_LT = Color.FromArgb(230, 90, 90);
        private static readonly Color C_BG_IDLE = Color.FromArgb(17, 22, 38);
        private static readonly Color C_BG_HOV = Color.FromArgb(26, 33, 58);

        public GoldButton(bool danger = false)
        {
            _danger = danger;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            ForeColor = Color.White;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.DoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);

            // Fill
            Color fill = _hover ? C_BG_HOV : C_BG_IDLE;
            using (var b = new SolidBrush(fill))
                g.FillRectangle(b, rc);

            // Accent border (gold or danger)
            Color accent = _danger ? (_hover ? C_DAN_LT : C_DANGER) : (_hover ? C_GOLD_LT : C_GOLD);
            Color accentDim = Color.FromArgb(40, accent);

            // Left & bottom glowing edge
            using (var pen = new Pen(accent, 1))
            {
                g.DrawLine(pen, rc.Left, rc.Top, rc.Left, rc.Bottom); // left
                g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom); // bottom
            }
            using (var pen = new Pen(accentDim, 1))
            {
                g.DrawLine(pen, rc.Left, rc.Top, rc.Right, rc.Top);    // top
                g.DrawLine(pen, rc.Right, rc.Top, rc.Right, rc.Bottom); // right
            }

            // Text
            TextRenderer.DrawText(g, Text, Font, rc, accent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}