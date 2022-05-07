using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public class BuildingGenerator : MonoBehaviour {
  public Tilemap buildingtm;
  public Tilemap doorstm;
  public Tilemap[] roofstm;
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

  [Header("Randomness")]
  public string RandomHash;
  public int NumberOfBuldings = 1;

  [Header("Reference assets")]
  public List<BuildMat> Mats;
  public List<DoorType> Doors;
  public List<WindowType> Windows;
  public List<TileBase> Roof;
  public List<TileBase> Roads;
}

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    if (GUILayout.Button("Generate")) {
      BuildingGenerator b = target as BuildingGenerator;
      Clean(b);
      Generate(b, 0);
    }
    if (GUILayout.Button("Generate from Hash")) {
      BuildingGenerator b = target as BuildingGenerator;
      CalculateValuesFromHash(b, b.RandomHash);
      Clean(b);
      Generate(b, 0);
    }
    if (GUILayout.Button("Generate full road")) {
      BuildingGenerator b = target as BuildingGenerator;

      Clean(b);
      int num = b.NumberOfBuldings;
      int pos = 0; // Calculate the last position
      for (int i = 0; i < num - 1; i++) {
        CalculateValuesFromHash(b, b.RandomHash + i);
        pos += b.SpaceBefore + b.Width;
        if (i == num - 1) pos -= b.Width;
      }
      CalculateValuesFromHash(b, b.RandomHash + (num - 1));
      for (int i = num - 1; i >= 0; i--) {
        Generate(b, i % 2, pos);
        CalculateValuesFromHash(b, b.RandomHash + (i - 1));
        pos -= b.SpaceBefore + b.Width;
      }
      // Roads
      CalculateValuesFromHash(b, b.RandomHash + 0);
      pos = b.SpaceBefore - 1;
      b.roadtm.SetTile(new Vector3Int(pos - 1, 1), b.Roads[0]);
      b.roadtm.SetTile(new Vector3Int(pos - 1, 0), b.Roads[10]);
      for (int i = 0; i < num; i++) {
        CalculateValuesFromHash(b, b.RandomHash + i);
        int width = b.SpaceBefore + b.Width;
        for (int x = 0; x < width; x++) {
          b.roadtm.SetTile(new Vector3Int(pos + x, 1), b.Roads[1]);
          b.roadtm.SetTile(new Vector3Int(pos + x, 0), b.Roads[10]);
        }
        pos += width;
      }
      b.roadtm.SetTile(new Vector3Int(pos + 0, 1), b.Roads[1]);
      b.roadtm.SetTile(new Vector3Int(pos + 1, 1), b.Roads[3]);
      b.roadtm.SetTile(new Vector3Int(pos + 0, 0), b.Roads[10]);
      b.roadtm.SetTile(new Vector3Int(pos + 1, 0), b.Roads[10]);
    }
  }

  Tilemap btm, rtm1, rtm2;

  void CalculateValuesFromHash(BuildingGenerator b, string hash) {
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


    b.SpaceBefore = (byte)(key & 3);
    b.Width = (byte)((key >> 2) & 7) + 4;
    b.Height = (byte)((key >> 5) & 3) + 3;
    b.DoorPosition = (byte)((key >> 7) & 7);
    b.DoorType = (byte)((key >> 10) & 7);
    b.WindowsType1 = (byte)((key >> 13) & 7);
    b.WindowsNum1 = (byte)((key >> 16) & 3);
    b.WindowsType2 = (byte)((key >> 18) & 7);
    b.WindowsNum2 = (byte)((key >> 21) & 7);
    b.MaterialType = (byte)((key >> 24) & 7);

    float h = (byte)((key >> 28) & 255) / 255f;
    float s = (byte)((key >> 36) & 15) / 45f + .025f;
    float v = (byte)((key >> 40) & 15) / 75f + .5f;
    Color32 hc = Color.HSVToRGB(h, s, v);
    hc.a = 255;
    b.WallsColor = hc;
    hc = Color.HSVToRGB(h < .5f ? h * h : 1 - (1 - h) * (1 - h), s * .5f, v * .8f);
    hc.a = 255;
    b.RoofColor = hc;
  }

  void Clean(BuildingGenerator b) {
    btm = b.buildingtm;
    rtm1 = b.roofstm[0];
    rtm2 = b.roofstm[1];
    for (int y = 0; y < 16; y++) {
      for (int x = -8; x < 128; x++) {
        Vector3Int cell = new(x, y, 0);
        b.buildingtm.SetTile(cell, null);
        b.doorstm.SetTile(cell, null);
        b.roofstm[0].SetTile(cell, null);
        b.roofstm[1].SetTile(cell, null);
        b.roadtm.SetTile(cell, null);
      }
    }
  }

  void Generate(BuildingGenerator b, int rm, int xoffset = 0) {
    Vector3Int bl = new(b.SpaceBefore + xoffset, 3);
    BuildMat mat = b.Mats[b.MaterialType % b.Mats.Count];
    Color32 col = b.WallsColor;
    col.a = 255;
    Color32 colroof = b.RoofColor;
    colroof.a = 255;
    int depth = b.SpaceBefore;
    if (depth < 2) depth = 2;

    // Side walls
    for (int x = 0; x < Mathf.Min(depth, b.Height); x++) {
      Vector3Int pos = new Vector3Int(-x - 1, x) + bl;
      SetTile(pos, b.buildingtm, col, mat.tiles[6]);
      for (int y = 1; y < b.Height; y++) {
        pos = new Vector3Int(-x - 1, y + x) + bl;
        SetTile(pos, b.buildingtm, col, mat.tiles[5]);
      }
      pos = new Vector3Int(-x - 1, b.Height + x) + bl;
      SetTile(pos, b.buildingtm, col, mat.tiles[4]);
    }

    // Left walls
    for (int y = 0; y < b.Height; y++) {
      Vector3Int pos = new Vector3Int(0, y) + bl;
      if (mat.mode == PiecePos.LeftRightRandom2 || mat.mode == PiecePos.LeftRightMod2X || mat.mode == PiecePos.LeftRightMod2Y || mat.mode == PiecePos.LeftRightMod2XY)
        SetTile(pos, b.buildingtm, col, mat.tiles[0]);
      else if (mat.mode == PiecePos.Random4)
        SetTile(pos, b.buildingtm, col, mat.tiles[Random.Range(0, 4)]);
      else if (mat.mode == PiecePos.Mod4X)
        SetTile(pos, b.buildingtm, col, mat.tiles[b.SpaceBefore % 4]);
      else if (mat.mode == PiecePos.Mod4X)
        SetTile(pos, b.buildingtm, col, mat.tiles[y % 4]);
      else if (mat.mode == PiecePos.Mod4XY)
        SetTile(pos, b.buildingtm, col, mat.tiles[(b.SpaceBefore + y) % 4]);
      else if (mat.mode == PiecePos.LeftLeftRight)
        SetTile(pos, b.buildingtm, col, mat.tiles[0]);
    }
    // Center walls
    for (int x = 1; x < b.Width - 1; x++) {
      for (int y = 0; y < b.Height; y++) {
        Vector3Int pos = new Vector3Int(x, y) + bl;
        if (mat.mode == PiecePos.LeftRightRandom2)
          SetTile(pos, b.buildingtm, col, mat.tiles[Random.Range(1, 3)]);
        else if (mat.mode == PiecePos.LeftRightMod2X)
          SetTile(pos, b.buildingtm, col, mat.tiles[pos.x % 2 + 1]);
        else if (mat.mode == PiecePos.LeftRightMod2Y)
          SetTile(pos, b.buildingtm, col, mat.tiles[pos.y % 2 + 1]);
        else if (mat.mode == PiecePos.LeftRightMod2XY)
          SetTile(pos, b.buildingtm, col, mat.tiles[(pos.x + pos.y) % 2 + 1]);
        else if (mat.mode == PiecePos.Random4)
          SetTile(pos, b.buildingtm, col, mat.tiles[Random.Range(0, 4)]);
        else if (mat.mode == PiecePos.Mod4X)
          SetTile(pos, b.buildingtm, col, mat.tiles[pos.x % 4]);
        else if (mat.mode == PiecePos.Mod4X)
          SetTile(pos, b.buildingtm, col, mat.tiles[pos.y % 4]);
        else if (mat.mode == PiecePos.Mod4XY)
          SetTile(pos, b.buildingtm, col, mat.tiles[(pos.x + pos.y) % 4]);
        else if (mat.mode == PiecePos.LeftLeftRight)
          SetTile(pos, b.buildingtm, col, mat.tiles[0]);
      }
    }
    // Right walls
    for (int y = 0; y < b.Height; y++) {
      Vector3Int pos = new Vector3Int(b.Width - 1, y) + bl;
      if (mat.mode == PiecePos.LeftRightRandom2 || mat.mode == PiecePos.LeftRightMod2X || mat.mode == PiecePos.LeftRightMod2Y || mat.mode == PiecePos.LeftRightMod2XY)
        SetTile(pos, b.buildingtm, col, mat.tiles[3]);
      else if (mat.mode == PiecePos.Random4)
        SetTile(pos, b.buildingtm, col, mat.tiles[Random.Range(0, 4)]);
      else if (mat.mode == PiecePos.Mod4X)
        SetTile(pos, b.buildingtm, col, mat.tiles[b.SpaceBefore % 4]);
      else if (mat.mode == PiecePos.Mod4X)
        SetTile(pos, b.buildingtm, col, mat.tiles[y % 4]);
      else if (mat.mode == PiecePos.Mod4XY)
        SetTile(pos, b.buildingtm, col, mat.tiles[(b.SpaceBefore + y) % 4]);
      else if (mat.mode == PiecePos.LeftLeftRight)
        SetTile(pos, b.buildingtm, col, mat.tiles[3]);
    }

    // Door
    DoorType dt = b.Doors[b.DoorType % b.Doors.Count];
    int doorp = b.DoorPosition % b.Width;
    if (doorp < 0) doorp = 0;
    if (doorp + dt.Width > b.Width) doorp = b.Width - dt.Width;
    for (int y = 0; y < dt.Height; y++) {
      for (int x = 0; x < dt.Width; x++) {
        b.doorstm.SetTile(bl + new Vector3Int(doorp + x, dt.Height - y - 1), dt.Tiles[x + dt.Width * y]);
      }
    }

    // Windows (low level)
    WindowType wt = b.Windows[b.WindowsType1 % b.Windows.Count];
    int num = b.WindowsNum1;
    if (num > 0) {
      if (wt.Strecheable) { // If the window is strechable try to fit the space
        int spaceBeforeDoor = doorp;
        int spaceAfterDoor = b.Width - doorp - 1;
        if (b.WindowsNum1 == 1) { // Fill the biggest space
          if (spaceBeforeDoor >= spaceAfterDoor) FillDoor(bl, 0, doorp, wt, b.doorstm);
          else FillDoor(bl, doorp + 1, b.Width, wt, b.doorstm);
        }
        else { // Fill both sides
          FillDoor(bl, 0, doorp, wt, b.doorstm);
          FillDoor(bl, doorp + dt.Width, b.Width, wt, b.doorstm);
        }
      }
      else { // Not streacheable, try to put the specified numebr of windows on each side of the door
             // split the number in two parts, balancing by space
        int numbef = b.WindowsNum1 / 2;
        int numaft = b.WindowsNum1 / 2;
        if (doorp == 0) {
          numbef = 0;
          numaft = b.WindowsNum1;
        }
        else if (doorp >= b.Width - wt.Width) {
          numbef = b.WindowsNum1;
          numaft = 0;
        }
        else if (numbef + numaft < b.WindowsNum1) {
          if (doorp > b.Width / 2) numbef++;
          else numaft++;
        }

        PlaceDoors(bl, 0, doorp, wt, b.doorstm, numbef);
        PlaceDoors(bl, doorp + dt.Width, b.Width, wt, b.doorstm, numaft);
      }
    }

    // Windows (top levels)
    wt = b.Windows[b.WindowsType2 % b.Windows.Count];
    num = b.WindowsNum2;
    if (num > 0) {
      for (int y = 2; y < b.Height - 1; y += 2) {
        Vector3Int wbl = bl;
        wbl.y += y;

        if (wt.Strecheable) { // If the window is strechable try to fit the space
          if (b.WindowsNum1 == 1) { // Fill the biggest space
            FillDoor(wbl, 0, b.Width, wt, b.doorstm);
          }
          else { // Fill by num
            int size = b.Width / b.WindowsNum2;
            if (size < 1) size = 1;
            for (int i = 0; i < b.WindowsNum2; i++) {
              FillDoor(wbl, i * size, (i + 1) * size, wt, b.doorstm);
            }
          }
        }
        else { // Not streacheable, try to put the specified numebr of windows on each side of the door
          PlaceDoors(wbl, 0, b.Width, wt, b.doorstm, b.WindowsNum2);
        }
      }
    }

    // Roof
    for (int x = -1; x < b.Width; x++) {
      for (int y = 0; y < depth; y++) {
        if (x == -1) SetTile(bl + new Vector3Int(x - y, b.Height + y), b.roofstm[rm], colroof, y == depth - 1 ? b.Roof[1] : b.Roof[0]);
        else if (x == b.Width - 1) SetTile(bl + new Vector3Int(x - y, b.Height + y), b.roofstm[rm], colroof, y == 0 ? b.Roof[3] : b.Roof[2]);
        else if (y == 0) SetTile(bl + new Vector3Int(x - y, b.Height + y), b.roofstm[rm], colroof, b.Roof[4]);
        else if (y == depth - 1) SetTile(bl + new Vector3Int(x - y, b.Height + y), b.roofstm[rm], colroof, b.Roof[6]);
        else SetTile(bl + new Vector3Int(x - y, b.Height + y), b.roofstm[rm], colroof, b.Roof[5]);
      }
    }
  
    // Roads
    


    
  }



  private void SetTile(Vector3Int pos, Tilemap tm, Color color, TileBase tile) {
    if (tm == btm) {
      rtm1.SetTile(pos, null);
      rtm2.SetTile(pos, null);
    }
    tm.SetTile(pos, tile);
    tm.SetTileFlags(pos, TileFlags.None);
    tm.SetColor(pos, color);
  }

  private void FillDoor(Vector3Int bl, int start, int end, WindowType wt, Tilemap tm) {
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

  private void PlaceDoors(Vector3Int bl, int start, int end, WindowType wt, Tilemap tm, int num) {
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
  public List<TileBase> Tiles;
}

