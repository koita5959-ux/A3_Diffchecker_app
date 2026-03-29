using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// DiffEngineの出力をRichTextBoxに色分け描画するレンダラー。
    /// </summary>
    public static class DiffRenderer
    {
        private static readonly Color ColorAdded = ColorTranslator.FromHtml("#E6FFE6");
        private static readonly Color ColorDeleted = ColorTranslator.FromHtml("#FFE6E6");
        private static readonly Color ColorModified = ColorTranslator.FromHtml("#FFFDE6");

        /// <summary>行番号なしの場合のスペース埋め（4桁＋コロン＋スペース = 6文字）</summary>
        private const string BlankLineNumber = "      ";

        /// <summary>
        /// 差分結果を左右のRichTextBoxに色分け描画する。
        /// </summary>
        /// <param name="rtbFile1">左パネル（ファイル1）のRichTextBox</param>
        /// <param name="rtbFile2">右パネル（ファイル2）のRichTextBox</param>
        /// <param name="diffs">差分結果のリスト</param>
        public static void Render(RichTextBox rtbFile1, RichTextBox rtbFile2, List<DiffLine> diffs)
        {
            rtbFile1.Clear();
            rtbFile2.Clear();

            foreach (var diff in diffs)
            {
                switch (diff.Status)
                {
                    case DiffStatus.Equal:
                        AppendLine(rtbFile1, FormatLineNumber(diff.LineNumber1) + diff.Content1, null);
                        AppendLine(rtbFile2, FormatLineNumber(diff.LineNumber2) + diff.Content2, null);
                        break;

                    case DiffStatus.Deleted:
                        AppendLine(rtbFile1, FormatLineNumber(diff.LineNumber1) + diff.Content1, ColorDeleted);
                        AppendLine(rtbFile2, BlankLineNumber + "---", ColorDeleted);
                        break;

                    case DiffStatus.Added:
                        AppendLine(rtbFile1, BlankLineNumber + "---", ColorAdded);
                        AppendLine(rtbFile2, FormatLineNumber(diff.LineNumber2) + diff.Content2, ColorAdded);
                        break;

                    case DiffStatus.Modified:
                        AppendLine(rtbFile1, FormatLineNumber(diff.LineNumber1) + diff.Content1, ColorModified);
                        AppendLine(rtbFile2, FormatLineNumber(diff.LineNumber2) + diff.Content2, ColorModified);
                        break;
                }
            }
        }

        /// <summary>
        /// 行番号を「4桁右揃え: 」の書式で返す。
        /// </summary>
        private static string FormatLineNumber(int? lineNumber)
        {
            if (lineNumber == null) return BlankLineNumber;
            return $"{lineNumber,4}: ";
        }

        /// <summary>
        /// RichTextBoxに1行を追加し、背景色を設定する。
        /// </summary>
        private static void AppendLine(RichTextBox rtb, string text, Color? backColor)
        {
            int start = rtb.TextLength;
            if (start > 0)
            {
                rtb.AppendText("\n");
                start = rtb.TextLength;
            }
            rtb.AppendText(text);
            int end = rtb.TextLength;

            if (backColor != null)
            {
                rtb.Select(start, end - start);
                rtb.SelectionBackColor = backColor.Value;
                rtb.Select(end, 0);
            }
        }

        /// <summary>
        /// 左右のRichTextBoxの垂直スクロールを連動させる。
        /// </summary>
        /// <param name="rtbFile1">左パネルのRichTextBox</param>
        /// <param name="rtbFile2">右パネルのRichTextBox</param>
        public static void SetupScrollSync(RichTextBox rtbFile1, RichTextBox rtbFile2)
        {
            rtbFile1.VScroll += (s, e) => SyncScroll(rtbFile1, rtbFile2);
            rtbFile2.VScroll += (s, e) => SyncScroll(rtbFile2, rtbFile1);
        }

        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int SB_VERT = 1;
        private const int WM_VSCROLL = 0x0115;
        private const int SB_THUMBPOSITION = 4;

        private static bool _syncing;

        /// <summary>
        /// ソース側のスクロール位置をターゲット側に反映する。
        /// </summary>
        private static void SyncScroll(RichTextBox source, RichTextBox target)
        {
            if (_syncing) return;
            _syncing = true;
            try
            {
                int pos = GetScrollPos(source.Handle, SB_VERT);
                SetScrollPos(target.Handle, SB_VERT, pos, true);
                SendMessage(target.Handle, WM_VSCROLL, (IntPtr)(SB_THUMBPOSITION | (pos << 16)), IntPtr.Zero);
            }
            finally
            {
                _syncing = false;
            }
        }
    }
}
