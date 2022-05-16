using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class Actor : MonoBehaviour {
  public Dir dir;
  public Gender gender;
  public Animator anim;
  public float Speed = 1;
  public SpritePart[] partsBody;
  public SpritePart[] partsClothes;
  public int AllAt = 0;
  public int AllAtClothes = 0;
  public Color32 SkinColor;

  public void UpdateSprites() {
    foreach (var part in partsBody) {
      try {
        string num = part.part.ToString();
        if (part.resS != null) { part.resS.SetCategoryAndLabel(part.Category + "S", num); if (part.IsSkin) part.resS.GetComponent<SpriteRenderer>().color = SkinColor; }
        if (part.resF != null) { part.resF.SetCategoryAndLabel(part.Category+"F", num); if (part.IsSkin) part.resF.GetComponent<SpriteRenderer>().color = SkinColor; }
        if (part.resB != null) { part.resB.SetCategoryAndLabel(part.Category+"B", num); if (part.IsSkin) part.resB.GetComponent<SpriteRenderer>().color = SkinColor; }
      } catch (System.Exception e) {
        Debug.LogException(e);
        Debug.Log(part.Category);
      }
    }
    foreach (var part in partsClothes) {
      try {
        string num = part.part.ToString();
        if (part.resS != null) { part.resS.SetCategoryAndLabel(part.Category + "S", num); if (part.IsSkin) part.resS.GetComponent<SpriteRenderer>().color = SkinColor; }
        if (part.resF != null) { part.resF.SetCategoryAndLabel(part.Category+"F", num); if (part.IsSkin) part.resF.GetComponent<SpriteRenderer>().color = SkinColor; }
        if (part.resB != null) { part.resB.SetCategoryAndLabel(part.Category+"B", num); if (part.IsSkin) part.resB.GetComponent<SpriteRenderer>().color = SkinColor; }
      } catch (System.Exception e) {
        Debug.LogException(e);
        Debug.Log(part.Category);
      }
    }
  }


  internal void Play(Anim what) {
    switch (what) {
      case Anim.Idle:
        anim.SetInteger("walk", 0);
        break;
      case Anim.Walk:
        anim.SetInteger("walk", 1);
        break;
      case Anim.Pick:
        anim.SetInteger("walk", 0);
        if (dir == Dir.Up) anim.Play("Pick Back");
        else if (dir == Dir.Down) anim.Play("Pick Front");
        else anim.Play("Pick Side");
        break;
    }
  }

  internal void SetDir(Dir newdir) {
    if (dir == newdir) return;
    switch (newdir) {
      case Dir.Up:
        if (dir != Dir.Up) anim.Play("Idle Back");
        break;
      case Dir.Down:
        if (dir != Dir.Down) anim.Play("Idle Front");
        break;
      case Dir.Left:
      case Dir.Right:
        if (dir != Dir.Left && dir != Dir.Right) anim.Play("Idle Side");
        break;
    }
    dir = newdir;
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
      foreach (var part in a.partsBody)
        part.part = a.AllAt;
      foreach (var part in a.partsClothes)
        part.part = a.AllAtClothes;
      a.UpdateSprites();
    }
  }
}

public enum Dir { Left, Right, Up, Down }
public enum Gender { Male, Female }
public enum Anim { Idle, Walk, Pick }

[System.Serializable]
public class SpritePart {
  public string Category;
  public SpriteResolver resS;
  public SpriteResolver resF;
  public SpriteResolver resB;
  public int part;
  public bool IsSkin;
}

[CustomPropertyDrawer(typeof(SpritePart))]
public class SpritePartDrawerUIE : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    position.x *= .5f;
    position.width += position.x;
    float w = position.width * .95f;
    var indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;

    // Calculate rects
    var rectcatg = new Rect(position.x + position.width * .00f, position.y, w * .5f, position.height * .5f);
    var rectpart = new Rect(position.x + position.width * .50f, position.y, w * .25f, position.height * .5f);
    var rectissk = new Rect(position.x + position.width * .75f, position.y, w * .25f, position.height * .5f);
    var rectress = new Rect(position.x + position.width * .00f, position.y + position.height * .5f, w * .3f, position.height * .5f);
    var rectresf = new Rect(position.x + position.width * .30f, position.y + position.height * .5f, w * .3f, position.height * .5f);
    var rectresb = new Rect(position.x + position.width * .60f, position.y + position.height * .5f, w * .3f, position.height * .5f);

    EditorGUI.PropertyField(rectcatg, property.FindPropertyRelative("Category"), GUIContent.none);
    EditorGUI.PropertyField(rectpart, property.FindPropertyRelative("part"), GUIContent.none);
    EditorGUI.PropertyField(rectissk, property.FindPropertyRelative("IsSkin"), GUIContent.none);
    EditorGUI.PropertyField(rectress, property.FindPropertyRelative("resS"), GUIContent.none);
    EditorGUI.PropertyField(rectresf, property.FindPropertyRelative("resF"), GUIContent.none);
    EditorGUI.PropertyField(rectresb, property.FindPropertyRelative("resB"), GUIContent.none);

    EditorGUI.indentLevel = indent;
    EditorGUI.EndProperty();
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    return 36;
  }
}

