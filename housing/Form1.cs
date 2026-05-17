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
            "Data Source=localhost;Initial Catalog=HousingDB;Integrated Security=True;TrustServerCertificate=True;";

        // ═══════════════ LUXURY DARK PALETTE ═══════════════
        private static readonly Color C_BG = Color.FromArgb(10, 13, 22);
        private static readonly Color C_SURFACE = Color.FromArgb(17, 22, 38);
        private static readonly Color C_SURFACE2 = Color.FromArgb(22, 29, 50);
        private static readonly Color C_BORDER = Color.FromArgb(35, 45, 75);
        private static readonly Color C_GOLD = Color.FromArgb(197, 160, 80);
        private static readonly Color C_GOLD_LT = Color.FromArgb(235, 200, 120);
        private static readonly Color C_TEXT = Color.FromArgb(230, 225, 215);
        private static readonly Color C_TEXT2 = Color.FromArgb(110, 125, 160);
        private static readonly Color C_SUCCESS = Color.FromArgb(52, 199, 130);
        private static readonly Color C_DANGER = Color.FromArgb(230, 76, 76);
        private static readonly Color C_INPUT_BG = Color.FromArgb(13, 17, 30);

        // ── Nav ──
        private Panel navPanel;
        private Panel contentPanel;
        private Panel currentPanel;

        private Button btnNavAddRep, btnNavAddClient, btnNavAddUnit;
        private Button btnNavDelete, btnNavUpdate, btnNavViewUnits;
        private Button btnNavViewTours, btnNavSchedule, btnNavAgreement;
        private Button btnNavSearch, btnNavReports;

        // ── Tab fields ──
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

        // Schedule tour
        private TextBox txtTourClientId, txtTourUnitId, txtTourRepId, txtTourDate, txtTourTime;
        private GoldButton btnScheduleTour;
        private Label lblTourResult;

        // Agreement
        private TextBox txtAgrClientId, txtAgrUnitId, txtAgrRepId, txtAgrPrice, txtAgrDate;
        private GoldButton btnAddAgreement;
        private Label lblAgrResult;

        // Search
        private TextBox txtSrchStyle, txtSrchMinVal, txtSrchMaxVal, txtSrchLong, txtSrchLat, txtSrchRadius;
        private GoldButton btnSearch;
        private DataGridView gridSearch;

        // Reports
        private DataGridView gridReport;
        private GoldButton btnR1, btnR2, btnR3, btnR4, btnR5, btnR6;
        private Label lblReportTitle;

        public Form1()
        {
            InitUI();
        }

        // ═══════════════ MAIN SHELL ═══════════════
        private void InitUI()
        {
            this.Text = "PRESTIGE  ·  Property Marketplace";
            this.Size = new Size(1280, 780);
            this.MinimumSize = new Size(1100, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = C_BG;
            this.Font = new Font("Segoe UI", 9.5f);
            this.ForeColor = C_TEXT;

            // ── Sidebar ──
            navPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 240,
                BackColor = Color.FromArgb(13, 17, 30),
                Padding = new Padding(0, 20, 0, 20)
            };
            navPanel.Paint += (s, e) =>
            {
                using (var br = new LinearGradientBrush(
                    new Rectangle(navPanel.Width - 3, 0, 3, navPanel.Height),
                    Color.FromArgb(80, 197, 160, 80), Color.FromArgb(15, 197, 160, 80),
                    LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(br, navPanel.Width - 3, 0, 3, navPanel.Height);
            };

            navPanel.Controls.Add(new Label
            {
                Text = "◈  PRESTIGE",
                Font = new Font("Palatino Linotype", 16f, FontStyle.Bold),
                ForeColor = C_GOLD,
                Location = new Point(20, 15),
                AutoSize = true
            });
            navPanel.Controls.Add(new Label
            {
                Text = "Property Management",
                Font = new Font("Segoe UI", 7f),
                ForeColor = C_TEXT2,
                Location = new Point(22, 42),
                AutoSize = true
            });
            navPanel.Controls.Add(new Panel
            {
                Location = new Point(20, 70),
                Size = new Size(190, 1),
                BackColor = C_BORDER
            });

            // ── Nav group labels ──
            int y = 82;
            AddNavGroup("MANAGEMENT", ref y);
            btnNavAddRep = CreateNavButton("Add Representative", y); y += 46;
            btnNavAddClient = CreateNavButton("Add Client", y); y += 46;
            btnNavAddUnit = CreateNavButton("Add Unit", y); y += 46;
            btnNavDelete = CreateNavButton("Delete", y); y += 46;
            btnNavUpdate = CreateNavButton("Update", y); y += 52;

            AddNavGroup("OPERATIONS", ref y);
            btnNavSchedule = CreateNavButton("Schedule Tour", y); y += 46;
            btnNavAgreement = CreateNavButton("Record Agreement", y); y += 52;

            AddNavGroup("EXPLORE", ref y);
            btnNavSearch = CreateNavButton("Search Units", y); y += 46;
            btnNavViewUnits = CreateNavButton("All Units", y); y += 46;
            btnNavViewTours = CreateNavButton("All Tours", y); y += 52;

            AddNavGroup("ANALYTICS", ref y);
            btnNavReports = CreateNavButton("Inquiry Reports", y);

            contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = C_BG };

            btnNavAddRep.Click += (s, e) => SwitchToPanel(BuildTabAddRep());
            btnNavAddClient.Click += (s, e) => SwitchToPanel(BuildTabAddClient());
            btnNavAddUnit.Click += (s, e) => SwitchToPanel(BuildTabAddUnit());
            btnNavDelete.Click += (s, e) => SwitchToPanel(BuildTabDelete());
            btnNavUpdate.Click += (s, e) => SwitchToPanel(BuildTabUpdate());
            btnNavSchedule.Click += (s, e) => SwitchToPanel(BuildTabScheduleTour());
            btnNavAgreement.Click += (s, e) => SwitchToPanel(BuildTabAgreement());
            btnNavSearch.Click += (s, e) => SwitchToPanel(BuildTabSearch());
            btnNavViewUnits.Click += (s, e) => { SwitchToPanel(BuildTabViewUnits()); LoadUnits(); };
            btnNavViewTours.Click += (s, e) => { SwitchToPanel(BuildTabViewTours()); LoadTours(); };
            btnNavReports.Click += (s, e) => SwitchToPanel(BuildTabReports());

            SwitchToPanel(BuildTabAddRep());

            this.Controls.Add(contentPanel);
            this.Controls.Add(navPanel);
        }

        private void AddNavGroup(string title, ref int y)
        {
            y += 6;
            navPanel.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 6.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 90, 130),
                Location = new Point(18, y),
                AutoSize = true
            });
            y += 16;
        }

        // ═══════════════ NAV HELPERS ═══════════════
        private Button CreateNavButton(string text, int yPos)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(10, yPos),
                Size = new Size(218, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(17, 22, 38),
                ForeColor = C_TEXT2,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Paint += (s, e) =>
            {
                using (var pen = new Pen(C_GOLD, 2))
                    e.Graphics.DrawLine(pen, 0, 0, 0, btn.Height);
                if (btn.ClientRectangle.Contains(btn.PointToClient(Cursor.Position)))
                {
                    using (var br = new LinearGradientBrush(btn.ClientRectangle,
                        Color.FromArgb(25, 197, 160, 80), Color.Transparent,
                        LinearGradientMode.Horizontal))
                        e.Graphics.FillRectangle(br, btn.ClientRectangle);
                }
            };
            btn.MouseEnter += (s, e) => { btn.BackColor = Color.FromArgb(25, 40, 70); btn.ForeColor = C_GOLD_LT; btn.Invalidate(); };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.FromArgb(17, 22, 38); btn.ForeColor = C_TEXT2; btn.Invalidate(); };
            navPanel.Controls.Add(btn);
            return btn;
        }

        private void SwitchToPanel(Panel newPanel)
        {
            if (currentPanel != null) contentPanel.Controls.Remove(currentPanel);
            currentPanel = newPanel;
            currentPanel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(currentPanel);
        }

        // ═══════════════ FACTORY HELPERS ═══════════════
        private Panel MakeCard(int x, int y, int w, int h)
        {
            var p = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = C_SURFACE };
            p.Paint += (s, e) =>
            {
                using (var b = new SolidBrush(C_GOLD))
                    e.Graphics.FillRectangle(b, 0, 0, 3, p.Height);
                using (var pen = new Pen(C_BORDER, 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
            return p;
        }

        private Label MakeSectionTitle(string text, int x, int y) => new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Palatino Linotype", 11f, FontStyle.Bold),
            ForeColor = C_GOLD
        };

        private Label MakeFieldLabel(string text, int x, int y) => new Label
        {
            Text = text.ToUpper(),
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = C_TEXT2
        };

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
            tb.Enter += (s, e) => tb.BackColor = Color.FromArgb(20, 26, 46);
            tb.Leave += (s, e) => tb.BackColor = C_INPUT_BG;
            return tb;
        }

        private GoldButton MakeBtn(string text, int x, int y, bool danger = false) =>
            new GoldButton(danger) { Text = text, Location = new Point(x, y), Width = 220, Height = 40 };

        private Label MakeResult(int x, int y) => new Label
        {
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = C_TEXT2
        };

        private void ShowResult(Label lbl, bool ok, string msg)
        {
            lbl.Text = (ok ? "✔   " : "✖   ") + msg;
            lbl.ForeColor = ok ? C_SUCCESS : C_DANGER;
        }

        private SqlConnection GetConn() => new SqlConnection(ConnStr);

        // ─── Layout helper ───
        private void AddRow(Panel card, string label, int x, int y, out TextBox tb)
        {
            card.Controls.Add(MakeFieldLabel(label, x, y));
            tb = MakeBox(x, y + 18, 300);
            card.Controls.Add(tb);
        }

        // ═══════════════ PANEL 1 – ADD REP ═══════════════
        private Panel BuildTabAddRep()
        {
            var panel = new Panel { BackColor = C_BG };
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
            panel.Controls.Add(card);
            panel.Controls.Add(lblRepResult);
            return panel;
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

        // ═══════════════ PANEL 2 – ADD CLIENT ═══════════════
        private Panel BuildTabAddClient()
        {
            var panel = new Panel { BackColor = C_BG };
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
            panel.Controls.Add(card);
            panel.Controls.Add(lblClientResult);
            return panel;
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

        // ═══════════════ PANEL 3 – ADD UNIT ═══════════════
        private Panel BuildTabAddUnit()
        {
            var panel = new Panel { BackColor = C_BG };
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
            panel.Controls.Add(card);
            panel.Controls.Add(lblUnitResult);
            return panel;
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

        // ═══════════════ PANEL 4 – DELETE ═══════════════
        private Panel BuildTabDelete()
        {
            var panel = new Panel { BackColor = C_BG };

            var c1 = MakeCard(30, 22, 500, 158);
            c1.Controls.Add(MakeSectionTitle("◈  Delete Client", 18, 16));
            c1.Controls.Add(MakeFieldLabel("Client ID", 18, 56));
            txtDelClientId = MakeBox(18, 74, 140);
            c1.Controls.Add(txtDelClientId);
            btnDelClient = MakeBtn("Delete Client", 180, 70, true);
            btnDelClient.Width = 180;
            btnDelClient.Click += BtnDelClient_Click;
            c1.Controls.Add(btnDelClient);

            var c2 = MakeCard(30, 200, 500, 158);
            c2.Controls.Add(MakeSectionTitle("◈  Delete Unit", 18, 16));
            c2.Controls.Add(MakeFieldLabel("Unit ID", 18, 56));
            txtDelUnitId = MakeBox(18, 74, 140);
            c2.Controls.Add(txtDelUnitId);
            btnDelUnit = MakeBtn("Delete Unit", 180, 70, true);
            btnDelUnit.Width = 180;
            btnDelUnit.Click += BtnDelUnit_Click;
            c2.Controls.Add(btnDelUnit);

            lblDelResult = MakeResult(30, 374);
            panel.Controls.Add(c1);
            panel.Controls.Add(c2);
            panel.Controls.Add(lblDelResult);
            return panel;
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
                var cmd1 = new SqlCommand("DELETE FROM dbo.client_tour WHERE tour_id IN (SELECT tour_id FROM dbo.tour WHERE unit_id=@id)", con);
                cmd1.Parameters.AddWithValue("@id", id); cmd1.ExecuteNonQuery();
                var cmd2 = new SqlCommand("DELETE FROM dbo.tour WHERE unit_id=@id", con);
                cmd2.Parameters.AddWithValue("@id", id); cmd2.ExecuteNonQuery();
                var cmd3 = new SqlCommand("DELETE FROM dbo.housing_unit WHERE unit_id=@id AND status!='sold'", con);
                cmd3.Parameters.AddWithValue("@id", id);
                int rows = cmd3.ExecuteNonQuery();
                ShowResult(lblDelResult, rows > 0, rows > 0 ? $"Unit #{id} removed." : "Unit not found or already sold.");
            }
            catch (Exception ex) { ShowResult(lblDelResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ PANEL 5 – UPDATE ═══════════════
        private Panel BuildTabUpdate()
        {
            var panel = new Panel { BackColor = C_BG };

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
            panel.Controls.Add(c1);
            panel.Controls.Add(c2);
            panel.Controls.Add(lblUpdResult);
            return panel;
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

        // ═══════════════ PANEL 6 – SCHEDULE TOUR ═══════════════
        private Panel BuildTabScheduleTour()
        {
            var panel = new Panel { BackColor = C_BG };
            var card = MakeCard(30, 22, 520, 440);
            card.Controls.Add(MakeSectionTitle("◈  Schedule Property Tour", 18, 18));

            AddRow(card, "Client ID", 18, 58, out txtTourClientId);
            AddRow(card, "Unit ID", 18, 114, out txtTourUnitId);
            AddRow(card, "Representative ID", 18, 170, out txtTourRepId);
            AddRow(card, "Tour Date  (YYYY-MM-DD)", 18, 226, out txtTourDate);
            AddRow(card, "Tour Time  (HH:MM)", 18, 282, out txtTourTime);

            // Hint label
            card.Controls.Add(new Label
            {
                Text = "A tour record will be created and linked to the client.",
                Location = new Point(18, 340),
                Size = new Size(460, 18),
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = C_TEXT2
            });

            btnScheduleTour = MakeBtn("Schedule Tour", 18, 368);
            btnScheduleTour.Click += BtnScheduleTour_Click;
            card.Controls.Add(btnScheduleTour);

            lblTourResult = MakeResult(30, 480);
            panel.Controls.Add(card);
            panel.Controls.Add(lblTourResult);
            return panel;
        }

        private void BtnScheduleTour_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                int clientId = int.Parse(txtTourClientId.Text.Trim());
                int unitId = int.Parse(txtTourUnitId.Text.Trim());
                int repId = int.Parse(txtTourRepId.Text.Trim());
                DateTime date = DateTime.Parse(txtTourDate.Text.Trim());
                TimeSpan time = TimeSpan.Parse(txtTourTime.Text.Trim());

                con = GetConn(); con.Open();

                // Use sp_RequestTour which handles tour + client_tour + request_date
                var cmd = new SqlCommand("sp_RequestTour", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@tourDate", date);
                cmd.Parameters.AddWithValue("@tourTime", time);
                cmd.Parameters.AddWithValue("@unitId", unitId);
                cmd.Parameters.AddWithValue("@represId", repId);
                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.ExecuteNonQuery();

                ShowResult(lblTourResult, true, $"Tour scheduled on {date:dd MMM yyyy} at {time:hh\\:mm}.");
                txtTourClientId.Clear(); txtTourUnitId.Clear(); txtTourRepId.Clear();
                txtTourDate.Clear(); txtTourTime.Clear();
            }
            catch (Exception ex) { ShowResult(lblTourResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ PANEL 7 – RECORD AGREEMENT ═══════════════
        private Panel BuildTabAgreement()
        {
            var panel = new Panel { BackColor = C_BG };
            var card = MakeCard(30, 22, 520, 420);
            card.Controls.Add(MakeSectionTitle("◈  Record Legal Agreement", 18, 18));

            AddRow(card, "Client ID", 18, 58, out txtAgrClientId);
            AddRow(card, "Unit ID", 18, 114, out txtAgrUnitId);
            AddRow(card, "Representative ID", 18, 170, out txtAgrRepId);
            AddRow(card, "Agreed Price (EGP)", 18, 226, out txtAgrPrice);
            AddRow(card, "Transfer Date  (YYYY-MM-DD)", 18, 282, out txtAgrDate);

            card.Controls.Add(new Label
            {
                Text = "Recording an agreement will mark the unit status as 'sold'.",
                Location = new Point(18, 340),
                Size = new Size(460, 18),
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = C_TEXT2
            });

            btnAddAgreement = MakeBtn("Record Agreement", 18, 362);
            btnAddAgreement.Click += BtnAddAgreement_Click;
            card.Controls.Add(btnAddAgreement);

            lblAgrResult = MakeResult(30, 458);
            panel.Controls.Add(card);
            panel.Controls.Add(lblAgrResult);
            return panel;
        }

        private void BtnAddAgreement_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                int clientId = int.Parse(txtAgrClientId.Text.Trim());
                int unitId = int.Parse(txtAgrUnitId.Text.Trim());
                int repId = int.Parse(txtAgrRepId.Text.Trim());
                decimal price = decimal.Parse(txtAgrPrice.Text.Trim());
                DateTime date = DateTime.Parse(txtAgrDate.Text.Trim());

                con = GetConn(); con.Open();

                // Use sp_FinalizeAgreement (final_price column, auto-marks unit sold)
                var cmd = new SqlCommand("sp_FinalizeAgreement", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@finalPrice", price);
                cmd.Parameters.AddWithValue("@transferDate", date);
                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.Parameters.AddWithValue("@unitId", unitId);
                cmd.Parameters.AddWithValue("@represId", repId);
                cmd.ExecuteNonQuery();

                ShowResult(lblAgrResult, true, $"Agreement recorded. Unit #{unitId} marked as sold.");
                txtAgrClientId.Clear(); txtAgrUnitId.Clear(); txtAgrRepId.Clear();
                txtAgrPrice.Clear(); txtAgrDate.Clear();
            }
            catch (Exception ex) { ShowResult(lblAgrResult, false, ex.Message); }
            finally { con?.Close(); }
        }

        // ═══════════════ PANEL 8 – SEARCH UNITS ═══════════════
        private Panel BuildTabSearch()
        {
            var panel = new Panel { BackColor = C_BG };

            // Filter card
            var card = MakeCard(24, 16, 960, 130);
            card.Controls.Add(MakeSectionTitle("◈  Search Housing Units", 18, 10));

            // Row 1: style + price range
            card.Controls.Add(MakeFieldLabel("Architectural Style", 18, 44));
            txtSrchStyle = MakeBox(18, 62, 200);
            card.Controls.Add(txtSrchStyle);

            card.Controls.Add(MakeFieldLabel("Min Price (EGP)", 236, 44));
            txtSrchMinVal = MakeBox(236, 62, 150);
            card.Controls.Add(txtSrchMinVal);

            card.Controls.Add(MakeFieldLabel("Max Price (EGP)", 404, 44));
            txtSrchMaxVal = MakeBox(404, 62, 150);
            card.Controls.Add(txtSrchMaxVal);

            // Row 1 cont: location
            card.Controls.Add(MakeFieldLabel("Longitude", 572, 44));
            txtSrchLong = MakeBox(572, 62, 110);
            card.Controls.Add(txtSrchLong);

            card.Controls.Add(MakeFieldLabel("Latitude", 700, 44));
            txtSrchLat = MakeBox(700, 62, 110);
            card.Controls.Add(txtSrchLat);

            card.Controls.Add(MakeFieldLabel("Radius (km)", 828, 44));
            txtSrchRadius = MakeBox(828, 62, 90);
            card.Controls.Add(txtSrchRadius);

            btnSearch = MakeBtn("Search", 18, 93);
            btnSearch.Width = 160;
            btnSearch.Click += BtnSearch_Click;
            card.Controls.Add(btnSearch);

            // Grid
            gridSearch = MakeGrid();
            gridSearch.AutoGenerateColumns = false;
            gridSearch.Columns.Add(MakeCol("unit_id", "Unit ID", 70));
            gridSearch.Columns.Add(MakeCol("architectural_style", "Style", 160));
            gridSearch.Columns.Add(MakeCol("market_valuation", "Valuation", 120));
            gridSearch.Columns.Add(MakeCol("status", "Status", 110));
            gridSearch.Columns.Add(MakeCol("longitude", "Long.", 90));
            gridSearch.Columns.Add(MakeCol("latitude", "Lat.", 90));
            gridSearch.Columns.Add(MakeCol("repres_id", "Rep ID", 70));
            gridSearch.Location = new Point(24, 158);
            gridSearch.Size = new Size(960, 430);

            panel.Controls.Add(card);
            panel.Controls.Add(gridSearch);
            return panel;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();

                // Build dynamic query
                string sql = @"
                    SELECT unit_id, architectural_style, market_valuation, status,
                           longitude, latitude, repres_id
                    FROM   dbo.housing_unit
                    WHERE  1=1";

                var cmd = new SqlCommand();
                cmd.Connection = con;

                if (!string.IsNullOrWhiteSpace(txtSrchStyle.Text))
                {
                    sql += " AND architectural_style LIKE @style";
                    cmd.Parameters.AddWithValue("@style", "%" + txtSrchStyle.Text.Trim() + "%");
                }
                if (decimal.TryParse(txtSrchMinVal.Text, out decimal minV))
                {
                    sql += " AND market_valuation >= @minV";
                    cmd.Parameters.AddWithValue("@minV", minV);
                }
                if (decimal.TryParse(txtSrchMaxVal.Text, out decimal maxV))
                {
                    sql += " AND market_valuation <= @maxV";
                    cmd.Parameters.AddWithValue("@maxV", maxV);
                }
                // Bounding-box proximity search (±radius converted to degrees ≈ km/111)
                if (decimal.TryParse(txtSrchLong.Text, out decimal lon) &&
                    decimal.TryParse(txtSrchLat.Text, out decimal lat) &&
                    decimal.TryParse(txtSrchRadius.Text, out decimal km))
                {
                    decimal deg = km / 111m;
                    sql += " AND longitude BETWEEN @lon1 AND @lon2 AND latitude BETWEEN @lat1 AND @lat2";
                    cmd.Parameters.AddWithValue("@lon1", lon - deg);
                    cmd.Parameters.AddWithValue("@lon2", lon + deg);
                    cmd.Parameters.AddWithValue("@lat1", lat - deg);
                    cmd.Parameters.AddWithValue("@lat2", lat + deg);
                }

                sql += " ORDER BY market_valuation";
                cmd.CommandText = sql;

                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gridSearch.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Search Error"); }
            finally { con?.Close(); }
        }

        // ═══════════════ PANEL 9 – ALL UNITS ═══════════════
        private Panel BuildTabViewUnits()
        {
            var panel = new Panel { BackColor = C_BG };
            var top = new Panel { Location = new Point(24, 16), Size = new Size(960, 48), BackColor = Color.Transparent };
            top.Controls.Add(MakeSectionTitle("◈  All Housing Units", 0, 10));
            var btn = MakeBtn("⟳  Refresh", 800, 4); btn.Width = 150;
            btn.Click += (s, e) => LoadUnits();
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
            gridUnits.Size = new Size(960, 520);
            panel.Controls.Add(top);
            panel.Controls.Add(gridUnits);
            return panel;
        }

        private void LoadUnits()
        {
            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var da = new SqlDataAdapter(new SqlCommand(
                    "SELECT unit_id, architectural_style, market_valuation, status, repres_id " +
                    "FROM dbo.housing_unit ORDER BY unit_id", con));
                var dt = new DataTable(); da.Fill(dt);
                gridUnits.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            finally { con?.Close(); }
        }

        // ═══════════════ PANEL 10 – ALL TOURS ═══════════════
        private Panel BuildTabViewTours()
        {
            var panel = new Panel { BackColor = C_BG };
            var top = new Panel { Location = new Point(24, 16), Size = new Size(960, 48), BackColor = Color.Transparent };
            top.Controls.Add(MakeSectionTitle("◈  Scheduled Tours", 0, 10));
            var btn = MakeBtn("⟳  Refresh", 800, 4); btn.Width = 150;
            btn.Click += (s, e) => LoadTours();
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
            gridTours.Size = new Size(960, 520);
            panel.Controls.Add(top);
            panel.Controls.Add(gridTours);
            return panel;
        }

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
                var dt = new DataTable(); da.Fill(dt);
                gridTours.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            finally { con?.Close(); }
        }

        // ═══════════════ PANEL 11 – INQUIRY REPORTS ═══════════════
        private Panel BuildTabReports()
        {
            var panel = new Panel { BackColor = C_BG };

            // Title
            panel.Controls.Add(MakeSectionTitle("◈  Inquiry Reports", 24, 18));

            // Button bar
            int bx = 24, by = 55, bw = 200, bh = 38, gap = 8;

            btnR1 = MakeReportBtn("Q1  Most Viewed Style", bx, by); bx += bw + gap;
            btnR2 = MakeReportBtn("Q2  No Tours Last Month", bx, by); bx += bw + gap;
            btnR3 = MakeReportBtn("Q3  Top Rep by Value", bx, by); bx += bw + gap;
            btnR4 = MakeReportBtn("Q4  Inactive Clients", bx, by); bx += bw + gap;
            btnR5 = MakeReportBtn("Q5  Units per Rep Last Month", bx, by); bx += bw + gap;
            btnR6 = MakeReportBtn("Q6  Client Tour Count", bx, by);

            btnR1.Click += (s, e) => RunReport(1);
            btnR2.Click += (s, e) => RunReport(2);
            btnR3.Click += (s, e) => RunReport(3);
            btnR4.Click += (s, e) => RunReport(4);
            btnR5.Click += (s, e) => RunReport(5);
            btnR6.Click += (s, e) => RunReport(6);

            panel.Controls.Add(btnR1); panel.Controls.Add(btnR2);
            panel.Controls.Add(btnR3); panel.Controls.Add(btnR4);
            panel.Controls.Add(btnR5); panel.Controls.Add(btnR6);

            // Description label
            lblReportTitle = new Label
            {
                Location = new Point(24, 104),
                Size = new Size(1000, 20),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = C_TEXT2,
                Text = "Select a query above to run the report."
            };
            panel.Controls.Add(lblReportTitle);

            // Results grid
            gridReport = MakeGrid();
            gridReport.Location = new Point(24, 130);
            gridReport.Size = new Size(1150, 500);
            panel.Controls.Add(gridReport);

            return panel;
        }

        private GoldButton MakeReportBtn(string text, int x, int y)
        {
            return new GoldButton(false)
            {
                Text = text,
                Location = new Point(x, y),
                Width = 195,
                Height = 38,
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold)
            };
        }

        private void RunReport(int queryNum)
        {
            // Descriptions shown above the grid
            string[] descs = {
                "Q1 · The architectural style(s) with the highest number of tour requests.",
                "Q2 · Housing units that had zero tour requests scheduled last month.",
                "Q3 · The representative with the highest total value of finalized agreements last month.",
                "Q4 · Clients who registered but did not request any property tour last month.",
                "Q5 · Available housing units managed by each representative last month.",
                "Q6 · Each client's contact information and total number of tours attended."
            };
            lblReportTitle.Text = descs[queryNum - 1];

            // SQL for each inquiry
            string[] queries = {
                // Q1 – Most viewed style (max tour count)
                @"SELECT TOP 1 WITH TIES u.architectural_style,
                         COUNT(*) AS tour_requests
                  FROM   dbo.tour t
                  JOIN   dbo.housing_unit u ON t.unit_id = u.unit_id
                  GROUP  BY u.architectural_style
                  ORDER  BY tour_requests DESC",

                // Q2 – Units with no tours last month
                @"SELECT u.unit_id, u.architectural_style, u.market_valuation, u.status
                  FROM   dbo.housing_unit u
                  WHERE  u.unit_id NOT IN (
                      SELECT DISTINCT unit_id FROM dbo.tour
                      WHERE  tour_date >= DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE())-1, 0)
                        AND  tour_date <  DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE()),   0)
                  )
                  ORDER  BY u.unit_id",

                // Q3 – Rep with highest total agreement value last month
                @"SELECT TOP 1 WITH TIES r.repres_id, r.repres_name,
                         SUM(a.final_price) AS total_value
                  FROM   dbo.agreement a
                  JOIN   dbo.representative r ON a.repres_id = r.repres_id
                  WHERE  a.transfer_date >= DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE())-1, 0)
                    AND  a.transfer_date <  DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE()),   0)
                  GROUP  BY r.repres_id, r.repres_name
                  ORDER  BY total_value DESC",

                // Q4 – Clients with no tours last month
                @"SELECT c.client_id, c.name, c.phone, c.email
                  FROM   dbo.client c
                  WHERE  c.client_id NOT IN (
                      SELECT DISTINCT ct.client_id
                      FROM   dbo.client_tour ct
                      JOIN   dbo.tour t ON ct.tour_id = t.tour_id
                      WHERE  t.tour_date >= DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE())-1, 0)
                        AND  t.tour_date <  DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE()),   0)
                  )
                  ORDER  BY c.client_id",

                // Q5 – Available units per rep last month
                @"SELECT r.repres_id, r.repres_name,
                         u.unit_id, u.architectural_style, u.market_valuation
                  FROM   dbo.housing_unit u
                  JOIN   dbo.representative r ON u.repres_id = r.repres_id
                  WHERE  u.status = 'available'
                    AND  u.unit_id IN (
                        SELECT DISTINCT unit_id FROM dbo.tour
                        WHERE  tour_date >= DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE())-1, 0)
                          AND  tour_date <  DATEADD(MONTH, DATEDIFF(MONTH,0,GETDATE()),   0)
                    )
                  ORDER  BY r.repres_id, u.unit_id",

                // Q6 – Client contact info + total tour count (all time)
                @"SELECT c.client_id, c.name, c.phone, c.email, c.national_id,
                         COUNT(ct.tour_id) AS tours_attended
                  FROM   dbo.client c
                  LEFT JOIN dbo.client_tour ct ON c.client_id = ct.client_id
                  GROUP  BY c.client_id, c.name, c.phone, c.email, c.national_id
                  ORDER  BY tours_attended DESC"
            };

            SqlConnection con = null;
            try
            {
                con = GetConn(); con.Open();
                var da = new SqlDataAdapter(new SqlCommand(queries[queryNum - 1], con));
                var dt = new DataTable();
                da.Fill(dt);
                gridReport.DataSource = null;
                gridReport.Columns.Clear();
                gridReport.AutoGenerateColumns = true;
                gridReport.DataSource = dt;

                // Style auto-generated headers
                foreach (DataGridViewColumn col in gridReport.Columns)
                {
                    col.HeaderText = System.Globalization.CultureInfo.CurrentCulture
                        .TextInfo.ToTitleCase(col.HeaderText.Replace("_", " "));
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Report Error"); }
            finally { con?.Close(); }
        }

        // ═══════════════ SHARED GRID FACTORY ═══════════════
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
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 26, 44);
            g.ColumnHeadersDefaultCellStyle.ForeColor = C_GOLD;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeight = 40;
            g.DefaultCellStyle.BackColor = C_SURFACE;
            g.DefaultCellStyle.ForeColor = C_TEXT;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(40, 197, 160, 80);
            g.DefaultCellStyle.SelectionForeColor = C_GOLD_LT;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            g.DefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            g.RowTemplate.Height = 34;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 28, 48);
            g.AlternatingRowsDefaultCellStyle.ForeColor = C_TEXT;
            SetDoubleBuffered(g);
            return g;
        }

        private DataGridViewTextBoxColumn MakeCol(string prop, string header, int width) =>
            new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                Width = width,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
    }

    // ═══════════════════════════════════════════════════════
    //  CUSTOM CONTROLS
    // ═══════════════════════════════════════════════════════

    internal class GoldButton : Control
    {
        private bool _hover, _danger;
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
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);

            using (var b = new SolidBrush(_hover ? C_BG_HOV : C_BG_IDLE))
                g.FillRectangle(b, rc);

            Color accent = _danger ? (_hover ? C_DAN_LT : C_DANGER) : (_hover ? C_GOLD_LT : C_GOLD);
            Color accentDim = Color.FromArgb(40, accent);

            using (var pen = new Pen(accent, 1))
            {
                g.DrawLine(pen, rc.Left, rc.Top, rc.Left, rc.Bottom);
                g.DrawLine(pen, rc.Left, rc.Bottom, rc.Right, rc.Bottom);
            }
            using (var pen = new Pen(accentDim, 1))
            {
                g.DrawLine(pen, rc.Left, rc.Top, rc.Right, rc.Top);
                g.DrawLine(pen, rc.Right, rc.Top, rc.Right, rc.Bottom);
            }

            TextRenderer.DrawText(g, Text, Font, rc, accent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
