using System;
using System.Collections.Generic;
using System.IO;

namespace JetBrains.HabitatDetector
{
  public sealed class OsReleaseProperties
  {
    public const string AnsiColorKey = "ANSI_COLOR";
    public const string BugReportUrlKey = "BUG_REPORT_URL";
    public const string BuildIdKey = "BUILD_ID";
    public const string DocumentationUrlKey = "DOCUMENTATION_URL";
    public const string HomeUrlKey = "HOME_URL";
    public const string IdKey = "ID";
    public const string IdLikeKey = "ID_LIKE";
    public const string ImageIdKey = "IMAGE_ID";
    public const string ImageVersionKey = "IMAGE_VERSION";
    public const string LogoKey = "LOGO";
    public const string NameKey = "NAME";
    public const string PrettyNameKey = "PRETTY_NAME";
    public const string PrivacyPolicyUrlKey = "PRIVACY_POLICY_URL";
    public const string SupportUrlKey = "SUPPORT_URL";
    public const string VariantIdKey = "VARIANT_ID";
    public const string VariantKey = "VARIANT";
    public const string VersionCodenameKey = "VERSION_CODENAME";
    public const string VersionIdKey = "VERSION_ID";
    public const string VersionKey = "VERSION";

    private readonly Dictionary<string, string> myProperties;

    private OsReleaseProperties(Dictionary<string, string> properties)
    {
      myProperties = properties;
    }

    public string? TryGetValue(string key) => myProperties.TryGetValue(key, out var value) ? value : null;

    public JetUnixId? TryGetUnixId()
    {
      if (myProperties.TryGetValue(IdKey, out var idStr))
      {
        var k = idStr.IndexOf('-');
        if (k >= 0)
          idStr = idStr.Substring(0, k);
        switch (idStr)
        {
        // @formatter:off
        case "alpine"    : return JetUnixId.Alpine;
        case "amzn"      : return JetUnixId.Amazon;
        case "arch"      : return JetUnixId.Arch;
        case "centos"    : return JetUnixId.CentOS;
        case "debian"    : return JetUnixId.Debian;
        case "elementary": return JetUnixId.Elementary;
        case "fedora"    : return JetUnixId.Fedora;
        case "freebsd"   : return JetUnixId.FreeBSD;
        case "kali"      : return JetUnixId.Kali;
        case "linuxmint" : return JetUnixId.Mint;
        case "manjaro"   : return JetUnixId.Manjaro;
        case "nixos"     : return JetUnixId.NixOS;
        case "ol"        : return JetUnixId.Oracle;
        case "opensuse"  : return JetUnixId.OpenSUSE;
        case "rhel"      : return JetUnixId.RHEL;
        case "sles"      : return JetUnixId.SLES;
        case "ubuntu"    : return JetUnixId.Ubuntu;
        // @formatter:on
        }
      }

      return null;
    }

    public static OsReleaseProperties? ReadFromDefaultLocations()
    {
      if (HabitatInfo.Platform is JetPlatform.FreeBSD or JetPlatform.Linux)
        foreach (var osReleaseFile in new[]
                 {
                   "/etc/os-release",
                   "/usr/lib/os-release"
                 })
          if (File.Exists(osReleaseFile))
          {
            using var reader = File.OpenText(osReleaseFile);
            return ReadFromStream(reader);
          }

      return null;
    }

    internal static OsReleaseProperties ReadFromStream(StreamReader reader)
    {
      // Note(ww898): See https://www.linux.org/docs/man5/os-release.html and https://www.freedesktop.org/software/systemd/man/os-release.html

      static bool IsQuotedString(string str, char ch) => str.Length >= 2 && str[0] == ch && str[str.Length - 1] == ch;

      var properties = new Dictionary<string, string>();
      for (string? line; (line = reader.ReadLine()) != null;)
      {
        line = line.TrimEnd();
        if (line.Length == 0 || line[0] == '#')
          continue;
        var n = line.IndexOf('=');
        if (n < 0)
          throw new FormatException();
        var key = line.Substring(0, n);
        var value = line.Substring(n + 1);
        properties[key] = IsQuotedString(value, '\'') || IsQuotedString(value, '"')
          ? value.Substring(1, value.Length - 2)
          : value;
      }

      return new OsReleaseProperties(properties);
    }
  }
}