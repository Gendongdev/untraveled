using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public ScreenController screen;
	public Transform model;

	[Header("Collision")]
	[Range(0, 1)]
	public float bumperRadius = 0.4f;
	[Range(0, 1)]
	public float bumperHeight = 0.6f;
	[Range(0, 0.1f)]
	public float bumperTolerance = 0.01f;
	[Range(0, 1)]
	public float lifterRadius = 0.3f;
	[Range(0, 0.1f)]
	public float lifterTolerance = 0.01f;
	[Range(0, 2)]
	public float lifterDistance = 0.5f;

	[Header("Dynamics")]
	[Range(0, 100)]
	public float gravity = 20;
	[Range(0, 100)]
	public float terminalVelocity = 50;
	[Range(0, 20)]
	public float horizontalSpeed = 6;
	[Range(0, 20)]
	public float jumpSpeed = 10;

	[Header("Sounds")]
	public AudioClip run1;
	public AudioClip run2;
	public AudioClip land;
	public AudioClip pickup;
	public AudioClip button_success;
	public AudioClip button_fail;
	public AudioClip move_rock;
	public AudioSource wind;

	[System.NonSerialized]
	public Vector2 velocity;
	[System.NonSerialized]
	public Vector2 movement;
	[System.NonSerialized]
	public float lastImpactVelocity;

	[System.NonSerialized]
	public bool isTouchingGround;
	[System.NonSerialized]
	public bool hasPendingJump;
	[System.NonSerialized]
	public float runWheel;
	[System.NonSerialized]
	public bool runWheelToggle;

	[System.NonSerialized]
	Animator animator;
	[System.NonSerialized]
	float orientation;
	[System.NonSerialized]
	float smoothedSpeed;
	[System.NonSerialized]
	int runningFlag = Animator.StringToHash("Run Speed");


	void Start() {
		velocity = Vector2.zero;
		movement = Vector2.zero;
		lastImpactVelocity = 0;
		isTouchingGround = false;
		hasPendingJump = false;
		animator = model.gameObject.GetComponentInChildren<Animator>();
		orientation = 0.5f;
		smoothedSpeed = 0;
	}

	void OnEnable() {
		animator = model.gameObject.GetComponentInChildren<Animator>();
	}

	void Update() {
		if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Up"))
			hasPendingJump = true;

		var left = Quaternion.LookRotation(Vector3.left + Vector3.back * 0.01f);
		var right = Quaternion.LookRotation(Vector3.right + Vector3.back * 0.01f);
		if (movement.x != 0) {
			orientation = Mathf.Clamp01(orientation + 0.5f * movement.x / Time.fixedDeltaTime * Time.deltaTime);
			model.localRotation = Quaternion.Slerp(left, right, orientation);
		} else if (!isTouchingGround) {
			orientation = Mathf.Lerp(orientation, 0.35f, Time.deltaTime);
			model.localRotation = Quaternion.Slerp(left, right, orientation);
		}

		if (isTouchingGround)
			smoothedSpeed = Mathf.Lerp(smoothedSpeed, Mathf.Abs(movement.x) / Time.fixedDeltaTime / horizontalSpeed, 10.0f * Time.deltaTime);
		else
			smoothedSpeed = Mathf.Lerp(smoothedSpeed, 0.3f, Time.deltaTime);

		if (isTouchingGround)
			wind.volume = Mathf.Lerp(wind.volume, 0.0f, 3.0f * Time.deltaTime);
		else
			wind.volume = Mathf.Lerp(wind.volume, 0.15f + 0.03f * Mathf.Sin(10 * Time.time), 0.1f * Mathf.Abs(movement.y) / Time.fixedDeltaTime * Time.deltaTime);

		animator.SetFloat(runningFlag, smoothedSpeed);

		// This is required to trigger OnTriggerEnter on other triggers...
		transform.localPosition = transform.localPosition;

		
		if (isTouchingGround && (Input.GetButtonDown("Left") || Input.GetButtonDown("Right"))) {
			GetComponent<AudioSource>().PlayOneShot(runWheelToggle ? run1 : run2);
			runWheel = 0;
			runWheelToggle = !runWheelToggle;
		}
	}

	void FixedUpdate() {
		velocity.x = (Input.GetButton("Left") ? -horizontalSpeed : 0) + (Input.GetButton("Right") ? horizontalSpeed : 0);

		if (!isTouchingGround)
			velocity.y = Mathf.Max(-terminalVelocity, velocity.y - gravity * Time.fixedDeltaTime);

		if (hasPendingJump) {
			if (isTouchingGround)
				velocity.y = jumpSpeed;
			hasPendingJump = false;
		}

		movement = velocity * Time.fixedDeltaTime;

		RaycastHit hitInfo;
		var lifterStart = Vector3.up * (lifterRadius + lifterDistance);
		if (WrappedSphereCast(lifterStart, lifterRadius, Vector3.down, out hitInfo, lifterDistance)) {
			// Move along the surface instead of horizontally
			movement.x = hitInfo.normal.y * movement.x;
			movement.y += -hitInfo.normal.x * movement.x;
			// Move out of the colliding element
			movement.y += Mathf.Max(0, lifterDistance - hitInfo.distance - lifterTolerance);
			// Kill vertical velocity
			if (velocity.y < 0) {
				lastImpactVelocity = velocity.y;
				if (velocity.y < -5.0f)
					GetComponent<AudioSource>().PlayOneShot(land);
			}
			velocity.y = Mathf.Max(velocity.y, 0);
			isTouchingGround = true;
		} else {
			isTouchingGround = false;
		}

		Vector3 movement3 = movement;
		var bumperStart = Vector3.up * bumperHeight;
		if (WrappedSphereCast(bumperStart, bumperRadius, movement3.normalized, out hitInfo, movement3.magnitude)) {
			var allowedDistance = Mathf.Max(0, hitInfo.distance - bumperTolerance);
			var allowedMovement = movement3.normalized * allowedDistance;
			// We get a normal from the bumper sphere instead of the hit surface, and remove the z component
			var hitNormal = Vector3.Scale((bumperStart + allowedMovement) - hitInfo.point, new Vector3(1, 1, 0)).normalized;
			// Kill vertical velocity if we hit a ceiling
			if (hitNormal.y < -Mathf.Sqrt(2) * 0.5f)
				velocity.y = Mathf.Min(0, velocity.y);
			// Use the rest of the movement to slide against the hit surface
			var unusedDistance = movement3.magnitude - allowedDistance;
			var slidingDirection = (movement3 + hitNormal * Vector3.Dot(movement3, -hitNormal)).normalized;
			var slidingDistance = unusedDistance;
			// Avoid manufacturing upward movement
			if (slidingDirection.y > 0 && slidingDistance > movement.y / slidingDirection.y) 
				slidingDistance = Mathf.Max(0, movement.y / slidingDirection.y);
			if (WrappedSphereCast(bumperStart + allowedMovement, bumperRadius, slidingDirection, out hitInfo, slidingDistance))
				movement = allowedMovement;
			else
				movement = allowedMovement + slidingDirection * slidingDistance;
		}

		if (isTouchingGround) {
			runWheel += Mathf.Abs(movement.x) * 0.75f;
			if (runWheel > 1) {
				GetComponent<AudioSource>().PlayOneShot(runWheelToggle ? run1 : run2);
				runWheel -= 1;
				runWheelToggle = !runWheelToggle;
			}
		}
	}

	bool WrappedSphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance) {
		var rightVector = Vector3.right * screen.size.x;
		var upVector = Vector3.up * screen.size.y;

		RaycastHit hitInfoRight;
		RaycastHit hitInfoUp;
		RaycastHit hitInfoUpRight;

		var hasHit = Physics.SphereCast(origin, radius, direction, out hitInfo, distance);
		var hasHitRight = Physics.SphereCast(origin + rightVector, radius, direction, out hitInfoRight, distance);
		var hasHitUp = Physics.SphereCast(origin + upVector, radius, direction, out hitInfoUp, distance);
		var hasHitUpRight = Physics.SphereCast(origin + upVector + rightVector, radius, direction, out hitInfoUpRight, distance);

		if (!hasHit)
			hitInfo.distance = Mathf.Infinity;
		if (hasHitRight && hitInfoRight.distance < hitInfo.distance) {
			hitInfo = hitInfoRight;
			hitInfo.point -= rightVector;
			hasHit = true;
		}
		if (hasHitUp && hitInfoUp.distance < hitInfo.distance) {
			hitInfo = hitInfoUp;
			hitInfo.point -= upVector;
			hasHit = true;
		}
		if (hasHitUpRight && hitInfoUpRight.distance < hitInfo.distance) {
			hitInfo = hitInfoUpRight;
			hitInfo.point -= upVector + rightVector;
			hasHit = true;
		}
		return hasHit;
	}

	public void PlayPickup() {
		GetComponent<AudioSource>().PlayOneShot(pickup);
	}

	public void PlayButtonSuccess() {
		GetComponent<AudioSource>().PlayOneShot(button_success);
	}

	public void PlayButtonFail() {
		GetComponent<AudioSource>().PlayOneShot(button_fail);
	}

	public void PlayMoveRock() {
		GetComponent<AudioSource>().PlayOneShot(move_rock);
	}

	void OnDrawGizmos() {
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(Vector3.up * lifterRadius, lifterRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Vector3.up * bumperHeight, bumperRadius);
	}
}
