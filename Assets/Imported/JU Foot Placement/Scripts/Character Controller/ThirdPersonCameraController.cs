
// >>> This is just a simple third-person camera controller, used only for testing, it is not necessary for the main system to function (Foot Placement)

using UnityEngine;

[AddComponentMenu("JU Foot Placement/Player Controller/Third Person Camera Controller")]
public class ThirdPersonCameraController : MonoBehaviour
{	
	
	[JUSubHeader("Camera Target")]
	[JUHeader("Camera Controller Settings")]
	public GameObject Target;
    public LayerMask CameraCollisionLayer;
	public bool AutoRotation;
	public float Sensibility = 500f;
	public float CameraMovementSpeed = 8f;
	public float CameraRotationSpeed = 6f;
	[JUSubHeader("Positions Offsets")]
	public float Distance = 3f;
	private float CurrentDistance;
	public float CameraHeight = 0f;
	public float PositionX = 0.5f;
	public float PositionY = 0f ;

	[JUSubHeader("Camera Rotation Limit")]
	public float MinRotation = -80f;
	public float MaxRotation = 80f;

	//Camera Rotation
	float DesiredXRotation;
	float DesiredYRotation;

	float XMouseInput;
	float YMouseInput;

	private float SmoothedXRotation;
	private float SmoothedYRotation;
	private Quaternion CameraCurrentRotation;

	void Start()
	{
		//Get Target
		if (Target == null)
			Target = GameObject.FindGameObjectWithTag("Player");

		if (Target.GetComponent<Animator>().isHuman)
		{
			Target = Target.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips).gameObject;
		}
		//Lock Mouse
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
    private void LateUpdate()
    {

		//Camera Position
		transform.position = Vector3.Slerp(transform.position, Target.transform.position + Target.transform.up * CameraHeight - transform.forward * CurrentDistance + transform.right * PositionX + transform.up * PositionY, CameraMovementSpeed * Time.deltaTime);

		SmoothedXRotation = Mathf.Lerp(SmoothedXRotation, DesiredXRotation, CameraRotationSpeed * Time.deltaTime);
		SmoothedYRotation = Mathf.LerpAngle(SmoothedYRotation, DesiredYRotation, CameraRotationSpeed * Time.deltaTime);

		CameraCurrentRotation = Quaternion.Euler(SmoothedXRotation, SmoothedYRotation, 0);
	}

	public void Update()
    {
		//Camera Collision
		RaycastHit hit;
		Vector3 start = Target.transform.position + Target.transform.up * CameraHeight;
		Vector3 end = Target.transform.position + Target.transform.up * CameraHeight - transform.forward * CurrentDistance + transform.right * PositionX + transform.up * PositionY;
		if (Physics.Linecast(start, end, out hit, CameraCollisionLayer))
		{
			CurrentDistance = Vector3.Distance(start, hit.point);
        }
        else
        {
			CurrentDistance = Distance;
        }

		//Camera Rotation
		XMouseInput = Input.GetAxis("Mouse Y");
        YMouseInput = Input.GetAxis("Mouse X");

        DesiredXRotation -= XMouseInput * Sensibility * Time.deltaTime;
		if (AutoRotation == false)
		{
			DesiredYRotation += YMouseInput * Sensibility * Time.deltaTime;
        }
        else
        {
			DesiredYRotation = Target.transform.eulerAngles.y;
		}
        DesiredXRotation = Mathf.Clamp(DesiredXRotation, MinRotation, MaxRotation);

		transform.rotation = CameraCurrentRotation;
    }
    void OnDrawGizmos(){
		var poscam = transform.position - transform.forward * Distance + transform.right * PositionX;
		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position,poscam);
		Gizmos.DrawSphere (poscam, 0.1f);
	}
}