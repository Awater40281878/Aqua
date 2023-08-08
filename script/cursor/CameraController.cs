using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController instance;
	bool LookCamera = false;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public Transform centerCamera;
	public float distance = 10f;
	public float minDistance = 2f;
	public float maxDistance = 20f;
	public float scrollSpeed = 5f;
	public float rotationSpeed = 5f;

	public float realRotationY = 0f;
	public float rotationX = 0f;
	private float minRotationX = 15f;
	private float maxRotationX = 65f;

	public polar ViewFont = polar.Southwest;
	public string ViewFonttext;
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			LookCamera = !LookCamera;
		}
		if (LookCamera==true)
		{
			return;
		}
		// 获取滚轮滚动的值
		float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

		// 计算目标距离
		distance -= scrollDelta * scrollSpeed;
		distance = Mathf.Clamp(distance, minDistance, maxDistance);

		// 计算目标位置
		Vector3 targetPosition = centerCamera.position + centerCamera.forward * -distance;

		// 平滑移动相机位置
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * scrollSpeed);

		// 获取滑鼠移动的值
		float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
		float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

		// 应用旋转
		realRotationY += mouseX;
		realRotationY = realRotationY < 0f ? realRotationY + 360f : realRotationY % 360f;
		if (Mathf.Abs(realRotationY - 45f) < Mathf.Epsilon)
		{
			realRotationY += 0.1f;

		}
		else if (Mathf.Abs(realRotationY - 135f) < Mathf.Epsilon)
		{
			realRotationY += 0.1f;

		}
		else if (Mathf.Abs(realRotationY - 225f) < Mathf.Epsilon)
		{
			realRotationY += 0.1f;

		}
		else if (Mathf.Abs(realRotationY - 315f) < Mathf.Epsilon)
		{
			realRotationY += 0.1f;

		}
		//獲取朝向
		if (realRotationY > 0.1 && realRotationY < 89.9)
		{
			ViewFont = polar.Northeast;
			ViewFonttext = ViewFont.ToString();
		}
		else
		{
			if (realRotationY > 90.1 && realRotationY < 179.9)
			{
				ViewFont = polar.Southeast;
				ViewFonttext = ViewFont.ToString();
			}
			else
			{
				if (realRotationY > 180.1 && realRotationY < 269.9)
				{

					ViewFont = polar.Southwest;
					ViewFonttext = ViewFont.ToString();
				}
				else
				{
					if (realRotationY > 270.1 || realRotationY < 359.9)
					{
						ViewFont = polar.Northwest;
						ViewFonttext = ViewFont.ToString();
					}
				}
			}
		}

		rotationX -= mouseY;
		rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);

		centerCamera.rotation = Quaternion.Euler(rotationX, realRotationY, 0f);

		// 调整相机朝向
		transform.LookAt(centerCamera);
	}
}

