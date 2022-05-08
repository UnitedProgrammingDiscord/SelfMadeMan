using UnityEngine;

public class Player : MonoBehaviour {
  public Animator anim;
  public Transform cam;


  Vector3 movement;
  public float Speed = 3;
  Vector3 MoveRight = new Vector3(-1, 1, 1);
  Vector3 MoveLeft = Vector3.one;


  void Update() {
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

    float camDist = Mathf.Abs(transform.position.x - cam.transform.position.x);
    if (camDist > 2.5f) {
      Vector3 cp = cam.position;
      cp.x = Mathf.Lerp(cp.x, transform.position.x + Mathf.Sign(cam.transform.position.x - transform.position.x), camDist * Time.deltaTime * .5f);
      cam.position = cp;
    }

  }
}
