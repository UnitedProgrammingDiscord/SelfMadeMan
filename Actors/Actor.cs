using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;

public class Actor : MonoBehaviour {
  public Dir dir;
  public Gender gender;
  public Animator anim;
  public float Speed = 1;
  public SpritePart[] parts;

  public void UpdateSprites() {
    foreach(var part in parts) {
      try {
        part.resolver.SetCategoryAndLabel(part.Category, part.Category + part.part);
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
  }
}

public enum Dir { Left, Right, Up, Down }
public enum Gender { Male, Female }

[System.Serializable]
public class SpritePart {
  public string Category;
  public SpriteResolver resolver;
  public int part;
}

