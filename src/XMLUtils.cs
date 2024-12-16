using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text;
using System.Xml.Xsl;

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

        Console.WriteLine(filePath);
        // After every saving to html, save also to html
        RunXSLT("data/xslt/convertXMLtoHTML.xslt", filePath, $"data/html/{Path.GetFileNameWithoutExtension(filePath)}.html");
    }

    public static void Load(string filename)
    {
        filename = "./data/saved_games/" + filename;
        // Check that the data file makes sense, aka it is not corrupted
        Validate("data/xsd/saved_game.xsd", filename);
        var serializer = new XmlSerializer(typeof(Game));
        var reader = new StreamReader(filename);
        Game? LoadedGame = (Game?)serializer.Deserialize(reader);
        if (LoadedGame == null) throw new Exception("Could not deserialize game.");

        LoadedGame.State = GameState.Paused;
        LoadedGame.Update();
        LoadedGame.Show();
        LoadedGame.Run();
    }


    public static void Validate(string XSDFilePath, string XMLFile)
    {
        // Load the schema
        XmlSchemaSet schemas = new XmlSchemaSet();
        schemas.Add(null, XSDFilePath);

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

    public static void ConvertXMLtoHTML(string XSDFilePath, string xmlFilePath, string outputHtmlPath)
    {
        // Validation du fichier XML avec le fichier XSD
        Validate(XSDFilePath, xmlFilePath);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlFilePath);

        // Gestion des espaces de noms
        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("game", "http://www.silentstrike.com");

        // Construire le contenu HTML
        StringBuilder htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine(@$"<!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                    <meta charset='UTF-8' />
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                    <title>Maze Renderer</title>
                                    <link rel='stylesheet' href='../css/style.css' />
                                </head>
                                <body>
                                    <div id='maze-container'>
                                        <p class='title'>
                                            <strong>Maze</strong>: {xmlFilePath}
                                        </p>");

        // Récupération des données du joueur
        XmlNode? playerNode = xmlDoc.SelectSingleNode("//game:player", nsManager);
        int playerX = int.Parse(playerNode?["game:X"]?.InnerText ?? "0");
        int playerY = int.Parse(playerNode?["game:Y"]?.InnerText ?? "0");
        Orientation playerOrientation = Enum.Parse<Orientation>(playerNode?["game:Orientation"]?.InnerText ?? "EAST");

        // Récupération des ennemis
        XmlNodeList? enemyNodes = xmlDoc.SelectNodes("//game:Enemy", nsManager);

        // Récupération des lignes de la grille
        XmlNodeList? rows = xmlDoc.SelectNodes("//game:Row", nsManager);

        int rowIndex = 0;
        foreach (XmlNode row in rows!)
        {
            htmlBuilder.AppendLine("<div class=\"row\">");
            XmlNodeList? nodes = row.SelectNodes("game:Node", nsManager);

            int colIndex = 0;
            foreach (XmlNode node in nodes!)
            {
                htmlBuilder.Append("<div class=\"node");

                // Ajouter les classes en fonction des murs
                if (node["game:TOP"]?.InnerText == "true") htmlBuilder.Append(" wall-top");
                if (node["game:RIGHT"]?.InnerText == "true") htmlBuilder.Append(" wall-right");
                if (node["game:BOTTOM"]?.InnerText == "true") htmlBuilder.Append(" wall-bottom");
                if (node["game:LEFT"]?.InnerText == "true") htmlBuilder.Append(" wall-left");
                htmlBuilder.AppendLine("\">");

                // Vérifier si c'est la position du joueur
                if (rowIndex == playerX && colIndex == playerY)
                {
                    htmlBuilder.Append(@$"<p>
                                            {new Player { X = playerX, Y = playerY, Orientation = playerOrientation }}
                                        </p>");
                }

                // Vérifier si c'est la position d'un ennemi
                foreach (XmlNode enemy in enemyNodes!)
                {
                    int enemyX = int.Parse(enemy["game:X"]?.InnerText ?? "0");
                    int enemyY = int.Parse(enemy["game:Y"]?.InnerText ?? "0");
                    Orientation enemyOrientation = Enum.Parse<Orientation>(enemy["game:Orientation"]?.InnerText ?? "EAST");
                    bool isAlive = bool.Parse(enemy["game:Alive"]?.InnerText ?? "false");

                    if (rowIndex == enemyX && colIndex == enemyY && isAlive)
                    {
                        htmlBuilder.Append(@$"<p class='red'>
                    
                                            {new Enemy { X = enemyX, Y = enemyY, Orientation = enemyOrientation }}
                                            </p>");
                    }
                }

                colIndex++;
                htmlBuilder.Append("</div>");
            }

            htmlBuilder.AppendLine("</div>");
            rowIndex++;
        }

        htmlBuilder.AppendLine("</div>");
        htmlBuilder.AppendLine("</body>");
        htmlBuilder.AppendLine("</html>");

        // Écriture du HTML dans un fichier
        File.WriteAllText(outputHtmlPath, htmlBuilder.ToString());
    }


    public static void RunXSLT(string xsltFilePath, string xmlFilePath, string outputHtmlPath)
    {
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(xsltFilePath);
        xslt.Transform(xmlFilePath, outputHtmlPath);

    }

    public static void UpdateSavedGamesMenu(string savedGamesDirectory, string outputFilePath)
    {
        // Get all XML files in the directory
        string[] xmlFiles = Directory.GetFiles(savedGamesDirectory, "*.xml");

        // Prepare the XML document with namespace
        XmlDocument doc = new XmlDocument();
        XmlElement rootElement = doc.CreateElement("menu", "saved_games", "http://www.silentstrike.com/menu");
        doc.AppendChild(rootElement);

        // Add namespace declaration
        XmlAttribute xmlnsAttr = doc.CreateAttribute("xmlns:menu");
        xmlnsAttr.Value = "http://www.silentstrike.com/menu";
        rootElement.Attributes.Append(xmlnsAttr);

        // Collect file names and last modified dates
        foreach (string filePath in xmlFiles)
        {
            string fileName = Path.GetFileName(filePath);
            DateTime lastModified = File.GetLastWriteTime(filePath);

            // Add the file name and modification time
            XmlElement fileElement = doc.CreateElement("menu", "file", "http://www.silentstrike.com/menu");

            XmlElement nameElement = doc.CreateElement("menu", "name", "http://www.silentstrike.com/menu");
            nameElement.InnerText = fileName;
            fileElement.AppendChild(nameElement);


            // Create the <path> element with the updated file path
            XmlElement pathToHTML = doc.CreateElement("menu", "path", "http://www.silentstrike.com/menu");
            pathToHTML.InnerText = $"html/{Path.GetFileNameWithoutExtension(filePath)}.html";
            fileElement.AppendChild(pathToHTML);



            XmlElement lastModifiedElement = doc.CreateElement("menu", "last_modified", "http://www.silentstrike.com/menu");
            lastModifiedElement.InnerText = lastModified.ToString("dd-MM-yyyy on HH:mm");
            fileElement.AppendChild(lastModifiedElement);

            rootElement.AppendChild(fileElement);
        }

        // Save the XML document to a file
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
        };

        using (XmlWriter writer = XmlWriter.Create(outputFilePath, settings))
        {
            doc.Save(writer);
        }
    }
}