using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private CinemachineFreeLook freelookCam;
    private bool canRotate = false;
    
    
    void Start()
    {
        freelookCam = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        canRotate = Input.GetMouseButton(1);
        
        if(canRotate)
        {
            freelookCam.m_XAxis.m_InputAxisName = "Mouse X";
            freelookCam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
        else
        {
            freelookCam.m_XAxis.m_InputAxisName = "";
            freelookCam.m_XAxis.m_InputAxisValue = 0;
            
            freelookCam.m_YAxis.m_InputAxisName = "";
            freelookCam.m_YAxis.m_InputAxisValue = 0;
        }
    }
}
