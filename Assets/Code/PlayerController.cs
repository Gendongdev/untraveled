using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public ScreenController screen;

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

	[System.NonSerialized]
	public Vector2 velocity;
	[System.NonSerialized]
	public Vector2 movement;
	[System.NonSerialized]
	public bool isTouchingGround;
	[System.NonSerialized]
	public bool hasPendingJump;

	void Start() {
		velocity = Vector2.zero;
		movement = Vector2.zero;
		isTouchingGround = false;
		hasPendingJump = false;
	}

	void Update() {
		if (Input.GetButtonDown("Jump"))
			hasPendingJump = true;
	}

	void FixedUpdate() {
		velocity.x = (Input.GetButton("Left") ? -horizontalSpeed : 0) + (Input.GetButton("Right") ? horizontalSpeed : 0);

		if (!isTouchingGround)
			velocity.y = Mathf.Max(-terminalVelocity, velocity.y - gravity * Time.fixedDeltaTime);

		if (hasPendingJump) {
			if (isTouchingGround) {
				velocity.y = jumpSpeed;
			}
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

	void OnDrawGizmos() {
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(Vector3.up * lifterRadius, lifterRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Vector3.up * bumperHeight, bumperRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawLine (Vector3.up * (bumperHeight + bumperRadius), Vector3.up * 1.6f);
		Gizmos.DrawLine(Vector3.up * 1.6f + Vector3.left * bumperRadius, Vector3.up * 1.6f + Vector3.right * bumperRadius);
		Gizmos.DrawWireSphere (Vector3.up * 1.8f, 0.2f);
	}
}
