using System.Text;
using UnityEngine;
using System;

public static class PlayerIdentity
{
    public static event Action<string> OnNameChanged;

    public static string Name { get; private set; }

    const string PrefKey = "player_name_v1";

    static bool _loaded;
    public static void EnsureLoaded()
    {
        if (_loaded) return;
        _loaded = true;
        Name = PlayerPrefs.GetString(PrefKey, string.Empty);
    }

    public static bool HasName => !string.IsNullOrWhiteSpace(Name);

    /// <summary>
    /// Validate tên theo quy tắc: độ dài [min..max], chỉ cho chữ cái, số, khoảng trắng, gạch dưới (_), gạch nối (-).
    /// Tự normalize: trim, rút gọn khoảng trắng, bỏ ký tự không hợp lệ, TitleCase nhẹ nếu cần.
    /// </summary>
    public static bool Validate(string raw, int minLen, int maxLen, out string normalized, out string error)
    {
        normalized = Normalize(raw, maxLen);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            error = $"Please enter your name ({minLen}-{maxLen} characters).";
            return false;
        }

        if (normalized.Length < minLen)
        {
            error = $"Name is too short (≥ {minLen}).";
            return false;
        }

        if (normalized.Length > maxLen)
        {
            // Normalize đã cắt cứng maxLen, nhánh này gần như không xảy ra
            error = $"Name is too long (≤ {maxLen}).";
            return false;
        }

        error = null;
        return true;
    }

    public static bool TrySetName(string raw, out string normalized, out string error)
    {
        // Mặc định min/max đồng bộ với UI recommend
        const int minLen = 3;
        const int maxLen = 16;

        if (!Validate(raw, minLen, maxLen, out normalized, out error))
            return false;

        // Không làm gì nếu không đổi
        if (string.Equals(Name, normalized, StringComparison.Ordinal))
            return true;

        Name = normalized;
        PlayerPrefs.SetString(PrefKey, Name);
        PlayerPrefs.Save();
        OnNameChanged?.Invoke(Name);
        return true;
    }

    static string Normalize(string raw, int maxLen)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;

        // 1) Trim 2 đầu
        string s = raw.Trim();

        // 2) Lọc ký tự: chữ cái, số, space, _ và -
        var sb = new StringBuilder(s.Length);
        foreach (char c in s)
        {
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '_' || c == '-')
                sb.Append(c);
        }

        // 3) Rút gọn khoảng trắng liên tiếp về 1 space
        for (int i = sb.Length - 1; i >= 1; i--)
        {
            if (sb[i] == ' ' && sb[i - 1] == ' ')
                sb.Remove(i, 1);
        }

        // 4) Cắt maxLen an toàn
        if (sb.Length > maxLen) sb.Length = maxLen;

        // 5) (Tuỳ chọn) TitleCase nhẹ: Viết hoa chữ cái đầu các từ
        for (int i = 0; i < sb.Length; i++)
        {
            if (i == 0 || sb[i - 1] == ' ')
                sb[i] = char.ToUpperInvariant(sb[i]);
        }

        return sb.ToString();
    }
}
