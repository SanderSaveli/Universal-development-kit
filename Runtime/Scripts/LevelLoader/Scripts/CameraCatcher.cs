using UnityEngine;

public class CameraCatcher : MonoBehaviour
{
    void Start()
    {
        Canvas cnv = GetComponent<Canvas>();
        cnv.worldCamera = Camera.main;
        cnv.planeDistance = 1;
        cnv.transform.SetAsLastSibling();
    }
}
