using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour {
  public Animator anim;
  public Transform cam;
  public BuildingGenerator building;
  public SpriteRenderer SkyDay, SkyNight;
  public Light2D GlobalLight;
  public static Transform mt;

  internal static Transform GetTransform() {
    return mt;
  }

  private void Awake() {
    mt = this.transform;
  }

  public TextMeshProUGUI TimeText;
  public static float DayTime = 9;
  [Range(0,24)] public float dayTime = 9;
  float TimeSpeed = 1 / 12f;

  Vector3 movement;
  public float Speed = 3;
  Vector3 MoveRight = new(-1, 1, 1);
  Vector3 MoveLeft = Vector3.one;

  private void Start() {
    building.GenerateFullRoad();
  }

  bool pickingUp = false;
  void Update() {
    if (!pickingUp) {
      HandleMovement();

      if (Input.GetKeyDown(KeyCode.LeftControl)) {
        // Do we have any pickable obejct under us?
        Collider2D coll = Physics2D.OverlapCircle(transform.position, 2);
        if (coll != null && coll.gameObject != null) {
          // FIXME check if it is a pickable item
          // FIXME Check if we can pick the object (space in inventory)
          movement = Vector3.zero;
          anim.SetInteger("walk", 0);
          anim.Play("Pick");
          pickingUp = true;
        }
      }
    }

    HandleCameraFollow();
    HandleTimeOfDay();
  }

  public void CompletePickup() {
    pickingUp = false;
  }

  void HandleMovement() {
    float mx = Input.GetAxis("Horizontal");
    if (mx == 0) movement.x *= (1 - 15 * Time.deltaTime);
    else {
      movement.x += mx * Time.deltaTime;
      movement.x = Mathf.Clamp(movement.x, -Speed, Speed);
    }
    float my = Input.GetAxis("Vertical");
    if (my == 0) movement.y *= (1 - 25 * Time.deltaTime);
    else {
      movement.y += my * Time.deltaTime * .5f;
      movement.y = Mathf.Clamp(movement.y, -Speed * .1f, Speed * .1f);
    }

    Vector3 pos = transform.position + Speed * Time.deltaTime * movement;
    pos.y = Mathf.Clamp(pos.y, -2.7f, -1.1f);
    transform.position = pos;

    if (Mathf.Abs(movement.x) < .1f && Mathf.Abs(movement.y) < .025f) {
      anim.SetInteger("walk", 0);
      anim.speed = 1;
    }
    else {
      anim.SetInteger("walk", 1);
      anim.speed = Mathf.Clamp(movement.sqrMagnitude, 0.5f, 2.5f);
      if (movement.x > 0) transform.localScale = MoveRight;
      else transform.localScale = MoveLeft;
    }
  }

  void HandleCameraFollow() {
  float camDist = Mathf.Abs(transform.position.x - cam.transform.position.x);
    if (camDist > 2.5f) {
      Vector3 cp = cam.position;
      cp.x = Mathf.Lerp(cp.x, transform.position.x + Mathf.Sign(cam.transform.position.x - transform.position.x), camDist * Time.deltaTime * .5f);
      cam.position = cp;
    }
  }

  public float sun = 0;
  public byte val = 0;

  float timeUpdateDelay = 0;
  void HandleTimeOfDay() {
    dayTime += Time.deltaTime * TimeSpeed;
    if (dayTime >= 24) dayTime -= 24f;
    Color32 c = Color.white;

    if (dayTime < 6) c.a = 0;
    else if (dayTime < 8) c.a = (byte)(255 * (dayTime - 6) * .5f);
    else if (dayTime <= 19) c.a = 255;
    else if (dayTime < 22) c.a = (byte)(255 * (22 - dayTime) * .3333334f);
    else c.a = 0;
    SkyDay.color = c;
    val = c.a;
    c.a = (byte)(255 - c.a);
    SkyNight.color = c;


    if (dayTime < 5) GlobalLight.intensity = .2f;
    else if (dayTime < 9) GlobalLight.intensity = .25f * dayTime - 1.05f;
    else if (dayTime < 20) GlobalLight.intensity = 1.2f;
    else if (dayTime < 23) GlobalLight.intensity = -.25f * dayTime + 5.95f;
    else GlobalLight.intensity = .2f;

    timeUpdateDelay += Time.deltaTime;
    if (timeUpdateDelay > 1) {
      timeUpdateDelay = 0;
      TimeText.text = (int)dayTime + ":" + ((dayTime - (int)dayTime) * 60).ToString("00");
    }

    DayTime = dayTime;
  }

  public void SetTimeSpeed(int val) {
    if (val == 0) TimeSpeed = 1/12f;  // 5 mins each second -> 12 second per 1 hour
    if (val == 1) TimeSpeed = 1/6f;   // 10 mins each second -> 6 seconds per 1 hour
    if (val == 2) TimeSpeed = .5f;    // 30 mins each second -> 2 seconds per 1 hour
    if (val == 3) TimeSpeed = 1f;     // 1 hour per second
    if (val == 4) TimeSpeed = 2f;     // 2 hours per second
  }
}
