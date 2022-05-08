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
    if (mx == 0) movement *= (1 - 15 * Time.deltaTime);
    else {
      movement.x += mx * Time.deltaTime;
      movement.x = Mathf.Clamp(movement.x, -Speed, Speed);
    }

    transform.position += Speed * Time.deltaTime * movement;
    if (Mathf.Abs(movement.x) < .1f) {
      anim.SetInteger("walk", 0);
      anim.speed = 1;
    }
    else {
      anim.SetInteger("walk", 1);
      anim.speed = Mathf.Clamp(Mathf.Abs(movement.x), 0.5f, 5);
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
