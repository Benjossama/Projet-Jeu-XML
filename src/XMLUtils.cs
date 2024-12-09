using System.Xml.Serialization;

class XMLUtils
{
    private XMLUtils() { }

    static public void Save(Game game)
    {
        var serializer = new XmlSerializer(typeof(Game));
        using (var writer = new StringWriter())
        {
            serializer.Serialize(writer, game);
            File.WriteAllText("./test.xml", writer.ToString());
        }
    }

}