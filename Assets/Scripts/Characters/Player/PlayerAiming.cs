using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public float mouseSensitivity = 15f;
    public float aimDuration = 0.3f;

    public Transform cameraLookAt;
    private PlayerController _player;

    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;
    private Cinemachine.CinemachineInputProvider inputAxisProvider;

    private void Awake()
    {
        inputAxisProvider = GetComponent<Cinemachine.CinemachineInputProvider>();
        
        xAxis.SetInputAxisProvider(0, inputAxisProvider);
        yAxis.SetInputAxisProvider(1, inputAxisProvider);


        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);

        cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
        
    }
    // Start is called before the first frame update
    void Start()
    {
       _player = gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player._isTakingSurvey || _player._isCustomizing) return;

        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
        
        cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
    }
}
