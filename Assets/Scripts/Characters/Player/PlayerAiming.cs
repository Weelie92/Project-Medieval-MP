using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    public float mouseSensitivity = 15f;
    public float aimDuration = 0.3f;


    [SerializeField] private GameObject _cameraHolder;
    public Transform cameraLookAt;
    private PlayerController _player;

    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;
    public Cinemachine.AxisState xAxisOld;
    public Cinemachine.AxisState yAxisOld;
    private Cinemachine.CinemachineInputProvider inputAxisProvider;

    private void Start()
    {
        inputAxisProvider = GetComponent<Cinemachine.CinemachineInputProvider>();

        xAxis.SetInputAxisProvider(0, inputAxisProvider);
        yAxis.SetInputAxisProvider(1, inputAxisProvider);

        _player = gameObject.GetComponent<PlayerController>();


    }


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (_player.isTakingSurvey) return;

        if (!_player.isInventoryOpen && !_player.isCustomizing)
        {
            xAxis.Update(Time.deltaTime);
            yAxis.Update(Time.deltaTime);

            cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
        }
        else if (_player.isInventoryOpen)
        {
            if (xAxis.Value != xAxisOld.Value)
            {
                xAxisOld = xAxis;
                yAxisOld = yAxis;

            }

            cameraLookAt.eulerAngles = new Vector3(yAxisOld.Value, xAxisOld.Value, 0);
        }
        else if (_player.isCustomizing && _player.isMainhanding)
        {
            xAxis.Update(Time.deltaTime);
            yAxis.Update(Time.deltaTime);

            cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
        }
    }
}
