using System;
using System.Globalization;
using System.IO;
using HtmlAgilityPack;

namespace MySimsToolkit.Scripts.Formats.Save;

public class SaveHeaderXml
{
    public const long GameCubeTime = 60750352;
    public string SimName { get; set; }
    public string TownName { get; set; }
    public int NumStars { get; set; }
    public uint Version { get; set; }
    public DateTime DateOfSave { get; set; }
    public TimeSpan TimePlayed { get; set; }
    
    public static SaveHeaderXml Read(Stream stream)
    {
        var doc = new HtmlDocument();
        doc.Load(stream);

        var root = doc.DocumentNode.Element("saveheader");

        var simName = root.Element("simname").InnerText;
        var townName = root.Element("townname").InnerText;
        var stars = int.Parse(root.Element("numstars").InnerText);
        var version = uint.Parse(root.Element("version").InnerText);
        
        var gameCubeTicks = long.Parse(root.Element("dateofsave").InnerText, NumberStyles.HexNumber) / GameCubeTime;
        var dateOfSave = DateTimeOffset.FromUnixTimeSeconds(gameCubeTicks).DateTime;
        
        var timePlayedSeconds = long.Parse(root.Element("timeplayed").InnerText, NumberStyles.HexNumber) / GameCubeTime;
        var timePlayed = TimeSpan.FromSeconds(timePlayedSeconds);

        return new SaveHeaderXml
        {
            SimName = simName,
            TownName = townName,
            NumStars = stars,
            Version = version,
            DateOfSave = dateOfSave,
            TimePlayed = timePlayed,
        };
    }
}