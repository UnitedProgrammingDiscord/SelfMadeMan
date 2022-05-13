using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;

public class Actor : MonoBehaviour {
  public Dir dir;
  public Gender gender;
  public Animator anim;
  public float Speed = 1;
  public SpritePart[] parts;
  public int AllAt = 0;

  public void UpdateSprites() {
    foreach(var part in parts) {
      try {
        part.resS.SetCategoryAndLabel(part.Category, part.Category + part.part);
        part.resF.SetCategoryAndLabel(part.Category, part.Category + part.part);
        part.resB.SetCategoryAndLabel(part.Category, part.Category + part.part);
      } catch (System.Exception e) {
        Debug.LogException(e);
        Debug.Log(part.Category);
      }
    }
  }
}

[CustomEditor(typeof(Actor))]
public class ActorEditor : Editor {
  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    if (GUILayout.Button("Update")) {
      (target as Actor).UpdateSprites();
    }
    if (GUILayout.Button("Update with Val")) {
      Actor a = target as Actor;
      foreach (var part in a.parts)
        part.part = a.AllAt;
      a.UpdateSprites();
    }
  }
}

public enum Dir { Left, Right, Up, Down }
public enum Gender { Male, Female }

[System.Serializable]
public class SpritePart {
  public string Category;
  public SpriteResolver resS;
  public SpriteResolver resF;
  public SpriteResolver resB;
  public int part;
}

