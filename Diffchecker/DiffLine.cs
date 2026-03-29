namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// 差分の種類を表す列挙型。
    /// </summary>
    public enum DiffStatus
    {
        /// <summary>両ファイルで同一の行</summary>
        Equal,
        /// <summary>ファイル2にのみ存在する行</summary>
        Added,
        /// <summary>ファイル1にのみ存在する行</summary>
        Deleted,
        /// <summary>両ファイルに存在するが内容が異なる行</summary>
        Modified
    }

    /// <summary>
    /// 差分結果の1行分のデータを保持するクラス。
    /// </summary>
    public class DiffLine
    {
        /// <summary>ファイル1での行番号（追加行はnull）</summary>
        public int? LineNumber1 { get; set; }

        /// <summary>ファイル2での行番号（削除行はnull）</summary>
        public int? LineNumber2 { get; set; }

        /// <summary>ファイル1の行内容（追加行は空文字列）</summary>
        public string Content1 { get; set; } = string.Empty;

        /// <summary>ファイル2の行内容（削除行は空文字列）</summary>
        public string Content2 { get; set; } = string.Empty;

        /// <summary>差分の種類</summary>
        public DiffStatus Status { get; set; }
    }
}
