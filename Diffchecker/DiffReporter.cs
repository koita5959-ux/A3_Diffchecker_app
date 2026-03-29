using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// DiffEngineの出力をテキストレポートとして整形・出力するクラス。
    /// </summary>
    public static class DiffReporter
    {
        /// <summary>
        /// 差分結果をテキストレポートファイルとして保存する。
        /// </summary>
        /// <param name="filePath">保存先ファイルパス</param>
        /// <param name="file1Path">比較元ファイルのパス</param>
        /// <param name="file2Path">比較先ファイルのパス</param>
        /// <param name="diffs">差分結果のリスト</param>
        public static void Export(string filePath, string file1Path, string file2Path, List<DiffLine> diffs)
        {
            var sb = new StringBuilder();

            // ヘッダー
            sb.AppendLine("差分レポート");
            sb.AppendLine("============================================================");
            sb.AppendLine($"比較元: {file1Path}");
            sb.AppendLine($"比較先: {file2Path}");
            sb.AppendLine($"作成日: {DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine("============================================================");
            sb.AppendLine();

            // セクション1：全行マーク付き表示
            sb.AppendLine("--- 全行比較 ---");
            sb.AppendLine();

            foreach (var diff in diffs)
            {
                switch (diff.Status)
                {
                    case DiffStatus.Equal:
                        sb.AppendLine($"  {FormatLineNumber(diff.LineNumber1)}  {diff.Content1}");
                        break;
                    case DiffStatus.Modified:
                        sb.AppendLine($"~ {FormatLineNumber(diff.LineNumber1)}  {diff.Content1}  \u2192  {diff.Content2}");
                        break;
                    case DiffStatus.Deleted:
                        sb.AppendLine($"- {FormatLineNumber(diff.LineNumber1)}  {diff.Content1}");
                        break;
                    case DiffStatus.Added:
                        sb.AppendLine($"+ {FormatLineNumber(diff.LineNumber2)}  {diff.Content2}");
                        break;
                }
            }

            sb.AppendLine();

            // セクション2：差分サマリー
            sb.AppendLine("--- 差分サマリー ---");
            sb.AppendLine();

            var hasDiff = diffs.Any(d => d.Status != DiffStatus.Equal);
            if (!hasDiff)
            {
                sb.AppendLine("差分はありません。");
            }
            else
            {
                foreach (var diff in diffs)
                {
                    switch (diff.Status)
                    {
                        case DiffStatus.Modified:
                            sb.AppendLine($"[変更] 行 {diff.LineNumber1}:");
                            sb.AppendLine($"  元: {diff.Content1}");
                            sb.AppendLine($"  先: {diff.Content2}");
                            sb.AppendLine();
                            break;
                        case DiffStatus.Deleted:
                            sb.AppendLine($"[削除] 行 {diff.LineNumber1}:");
                            sb.AppendLine($"  元: {diff.Content1}");
                            sb.AppendLine();
                            break;
                        case DiffStatus.Added:
                            sb.AppendLine($"[追加] 行 {diff.LineNumber2}:");
                            sb.AppendLine($"  先: {diff.Content2}");
                            sb.AppendLine();
                            break;
                    }
                }
            }

            // 集計
            int modified = diffs.Count(d => d.Status == DiffStatus.Modified);
            int added = diffs.Count(d => d.Status == DiffStatus.Added);
            int deleted = diffs.Count(d => d.Status == DiffStatus.Deleted);
            int total = modified + added + deleted;

            sb.AppendLine("--- 集計 ---");
            sb.AppendLine($"変更: {modified}件 / 追加: {added}件 / 削除: {deleted}件 / 合計: {total}件");
            sb.AppendLine("============================================================");

            // UTF-8 BOM付き、CRLFで出力
            var content = sb.ToString().Replace("\r\n", "\n").Replace("\n", "\r\n");
            File.WriteAllText(filePath, content, new UTF8Encoding(true));
        }

        /// <summary>
        /// 行番号を「4桁右揃え:」の書式で返す。
        /// </summary>
        private static string FormatLineNumber(int? lineNumber)
        {
            if (lineNumber == null) return "    :";
            return $"{lineNumber,4}:";
        }
    }
}
