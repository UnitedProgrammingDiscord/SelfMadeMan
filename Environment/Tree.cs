using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour {
  SpriteRenderer sr;
  Transform player;

  private IEnumerator Start() {
    sr = GetComponent<SpriteRenderer>();
    yield return null;
    yield return null;
    yield return null;
    yield return null;
    player = Player.GetTransform();
  }

  bool wasBehind = false;
  void Update() {
    if (player == null) return;
    float val = (player.position - transform.position).sqrMagnitude;
    if (val < 5) {
      Color32 c = Color.white;
      c.a = (byte)(255 * Mathf.Clamp01(.2f + val * .16f));
      sr.color = c;
      wasBehind = true;
    }
    else if (wasBehind) {
      wasBehind = false;
      Color32 c = Color.white;
      c.a = 255;
      sr.color = c;
    }
  }
}
