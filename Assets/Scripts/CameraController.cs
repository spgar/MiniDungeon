using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera HeroCamera;
    public float HeroCameraZoomedInDistance;
    public float HeroCameraZoomedOutDistance;
    private bool heroCameraZoomedIn = true;

	void Start()
    {
        HeroCamera.transform.localPosition = new Vector3(HeroCamera.transform.localPosition.x, HeroCameraZoomedInDistance, HeroCamera.transform.localPosition.z);
	}
	
	void Update()
    {
		// Support zooming in and out to view the entire maze at once.
        if (Input.GetKeyUp(KeyCode.Z))
        {
            heroCameraZoomedIn = !heroCameraZoomedIn;
            float newYposition = HeroCameraZoomedInDistance;
            if (!heroCameraZoomedIn)
            {
                newYposition = HeroCameraZoomedOutDistance;
            }
            HeroCamera.transform.localPosition = new Vector3(HeroCamera.transform.localPosition.x, newYposition, HeroCamera.transform.localPosition.z);
        }
	}
}
