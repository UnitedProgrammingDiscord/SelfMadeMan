using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class BuildingGenerator : MonoBehaviour {
  public Tilemap buildingtm;
  public Tilemap doorstm;
  public Tilemap[] roofstmEv;
  public Tilemap[] roofstmOd;
  public Tilemap signstm;
  public Tilemap roadtm;


  /* Space before previous 0-8 tiles
   * Width 5-16 tiles
   * Height 4-8 tiles
   * Second floor 0-1
   * Door position 1..(width-1)
   * Type of door
   * Type of windows
   * Number of windows
   * Type of material
   * Color
   */

  [Header("Specs")]
  public int SpaceBefore = 0;
  public int Width = 5;
  public int Height = 4;
  public int DoorPosition = 1;
  public int DoorType = 0;
  public int WindowsType1 = 0;
  public int WindowsNum1 = 0;
  public int WindowsType2 = 0;
  public int WindowsNum2 = 0;
  public int MaterialType = 0;
  public Color WallsColor = Color.white;
  public Color RoofColor = Color.white;
  [Range(0, 1)] public float Happiness = .75f;
  public BuildingType BType;

  [Header("Randomness")]
  public string RandomHash;
  public int NumberOfBuldings = 1;

  [Header("Reference assets")]
  public List<BuildMat> Mats;
  public List<DoorType> Doors;
  public List<WindowType> Windows;
  public List<TileBase> Roof;
  public List<TileBase> Roads;
  public List<TileBase> Special; // 0-11 food stand

  readonly string[] buildingColors = {
    "#fa8d93ff", "#c5aa53ff", "#9d9f92ff", "#f58987ff", "#cda54eff", "#bd9357ff",
    "#b68b25ff", "#d0981fff", "#a2988eff", "#bc7271ff", "#82272eff", "#798986ff",
    "#018bafff", "#b37659ff", "#7c4f32ff", "#a79e5dff", "#a9a05fff", "#f1e7a2ff",
    "#fddbcfff", "#f1d3c9ff", "#d85e59ff", "#8d9a7eff", "#8abfe9ff", "#fc6380ff",
    "#fae697ff", "#ffa48dff", "#00af6bff", "#00af6bff", "#c6aa94ff", "#ba8691ff",
    "#c4a670ff", "#c4b783ff", "#a1c0b0ff", "#a3bbadff", "#e6e9deff", "#a3badbff",
    "#fdd4ceff", "#b695a8ff", "#94a4bbff", "#f5edaeff", "#fe9e76ff", "#beb18fff",
    "#db7b6fff", "#d7e0dbff", "#a1b0c3ff", "#d85d79ff", "#fdab8fff", "#a9a09fff",
   };


  Tilemap[] rtmsEv;
  Tilemap[] rtmsOd;

  public void Clean() {
    rtmsEv = new Tilemap[roofstmEv.Length];
    rtmsOd = new Tilemap[roofstmOd.Length];
    for (int i = 0; i < roofstmEv.Length; i++) rtmsEv[i] = roofstmEv[i];
    for (int i = 0; i < roofstmOd.Length; i++) rtmsOd[i] = roofstmOd[i];
    for (int y = 0; y < 16; y++) {
      for (int x = -8; x < 200; x++) {
        Vector3Int cell = new(x, y, 0);
        buildingtm.SetTile(cell, null);
        doorstm.SetTile(cell, null);
        for (int z = 0; z < roofstmEv.Length; z++) {
          roofstmEv[z].SetTile(cell, null);
          roofstmOd[z].SetTile(cell, null);
        }
        roadtm.SetTile(cell, null);
      }
    }
  }

  public void CalculateValuesFromHash(string hash) {
    ulong key = 0;

    byte[] bytes;
    using (HashAlgorithm algorithm = SHA256.Create())
      bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(hash));
    for (int i = 0; i < bytes.Length; i++) {
      int pos = i % 8;
      key ^= (ulong)(bytes[i] << pos);
    }

    foreach (var l in hash) {
      key ^= (key & 0xFF0000000000) >> 40;
      key <<= 6;
      key ^= (uint)l;
    }
    key ^= (key & 0xFF0000000000) >> 40;
    key <<= 6;
    key ^= (uint)(hash.Length);


    const ulong BIT_NOISE1 = 0xB5297A4DB5297A4Dul;
    const ulong BIT_NOISE2 = 0x68E31DA468E31DA4ul;
    const ulong BIT_NOISE3 = 0x1B56C4E91B56C4E9ul;

    key *= BIT_NOISE1;
    key ^= (key >> 8);
    key += BIT_NOISE2;
    key ^= (key << 8);
    key *= BIT_NOISE3;
    key ^= (key >> 8);


    SpaceBefore = (byte)(key & 7);
    Width = (byte)((key >> 3) & 7) + 4;
    Height = (byte)((key >> 6) & 3) + 3;
    DoorPosition = (byte)((key >> 8) & 7);
    DoorPosition = (DoorPosition & (Width - 2)) + 1;
    DoorType = (byte)((key >> 11) & 7);
    WindowsType1 = (byte)((key >> 14) & 7);
    WindowsNum1 = (byte)((key >> 17) & 3);
    WindowsType2 = (byte)((key >> 19) & 7);
    WindowsNum2 = (byte)((key >> 22) & 7);
    MaterialType = (byte)((key >> 25) & 7);

    if (WindowsNum1 == 0 && WindowsNum2 == 0) {
      if (Height < 3) WindowsNum1 = 1;
      else WindowsNum2 = 1;
    }

    int cpos = (byte)(buildingColors.Length * ((int)(key >> 28) & 255) / 255f);
    string hex = buildingColors[cpos];
    ColorUtility.TryParseHtmlString(hex, out Color fhc);
    fhc.a = 1;
    // Set saturation according to happiness
    Color.RGBToHSV(fhc, out float h, out float s, out float v);
    s *= Happiness;
    s = .975f * s + .025f;
    Color32 hc = Color.HSVToRGB(h, s, v);
    hc.a = 255;
    WallsColor = hc;
    hc = Color.HSVToRGB(h < .5f ? h * h * h : (1 - h) * (1 - h) * (1 - h), s * .5f, v * .8f); // Make roofs more on the red
    hc.a = 255;
    RoofColor = hc;

    BType = (BuildingType)((((int)(key >> 36) & 255) % 29) + 1);
    BType = BuildingType.FoodStand;
  }


  public void GenerateFullRoad() {
    Clean();
    int num = NumberOfBuldings;
    int pos = 0; // Calculate the last position
    for (int i = 0; i < num - 1; i++) {
      CalculateValuesFromHash(RandomHash + i);
      pos += SpaceBefore + Width;
      if (i == num - 1) pos -= Width;
    }
    CalculateValuesFromHash(RandomHash + (num - 1));
    for (int i = num - 1; i >= 0; i--) {
      Generate(i % 2, pos);
      CalculateValuesFromHash(RandomHash + (i - 1));
      pos -= SpaceBefore + Width;
    }
    // Roads
    CalculateValuesFromHash(RandomHash + 0);

    int min = SpaceBefore - 3;
    int max = min + 6;
    for (int i = 0; i < num; i++) {
      CalculateValuesFromHash(RandomHash + i);
      max += SpaceBefore + Width;
    }
    CleanRoad(roadtm, min, max, Roads[10]);

    min = SpaceBefore - 3;
    max = min + 6;
    for (int i = 0; i < 5; i++) {
      CalculateValuesFromHash(RandomHash + i);
      max += SpaceBefore + Width;
    }
    GenerateRoad(roadtm, min, max);

    min = max;
    CalculateValuesFromHash(RandomHash + 5);
    min += SpaceBefore + Width;
    max = min + 6;
    for (int i = 6; i < num; i++) {
      CalculateValuesFromHash(RandomHash + i);
      max += SpaceBefore + Width;
    }
    GenerateRoad(roadtm, min, max);


  }

  public void Generate(int rindex, int xoffset = 0) {
    Vector3Int bl = new(SpaceBefore + xoffset, 3);
    BuildMat mat = Mats[MaterialType % Mats.Count];
    Color32 col = WallsColor;
    col.a = 255;
    Color32 colroof = RoofColor;
    colroof.a = 255;
    int depth = SpaceBefore;
    if (depth < 2) depth = 2;
    if (depth > Height) depth = Height;
    if (depth > 4) depth = 4;


    if (BType == BuildingType.Intersection) return; // Nothing to draw here

    if (BType == BuildingType.FoodStand) {
      GenerateFoodStand(bl, col);
      return;
    }

    if (BType == BuildingType.SmallPark) {
      GeneratePark(bl);
      return;
    }

    bool drawWalls = (BType != BuildingType.SmallPark) &&
      (BType != BuildingType.Park) && (BType != BuildingType.Garden) && (BType != BuildingType.FoodTruck) &&
      (BType != BuildingType.Market) && (BType != BuildingType.Dumpster) && (BType != BuildingType.Grass);



    if (drawWalls) {
      // Side walls
      for (int x = 0; x < depth; x++) {
        Vector3Int pos = new Vector3Int(-x - 1, x) + bl;
        SetTile(pos, buildingtm, col, mat.tiles[6]);
        for (int y = 1; y < Height; y++) {
          pos = new Vector3Int(-x - 1, y + x) + bl;
          SetTile(pos, buildingtm, col, mat.tiles[5]);
        }
        pos = new Vector3Int(-x - 1, Height + x) + bl;
        SetTile(pos, buildingtm, col, mat.tiles[4]);
      }
      // Left walls
      for (int y = 0; y < Height; y++) {
        Vector3Int pos = new Vector3Int(0, y) + bl;
        if (mat.mode == PiecePos.LeftRightRandom2 || mat.mode == PiecePos.LeftRightMod2X || mat.mode == PiecePos.LeftRightMod2Y || mat.mode == PiecePos.LeftRightMod2XY)
          SetTile(pos, buildingtm, col, mat.tiles[0]);
        else if (mat.mode == PiecePos.Random4)
          SetTile(pos, buildingtm, col, mat.tiles[Random.Range(0, 4)]);
        else if (mat.mode == PiecePos.Mod4X)
          SetTile(pos, buildingtm, col, mat.tiles[SpaceBefore % 4]);
        else if (mat.mode == PiecePos.Mod4X)
          SetTile(pos, buildingtm, col, mat.tiles[y % 4]);
        else if (mat.mode == PiecePos.Mod4XY)
          SetTile(pos, buildingtm, col, mat.tiles[(SpaceBefore + y) % 4]);
        else if (mat.mode == PiecePos.LeftLeftRight)
          SetTile(pos, buildingtm, col, mat.tiles[0]);
      }
      // Center walls
      for (int x = 1; x < Width - 1; x++) {
        for (int y = 0; y < Height; y++) {
          Vector3Int pos = new Vector3Int(x, y) + bl;
          if (mat.mode == PiecePos.LeftRightRandom2)
            SetTile(pos, buildingtm, col, mat.tiles[Random.Range(1, 3)]);
          else if (mat.mode == PiecePos.LeftRightMod2X)
            SetTile(pos, buildingtm, col, mat.tiles[pos.x % 2 + 1]);
          else if (mat.mode == PiecePos.LeftRightMod2Y)
            SetTile(pos, buildingtm, col, mat.tiles[pos.y % 2 + 1]);
          else if (mat.mode == PiecePos.LeftRightMod2XY)
            SetTile(pos, buildingtm, col, mat.tiles[(pos.x + pos.y) % 2 + 1]);
          else if (mat.mode == PiecePos.Random4)
            SetTile(pos, buildingtm, col, mat.tiles[Random.Range(0, 4)]);
          else if (mat.mode == PiecePos.Mod4X)
            SetTile(pos, buildingtm, col, mat.tiles[pos.x % 4]);
          else if (mat.mode == PiecePos.Mod4X)
            SetTile(pos, buildingtm, col, mat.tiles[pos.y % 4]);
          else if (mat.mode == PiecePos.Mod4XY)
            SetTile(pos, buildingtm, col, mat.tiles[(pos.x + pos.y) % 4]);
          else if (mat.mode == PiecePos.LeftLeftRight)
            SetTile(pos, buildingtm, col, mat.tiles[0]);
        }
      }
      // Right walls
      for (int y = 0; y < Height; y++) {
        Vector3Int pos = new Vector3Int(Width - 1, y) + bl;
        if (mat.mode == PiecePos.LeftRightRandom2 || mat.mode == PiecePos.LeftRightMod2X || mat.mode == PiecePos.LeftRightMod2Y || mat.mode == PiecePos.LeftRightMod2XY)
          SetTile(pos, buildingtm, col, mat.tiles[3]);
        else if (mat.mode == PiecePos.Random4)
          SetTile(pos, buildingtm, col, mat.tiles[Random.Range(0, 4)]);
        else if (mat.mode == PiecePos.Mod4X)
          SetTile(pos, buildingtm, col, mat.tiles[SpaceBefore % 4]);
        else if (mat.mode == PiecePos.Mod4X)
          SetTile(pos, buildingtm, col, mat.tiles[y % 4]);
        else if (mat.mode == PiecePos.Mod4XY)
          SetTile(pos, buildingtm, col, mat.tiles[(SpaceBefore + y) % 4]);
        else if (mat.mode == PiecePos.LeftLeftRight)
          SetTile(pos, buildingtm, col, mat.tiles[3]);
      }

      // Clean up previous windows
      for (int x = 0; x < Width; x++) {
        for (int y = 0; y < Height; y++) {
          SetTile(new Vector3Int(x, y) + bl, doorstm, Color.white, null);
        }
      }

      // Door
      DoorType dt = Doors[DoorType % Doors.Count];
      int doorp = DoorPosition % Width;
      if (doorp < 1) doorp = 1;
      if (doorp + dt.Width >= Width) doorp = Width - dt.Width - 1;
      for (int y = 0; y < dt.Height; y++) {
        for (int x = 0; x < dt.Width; x++) {
          doorstm.SetTile(bl + new Vector3Int(doorp + x, dt.Height - y - 1), dt.Tiles[x + dt.Width * y]);
        }
      }

      // Windows (low level)
      WindowType wt = Windows[WindowsType1 % Windows.Count];
      int num = WindowsNum1;
      if (num > 0) {
        if (wt.Strecheable) { // If the window is strechable try to fit the space
          int spaceBeforeDoor = doorp;
          int spaceAfterDoor = Width - doorp - 1;
          if (WindowsNum1 == 1) { // Fill the biggest space
            if (spaceBeforeDoor >= spaceAfterDoor) FillDoor(bl, 0, doorp, wt, doorstm);
            else FillDoor(bl, doorp + 1, Width, wt, doorstm);
          }
          else { // Fill both sides
            FillDoor(bl, 0, doorp, wt, doorstm);
            FillDoor(bl, doorp + dt.Width, Width, wt, doorstm);
          }
        }
        else { // Not streacheable, try to put the specified numebr of windows on each side of the door
               // split the number in two parts, balancing by space
          int numbef = WindowsNum1 / 2;
          int numaft = WindowsNum1 / 2;
          if (doorp == 0) {
            numbef = 0;
            numaft = WindowsNum1;
          }
          else if (doorp >= Width - wt.Width) {
            numbef = WindowsNum1;
            numaft = 0;
          }
          else if (numbef + numaft < WindowsNum1) {
            if (doorp > Width / 2) numbef++;
            else numaft++;
          }

          PlaceDoors(bl, 0, doorp, wt, doorstm, numbef);
          PlaceDoors(bl, doorp + dt.Width, Width, wt, doorstm, numaft);
        }
      }

      // Windows (top levels)
      wt = Windows[WindowsType2 % Windows.Count];
      num = WindowsNum2;
      if (num > 0) {
        for (int y = 2; y < Height - 1; y += 2) {
          Vector3Int wbl = bl;
          wbl.y += y;

          if (wt.Strecheable) { // If the window is strechable try to fit the space
            if (WindowsNum1 == 1) { // Fill the biggest space
              FillDoor(wbl, 0, Width, wt, doorstm);
            }
            else { // Fill by num
              int size = Width / WindowsNum2;
              if (size < 2) size = 2;
              for (int i = 0; i < Width; i += size) {
                if (i + 2 * size >= Width) {
                  FillDoor(wbl, i, Width, wt, doorstm);
                  break;
                }
                FillDoor(wbl, i, i + size, wt, doorstm);
              }
            }
          }
          else { // Not streacheable, try to put the specified numebr of windows on each side of the door
            PlaceDoors(wbl, 0, Width, wt, doorstm, WindowsNum2);
          }
        }
      }

      // Side Windows
      if (num > 0 && wt.SHeight != 0 && wt.SWidth != 0) {
        int yh = (Height / wt.SHeight) * wt.SHeight;
        int xw = (depth / wt.SWidth) * wt.SWidth;
        int step = wt.SWidth;
        if (step < 2) step = 2;
        for (int x = 0; x < xw; x+= step) {
          for (int y = 0; y < yh; y++) {
            for (int px = 0; px < wt.SWidth; px++) {
              Vector3Int pos = new Vector3Int(-x-1-px, 1 + y + x) + bl;
              int py = y % wt.SHeight;
              SetTile(pos, doorstm, Color.white, wt.SideTiles[px + wt.SWidth * py]);
            }
          }
        }
      }

      // Roof
      Tilemap[] rms = rindex == 0 ? roofstmEv : roofstmOd;
      int rm = Height - 3;
      if (rm < 0) rm = 0;
      if (rm >= rms.Length) rm = rms.Length - 1;
      Tilemap rtm = rms[rm];

      for (int x = -1; x < Width; x++) {
        for (int y = 0; y < depth; y++) {
          if (x == -1) SetTile(bl + new Vector3Int(x - y, Height + y), rtm, colroof, y == depth - 1 ? Roof[1] : Roof[0]);
          else if (x == Width - 1) SetTile(bl + new Vector3Int(x - y, Height + y), rtm, colroof, y == 0 ? Roof[3] : Roof[2]);
          else if (y == 0) SetTile(bl + new Vector3Int(x - y, Height + y), rtm, colroof, Roof[4]);
          else if (y == depth - 1) SetTile(bl + new Vector3Int(x - y, Height + y), rtm, colroof, Roof[6]);
          else SetTile(bl + new Vector3Int(x - y, Height + y), rtm, colroof, Roof[5]);
        }
      }
    }
  }

  void GeneratePark(Vector3Int bl) {
    // Because this will be pretty much just roads we may want to post-generate it after the roads
  }

  void GenerateFoodStand(Vector3Int bl, Color32 col) {
    Color.RGBToHSV(col, out float h, out float s, out float v);
    if (h > .5f) h = 1 - h;
    h = .25f * h + .05f;
    s *= 1.05f;
    v *= 1.1f;
    col = Color.HSVToRGB(h, s, v);

    Width %= 3;
    Width += 4;

    SetTile(bl + new Vector3Int(0, 2), buildingtm, col, Special[0]);
    SetTile(bl + new Vector3Int(0, 1), buildingtm, col, Special[4]);
    SetTile(bl + new Vector3Int(0, 0), buildingtm, col, Special[7]);
    for (int x = 1; x < Width; x++) {
      SetTile(bl + new Vector3Int(x, 2), buildingtm, col, x < Width - 1 ? Special[1] : Special[2]);
      SetTile(bl + new Vector3Int(x, 1), buildingtm, col, Special[4]);
      SetTile(bl + new Vector3Int(x, 0), buildingtm, col, Special[4]);
    }
    SetTile(bl + new Vector3Int(Width, 2), buildingtm, col, Special[3]);

    // Windows
    int add = Width == 6 ? 1 : 0;
    SetTile(bl + new Vector3Int(1 + add, 2), doorstm, col, Special[5]);
    SetTile(bl + new Vector3Int(2 + add, 2), doorstm, col, Special[6]);
    SetTile(bl + new Vector3Int(1 + add, 1), doorstm, col, Special[8]);
    SetTile(bl + new Vector3Int(2 + add, 1), doorstm, col, Special[9]);
    SetTile(bl + new Vector3Int(3 + add, 1), doorstm, col, Special[10]);
    SetTile(bl + new Vector3Int(1 + add, 0), doorstm, col, Special[11]);
    SetTile(bl + new Vector3Int(2 + add, 0), doorstm, col, Special[12]);
    SetTile(bl + new Vector3Int(3 + add, 0), doorstm, col, Special[13]);
  }

  void CleanRoad(Tilemap tm, int min, int max, TileBase clean) {// Clean everything
    for (int y = 0; y < 7; y++)
      for (int x = min - 16; x < max + 10; x++)
        tm.SetTile(new Vector3Int(x, y), null);
  }

  void GenerateRoad(Tilemap tm, int min, int max) {
    const int inner = 4;
    //FIXME    const int top = 10;
    //FIXME    const int top2 = 9;
    const int bottoml = 6;
    const int bottom = 13;

    //FIXME    const int leftt = 8;
    const int leftm = 0;
    const int leftm2 = 1;
    const int leftb = 5;

    //FIXME    const int rightt = 11;
    const int rightm = 2;
    const int rightm2 = 3;
    const int rightb = 7;

    TileBase t;
    for (int x = min; x < max + 1; x++) {
      for (int y = 0; y < 7; y++) {
        if (x == min) {
          if (y == 0) t = Roads[leftb];
          else t = Roads[leftm];
        }
        else if (x == min + 1) {
          if (y > 0) t = Roads[leftm2];
          else t = Roads[bottoml];
        }
        else if (x == max) {
          t = Roads[rightm2];
        }
        else if (x == max - 1) {
          if (y > 0) t = Roads[rightm];
          else t = Roads[rightb];
        }
        else if (y == 0) t = Roads[bottom];
        else t = Roads[inner];

        tm.SetTile(new Vector3Int(x - y, y), t);
      }
    }
  }


  void SetTile(Vector3Int pos, Tilemap tm, Color color, TileBase tile) {
    if (tm == buildingtm) {
      foreach (var rtm in rtmsEv) rtm.SetTile(pos, null);
      foreach (var rtm in rtmsOd) rtm.SetTile(pos, null);
    }
    tm.SetTile(pos, tile);
    tm.SetTileFlags(pos, TileFlags.None);
    tm.SetColor(pos, color);
  }

  void FillDoor(Vector3Int bl, int start, int end, WindowType wt, Tilemap tm) {
    int tlc = 0;
    int blc = wt.Height == 1 ? 0 : 3;
    int tc = 1;
    int bc = wt.Height == 1 ? 1 : 4;
    int trc = 2;
    int brc = wt.Height == 1 ? 2 : 5;

    if (wt.Height == 1) bl.y++; // Put one tile up in case it is a small window

    for (int y = 0; y < wt.Height; y++) {
      for (int x = start; x < end; x++) {
        if (x == start) tm.SetTile(bl + new Vector3Int(x, wt.Height - y - 1), wt.Tiles[y == 0 ? tlc : blc]);
        else if (x == end - 1) tm.SetTile(bl + new Vector3Int(x, wt.Height - y - 1), wt.Tiles[y == 0 ? trc : brc]);
        else tm.SetTile(bl + new Vector3Int(x, wt.Height - y - 1), wt.Tiles[y == 0 ? tc : bc]);
      }
    }
  }

  void PlaceDoors(Vector3Int bl, int start, int end, WindowType wt, Tilemap tm, int num) {
    int space = end - start; // How much space is available?
    if (num * wt.Width > space) num = space / wt.Width; // How many windows can we fit?
    if (num <= 0) return;
    float between = (float)space / num; // How much space for each window?

    Vector3Int wbl = bl;
    if (wt.Height == 1) wbl.y++;

    // Place the windows
    for (float x = (between - wt.Width) * .5f + start; x < end; x += between) {
      for (int wx = 0; wx < wt.Width; wx++) {
        for (int y = 0; y < wt.Height; y++) {
          tm.SetTile(wbl + new Vector3Int(Mathf.RoundToInt(x) + wx, wt.Height - y - 1), wt.Tiles[wx + wt.Width * y]);
        }
      }
      if (--num == 0) return;
    }
  }
}


[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    if (GUILayout.Button("Generate")) {
      BuildingGenerator b = target as BuildingGenerator;
      b.Clean();
      b.Generate(0);
    }
    if (GUILayout.Button("Generate from Hash")) {
      BuildingGenerator b = target as BuildingGenerator;
      b.CalculateValuesFromHash(b.RandomHash);
      b.Clean();
      b.Generate(0);
    }
    if (GUILayout.Button("Generate full road")) {
      BuildingGenerator b = target as BuildingGenerator;
      b.GenerateFullRoad();
    }
  }
}

[System.Serializable]
public class BuildMat {
  public string Name;
  public PiecePos mode;
  public List<TileBase> tiles;
}

public enum PiecePos {
  LeftRightMod2X,
  LeftRightMod2Y,
  LeftRightMod2XY,
  LeftRightRandom2,
  Mod4X,
  Mod4Y,
  Mod4XY,
  Random4,
  LeftLeftRight
}

[System.Serializable]
public class DoorType {
  public string Name;
  public int Width;
  public int Height;
  public List<TileBase> Tiles;
}

[System.Serializable]
public class WindowType {
  public string Name;
  public bool OnlyFloor;
  public bool Strecheable;
  public int Width;
  public int Height;
  public int SWidth;
  public int SHeight;
  public List<TileBase> Tiles;
  public List<TileBase> SideTiles;
}

