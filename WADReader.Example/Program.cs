using System.Text;
using nolemretaW.WADReader;

class Program
{
    public static void Main(string[] args)
    {
        //Load an IWAD file
        WadReader.AddFile("test.wad");

        //List loaded WADs
        foreach (var file in WadReader.LoadedFiles)
        {
            Console.WriteLine(file.Path);
        }
        
        //Load a PWAD
        WadReader.AddFile("test2.wad");
        
        //Find HELLOWLD, which is replaced by test2.wad
        Lump? hellowld = WadReader.FindLump("HELLOWLD");
        Console.WriteLine("HELLOWLD found");
        Console.WriteLine("Source WAD: " + hellowld.SourceWAD);
        Console.WriteLine("Position in WAD: " + hellowld.PositionInWad);
        Console.WriteLine("Size: " + hellowld.Size);
                
        //Get HELLOWLD contents
        byte[] hellowldContent = WadReader.ReadFile(hellowld);
        Console.WriteLine(Encoding.ASCII.GetString(hellowldContent));
        
        //Find HELLO2, which is only in test.wad
        Lump? hello2 = WadReader.FindLump("HELLO2");
        Console.WriteLine("HELLO2 found");
        Console.WriteLine("Source WAD: " + hello2.SourceWAD);
        Console.WriteLine("Position in WAD: " + hello2.PositionInWad);
        Console.WriteLine("Size: " + hello2.Size);

        //Read file next to HELLOWLD in test2.wad
        Lump otherLump = WadReader.LoadedFiles[hellowld.SourceWAD].Directory[hellowld.PositionInWad + 1];
        Console.WriteLine("Name: " + otherLump.Name);
        Console.WriteLine("Source WAD: " + otherLump.SourceWAD);
        Console.WriteLine("Position in WAD: " + otherLump.PositionInWad);
        Console.WriteLine("Size: " + otherLump.Size);
    }
}