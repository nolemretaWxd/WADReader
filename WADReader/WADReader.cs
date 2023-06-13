using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace nolemretaW.WADReader;

public static class WadReader
{
    public static WadList LoadedFiles { get; internal set; } = new WadList();
    public static bool DisableWadTypeCheck { get; set; } = false;

    public static byte[] ReadFile(Lump file)
    {
        using (FileStream fs = File.Open(file.SourceWAD, FileMode.Open))
        {
            fs.Seek(file.Offset, SeekOrigin.Begin);
            byte[] contents = new byte[file.Size];
            fs.Read(contents, 0, file.Size);
            return contents;
        }
    }

    public static  async Task<Byte[]> ReadFileAsync(Lump file)
    {
        using (FileStream fs = File.Open(file.SourceWAD, FileMode.Open)) 
        {
            fs.Seek(file.Offset, SeekOrigin.Begin);
            byte[] contents = new byte[file.Size];
            await fs.ReadAsync(contents, 0, file.Size);
            return contents;
        }
    }

    public static Lump? FindLump(string name)
    {
        foreach (var wad in LoadedFiles.Reverse())
        {
            foreach (Lump lump in wad.Directory)
            {
                if (lump.Name == name)
                    return lump;
            }
        }

        return null;
    }

    public static void AddFile(string path)
    {
        using(FileStream fs = File.Open(path, FileMode.Open))
        {
            byte[] header = new Byte[4];
            fs.Read(header, 0, 4);
            if (Encoding.ASCII.GetString(header) != "PWAD" && Encoding.ASCII.GetString(header) != "IWAD")
                throw new NonWadFileException();

            if (LoadedFiles.Where(i => i.Path == path).FirstOrDefault() != null)
                throw new WadAlreadyLoadedException();

            if (!DisableWadTypeCheck && LoadedFiles.Count == 0 && Encoding.ASCII.GetString(header) != "IWAD")
                throw new IwadNotFirstException();

            if (!DisableWadTypeCheck && LoadedFiles.Count >= 1 && Encoding.ASCII.GetString(header) != "PWAD")
                throw new MoreThanOneIwadException();

            byte[] numLumps = new byte[4];
            fs.Read(numLumps, 0, 4);
            byte[] dirOffset = new byte[4];
            fs.Read(dirOffset, 0, 4);
            List<Lump> dir = new List<Lump>();
            Wad wad = new Wad();
            wad.Path = path;
            wad.Type = Encoding.ASCII.GetString(header) == "PWAD" ? WadType.Pwad : WadType.Iwad;

            for(int index = 0; index < BitConverter.ToInt32(numLumps); index++)
            {
                int offset = index * 16 + BitConverter.ToInt32(dirOffset);
                byte[] filesize, fileoffset;
                filesize = new byte[4]; 
                fileoffset = new byte[4];
                byte[] filename = new byte[8];
                
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Read(fileoffset, 0, 4);
                fs.Read(filesize, 0, 4);
                fs.Read(filename, 0, 8);
                
                Lump file = new Lump();
                file.Offset = BitConverter.ToInt32(fileoffset);
                file.Size = BitConverter.ToInt32(filesize);
                file.SourceWAD = path;
                file.Name = Encoding.ASCII.GetString(filename).Trim('\0');
                file.PositionInWad = index;
                wad.Directory.Add(file);
            }
            LoadedFiles.Add(wad);
        }
    }
    
    [Serializable]
    class MoreThanOneIwadException : Exception
    {
        public MoreThanOneIwadException() : base("Provided file is an IWAD. There can be only one IWAD loaded.")
        {
            
        }
    }
    
    [Serializable]
    class IwadNotFirstException : Exception
    {
        public IwadNotFirstException() : base("You're trying to load a PWAD first. You need to load an IWAD before loading any PWADs")
        {
            
        }
    }

    [Serializable]
    class NonWadFileException : Exception
    {
        public NonWadFileException() : base("This file is not a valid WAD.")
        {
            
        }
    }

    [Serializable]
    class WadAlreadyLoadedException : Exception
    {
        public WadAlreadyLoadedException() : base("Wad file is already loaded.")
        {
            
        }
    }
}

    
public class Lump
{
    public int Offset { get; internal set; }
    public int Size { get; internal set; }
    public string Name { get; internal set; } = "";
    public int PositionInWad { get; internal set; }
    public string SourceWAD { get; internal set; } = "";
}

public class Wad
{
    public WadType Type { get; internal set; }
    public List<Lump> Directory { get; internal set; } = new List<Lump>();
    public string Path { get; internal set; } = "";
}

public class WadList : Collection<Wad>
{
    public Wad? this[string path]
    {
        get
        {
            foreach (var item in base.Items)
            {
                if (item.Path == path)
                    return item;
            }
            return null;
        }
    }
}

public enum WadType
{
    Iwad,
    Pwad
}