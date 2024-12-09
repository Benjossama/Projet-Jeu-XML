using System.Dynamic;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

[Serializable]
internal class TestMaze
{
    [XmlElement("Height")]
    public int h;
    [XmlElement("Width")]
    public int w;
}
class Program
{
    public static void Main(string[] args)
    {
        //     if (args[1] == "1")
        //     {
        // Game game = new Game(5, 5, 2);
        // }
        // else
        // {
        // Maze test = new Maze(5, 5);
        var serializer = new XmlSerializer(typeof(Maze));
        // using (var writer = new StringWriter())
        // {
        //     serializer.Serialize(writer, test);
        //     // Console.WriteLine(writer); // Check the serialized XML outpu
        //     File.WriteAllText("./test.xml", writer.ToString());
        // }
        // var serializer = new XmlSerializer(typeof(Game));


        using (var reader = new StreamReader("./test.xml"))
        {
            Maze test1 = (Maze)serializer.Deserialize(reader);
            Engine.Print(test1, null, null, "test");
        }
        // }

    }
}
