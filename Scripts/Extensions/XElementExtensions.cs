using System;
using System.Linq;
using System.Xml.Linq;

namespace MySimsToolkit.Scripts.Extensions;

public static class XElementExtensions
{
    public static XElement? ElementIgnoreCase(this XElement element, string name)
    {
        return element.Elements().FirstOrDefault(x =>
            string.Equals(x.Name.LocalName, name, StringComparison.CurrentCultureIgnoreCase));
    }
}