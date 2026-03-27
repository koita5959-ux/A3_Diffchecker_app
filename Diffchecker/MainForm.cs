using DesktopKit.Common;
using System.Windows.Forms;
using System.Drawing;

namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// Diffchecker（ファイル差分レポーター）のメインフォーム。
    /// </summary>
    public class MainForm : BaseForm
    {
        private Label lblFile1 = null!;
        private TextBox txtFile1 = null!;
        private Button btnBrowse1 = null!;
        private Label lblFile2 = null!;
        private TextBox txtFile2 = null!;
        private Button btnBrowse2 = null!;
        private Button btnCompare = null!;
        private SplitContainer splitContainer = null!;
        private Label lblPanel1Title = null!;
        private RichTextBox rtbFile1 = null!;
        private Label lblPanel2Title = null!;
        private RichTextBox rtbFile2 = null!;
        private Button btnExportReport = null!;

        /// <summary>
        /// MainFormのコンストラクタ。
        /// </summary>
        public MainForm()
        {
            ComponentName = "Diffchecker";
            InitializeControls();
        }

        private void InitializeControls()
        {
            // --- 上部パネル: ファイル選択 + 比較ボタン ---
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                Padding = new Padding(10, 10, 10, 5)
            };

            // ファイル1
            lblFile1 = new Label
            {
                Text = "ファイル1:",
                Location = new Point(10, 12),
                AutoSize = true
            };

            txtFile1 = new TextBox
            {
                ReadOnly = true,
                Location = new Point(90, 10),
                Size = new Size(600, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btnBrowse1 = new Button
            {
                Text = "参照",
                Location = new Point(700, 8),
                Size = new Size(60, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // ファイル2
            lblFile2 = new Label
            {
                Text = "ファイル2:",
                Location = new Point(10, 45),
                AutoSize = true
            };

            txtFile2 = new TextBox
            {
                ReadOnly = true,
                Location = new Point(90, 43),
                Size = new Size(600, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btnBrowse2 = new Button
            {
                Text = "参照",
                Location = new Point(700, 41),
                Size = new Size(60, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // 比較ボタン
            btnCompare = new Button
            {
                Text = "比較",
                Location = new Point(10, 73),
                Size = new Size(100, 28)
            };

            topPanel.Controls.AddRange(new Control[]
            {
                lblFile1, txtFile1, btnBrowse1,
                lblFile2, txtFile2, btnBrowse2,
                btnCompare
            });

            // --- 中央: SplitContainer ---
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 380
            };

            // Panel1: ファイル1
            lblPanel1Title = new Label
            {
                Text = "ファイル1",
                Dock = DockStyle.Top,
                Height = 22
            };

            rtbFile1 = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };

            splitContainer.Panel1.Controls.Add(rtbFile1);
            splitContainer.Panel1.Controls.Add(lblPanel1Title);

            // Panel2: ファイル2
            lblPanel2Title = new Label
            {
                Text = "ファイル2",
                Dock = DockStyle.Top,
                Height = 22
            };

            rtbFile2 = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };

            splitContainer.Panel2.Controls.Add(rtbFile2);
            splitContainer.Panel2.Controls.Add(lblPanel2Title);

            // --- 下部パネル: レポート出力ボタン ---
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                Padding = new Padding(10, 5, 10, 5)
            };

            btnExportReport = new Button
            {
                Text = "レポート出力",
                Location = new Point(10, 8),
                Size = new Size(120, 28)
            };

            bottomPanel.Controls.Add(btnExportReport);

            // --- フォームに追加 ---
            Controls.Add(splitContainer);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }
    }
}
