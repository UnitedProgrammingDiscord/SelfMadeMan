using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System;

public class Actor : MonoBehaviour {
  public Dir dir;
  public Gender gender;
  public Animator anim;
  public float Speed = 1;
  public SpritePart[] parts;
  public int AllAt = 0;

  public void UpdateSprites() {
    foreach (var part in parts) {
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
      foreach (var part in a.parts)
        part.part = a.AllAt;
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
}

