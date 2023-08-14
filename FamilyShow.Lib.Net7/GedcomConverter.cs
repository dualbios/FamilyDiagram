/*
 * GedcomConverter class
 * Converts a GEDCOM file to an XML file.
 * 
 * GedcomLine class
 * Parses one line in a GEDCOM file.
*/

using System.Text;
using System.Xml;

namespace FamilyShow.Lib.Net7;

/// <summary>
/// Converts a GEDCOM file to an XML file.
/// </summary>
internal static class GedcomConverter
{
    /// Create an xml file that contains the same hierarchy specified in 
    /// the GEDCOM file. GEDCOM lines are limited to 255 characters, 
    /// combineSplitValues indicates if the split lines should be combined 
    /// into a single XML element.
    public static Stream ConvertToXml(StreamReader stream, bool combineSplitValues)
    {
        // Store the previous level so can determine when need
        // to close xml element tags.
        var prevLevel = -1;

        // Used to create the .xml file, XmlWriterSettings. Indent is 
        // specified if you want to examine the xml file, otherwise
        // it should be removed.
        XmlWriterSettings settings = new();
        settings.Indent = true;
        Stream st = new MemoryStream();
        TextWriter tw = new StreamWriter(st);
        using (XmlWriter writer = XmlWriter.Create(tw, settings))
        {
            // Root element that contains all of the other elements.
            writer.WriteStartElement("root");

            // Convert each line of the gedcom file to an xml element.

            {
                string text;
                GedcomLine line = new();
                while ((text = stream.ReadLine()) != null)
                {
                    // Some GEDCOM files indent each line with whitespace, delete any
                    // whitespace from the beginning and end of the line.
                    text = text.Trim();

                    // Parse gedcome line into Level, Tag and Value fields.
                    if (line.Parse(text))
                    {
                        // See if need to close previous xml elements.
                        if (line.Level <= prevLevel)
                        {
                            // Determine how many elements to close.
                            var count = prevLevel - line.Level + 1;
                            for (var i = 0; i < count; i++) writer.WriteEndElement();
                        }

                        // Create new xml element.
                        writer.WriteStartElement(line.Tag);
                        writer.WriteAttributeString("Value", line.Data);
                        prevLevel = line.Level;
                    }
                }
            }

            // Close the last element.
            writer.WriteEndElement();

            // Close the root element.
            writer.WriteEndElement();

            // Write to the file system.
            writer.Flush();
            writer.Close();

            if (combineSplitValues)
            {
                st.Position = 0;
                st = CombineSplitValues(st);
            }

            return st;
        }
    }

    /// <summary>
    /// GEDCOM lines have a max length of 255 characters, this goes through
    /// the XML files and combines all of the split lines which makes the
    /// XML file easier to process.
    /// </summary>
    private static Stream CombineSplitValues(Stream xmlFilePath)
    {
        Stream ms = new MemoryStream();
        XmlDocument doc = new();
        doc.Load(xmlFilePath);

        // Get all nodes that contain child continue nodes.
        XmlNodeList list = doc.SelectNodes("//CONT/.. | //CONC/..");

        // Go through each node and append child continue nodes.
        foreach (XmlNode node in list) AppendValues(node);

        // Write the updates to the file system.
        doc.Save(ms);
        return ms;
    }

    /// <summary>
    /// Append child continue nodes to the parent.
    /// </summary>
    private static void AppendValues(XmlNode node)
    {
        // Get the value for the parent node.
        StringBuilder sb = new(node.Attributes["Value"].Value);

        // Find all of the child continue nodes.
        XmlNodeList list = node.SelectNodes("CONT | CONC");
        foreach (XmlNode childNode in list)
        {
            switch (childNode.Name)
            {
                // Concatenate.
                case "CONC":
                    sb.Append(childNode.Attributes["Value"].Value);
                    break;

                // Continue, add line return and then the text.
                case "CONT":
                    sb.AppendFormat("\r{0}", childNode.Attributes["Value"].Value);
                    break;
            }

            // Remove all of the child continue nodes.
            node.RemoveChild(childNode);
        }

        // Update the parent node value.
        node.Attributes["Value"].Value = sb.ToString();
    }
}