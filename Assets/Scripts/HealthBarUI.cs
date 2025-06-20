using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
   
    public Image fillImage;

    private Transform cameraTransform;

    void Start()
    {
     
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }

 
    public void SetHealthFraction(float fraction)
    {
        fillImage.fillAmount = Mathf.Clamp01(fraction);
    }
}
