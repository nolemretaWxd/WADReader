# Documentation

## WadReader

### Properties

#### LoadedFiles
```cs
public static WadList LoadedFiles
```
Collection of Wad objects that are loaded.
You can index this list by WADs path eg. `LoadedFiles["DOOM2.WAD"]` to get a Wad object of DOOM2.WAD if it's loaded. Otherwise, it returns null.

#### DisableWadTypeCheck
```cs
public static bool DisableWadTypeCheck { get; set; } = false;
```
Setting this to true disables checking WAD type.
When set to true:
 * If this is first file, library won't check if it's a IWAD.
 * If this is not the first file, library won't check if it's an PWAD.

### Methods

#### AddFile
```cs
public static void AddFile(string path)
```
Loads a WAD. First file has to be a IWAD, next ones have to be PWADs. Set `DisableWadTypeCheck` to true to disable these checks.

#### FindLump
```cs
public static Lump? FindLump(string name)
```
Tries to find a Lump in loaded WADs.
Returns null if not found.
NOTE: Library searches from last file loaded to first.

#### ReadFile
```cs
public static byte[] ReadFile(Lump file)
```
Loads lump's data.

Async:
```cs
public static async Task<Byte[]> ReadFileAsync(Lump file)
```

## Lump
 * `public int Offset` - Offset of lump's data in a WAD.
 * `public int Size` - Size of a lump.
 * `public string Name` - Name of a lump.
 * `public int PositionInWad` - Position of a lump in WAD's directory it's in.
 * `public string SourceWAD` - Path to a WAD the lump is in.

## Wad
 * `public WadType Type` - Type of a WAD. Can be either `WadType.Pwad` or `WadType.Iwad`.
 * `public List<Lump> Directory` - Directory of a WAD.
 * `public string Path` - Path to a WAD.
