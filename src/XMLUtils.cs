using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

class XMLUtils
{
    private XMLUtils() { }

    public static void Save(Game game, string? filename)
    {
        if (filename == null) throw new ArgumentNullException(nameof(filename));

        var serializer = new XmlSerializer(typeof(Game));

        string filePath = "./data/saved_games/" + filename;

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        using (var writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(writer, game);
        }
    }

    public static void Load(string filename)
    {
        filename = "./data/saved_games/" + filename;
        XMLUtils.Validate("./xsd/save.xsd", filename);
        var serializer = new XmlSerializer(typeof(Game));
        var reader = new StreamReader(filename);
        Game? LoadedGame = (Game?)serializer.Deserialize(reader);
        if (LoadedGame == null) throw new Exception("Could not deserialize game.");

        LoadedGame.Update();
        LoadedGame.Show();
        LoadedGame.RunPlayer();
    }


    public static void Validate(string XSDFile, string XMLFile)
    {
        // Load the schema
        XmlSchemaSet schemas = new XmlSchemaSet();
        schemas.Add(null, XSDFile);

        // Configure the XmlReaderSettings
        XmlReaderSettings settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemas
        };

        // Validation event handler to throw exceptions for any validation errors
        settings.ValidationEventHandler += (sender, e) =>
        {
            throw new Exception($"Corrupt file: {e.Message}");
        };

        // Validate the XML
        using (XmlReader reader = XmlReader.Create(XMLFile, settings))
        {
            while (reader.Read()) { } // Reading ensures validation occurs
        }
    }

}