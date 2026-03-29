namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// 行単位のLCS（Longest Common Subsequence）ベースの差分検出エンジン。
    /// </summary>
    public static class DiffEngine
    {
        /// <summary>
        /// 2つのファイルの行配列を比較し、差分結果のリストを返す。
        /// </summary>
        /// <param name="lines1">ファイル1の行配列</param>
        /// <param name="lines2">ファイル2の行配列</param>
        /// <returns>差分結果のリスト</returns>
        public static List<DiffLine> Compare(string[] lines1, string[] lines2)
        {
            // LCSテーブル構築
            var lcsTable = BuildLcsTable(lines1, lines2);

            // バックトラックでEqual/Deleted/Addedを分類
            var rawDiffs = Backtrack(lcsTable, lines1, lines2);

            // Deleted+Added の連続をModifiedに変換
            var result = MergeToModified(rawDiffs);

            return result;
        }

        /// <summary>
        /// LCSのDPテーブルを構築する。
        /// </summary>
        private static int[,] BuildLcsTable(string[] lines1, string[] lines2)
        {
            int m = lines1.Length;
            int n = lines2.Length;
            var dp = new int[m + 1, n + 1];

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (lines1[i - 1] == lines2[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            return dp;
        }

        /// <summary>
        /// LCSテーブルをバックトラックし、Equal/Deleted/Addedに分類する。
        /// </summary>
        private static List<DiffLine> Backtrack(int[,] dp, string[] lines1, string[] lines2)
        {
            var result = new List<DiffLine>();
            int i = lines1.Length;
            int j = lines2.Length;
            int lineNum1 = lines1.Length;
            int lineNum2 = lines2.Length;

            while (i > 0 || j > 0)
            {
                if (i > 0 && j > 0 && lines1[i - 1] == lines2[j - 1])
                {
                    // Equal
                    result.Add(new DiffLine
                    {
                        LineNumber1 = lineNum1,
                        LineNumber2 = lineNum2,
                        Content1 = lines1[i - 1],
                        Content2 = lines2[j - 1],
                        Status = DiffStatus.Equal
                    });
                    i--;
                    j--;
                    lineNum1--;
                    lineNum2--;
                }
                else if (j > 0 && (i == 0 || dp[i, j - 1] >= dp[i - 1, j]))
                {
                    // Added（ファイル2にのみ存在）
                    result.Add(new DiffLine
                    {
                        LineNumber1 = null,
                        LineNumber2 = lineNum2,
                        Content1 = string.Empty,
                        Content2 = lines2[j - 1],
                        Status = DiffStatus.Added
                    });
                    j--;
                    lineNum2--;
                }
                else
                {
                    // Deleted（ファイル1にのみ存在）
                    result.Add(new DiffLine
                    {
                        LineNumber1 = lineNum1,
                        LineNumber2 = null,
                        Content1 = lines1[i - 1],
                        Content2 = string.Empty,
                        Status = DiffStatus.Deleted
                    });
                    i--;
                    lineNum1--;
                }
            }

            result.Reverse();
            return result;
        }

        /// <summary>
        /// 連続するDeleted行とAdded行の組を検出し、Modifiedに変換する。
        /// </summary>
        private static List<DiffLine> MergeToModified(List<DiffLine> diffs)
        {
            var result = new List<DiffLine>();
            int i = 0;

            while (i < diffs.Count)
            {
                // Deleted行の連続を収集
                var deletedRun = new List<DiffLine>();
                while (i < diffs.Count && diffs[i].Status == DiffStatus.Deleted)
                {
                    deletedRun.Add(diffs[i]);
                    i++;
                }

                // 直後のAdded行の連続を収集
                var addedRun = new List<DiffLine>();
                while (i < diffs.Count && diffs[i].Status == DiffStatus.Added)
                {
                    addedRun.Add(diffs[i]);
                    i++;
                }

                // Deleted と Added のペアをModifiedに変換
                int pairCount = Math.Min(deletedRun.Count, addedRun.Count);
                for (int p = 0; p < pairCount; p++)
                {
                    result.Add(new DiffLine
                    {
                        LineNumber1 = deletedRun[p].LineNumber1,
                        LineNumber2 = addedRun[p].LineNumber2,
                        Content1 = deletedRun[p].Content1,
                        Content2 = addedRun[p].Content2,
                        Status = DiffStatus.Modified
                    });
                }

                // ペアにならなかった余りはそのまま
                for (int p = pairCount; p < deletedRun.Count; p++)
                {
                    result.Add(deletedRun[p]);
                }
                for (int p = pairCount; p < addedRun.Count; p++)
                {
                    result.Add(addedRun[p]);
                }

                // Deleted/Added以外の行はそのまま追加
                if (deletedRun.Count == 0 && addedRun.Count == 0)
                {
                    result.Add(diffs[i]);
                    i++;
                }
            }

            return result;
        }
    }
}
