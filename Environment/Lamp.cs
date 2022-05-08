using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lamp : MonoBehaviour {
  public Light2D Light;

  void Update() {
    Light.enabled = (Player.DayTime < 6 || Player.DayTime > 22);
  }
}
