using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAim : NetworkBehaviour
{
    [SerializeField] private Transform tankTurret;
    [SerializeField] private InputReader inputReader;


    private void LateUpdate()
    {
        if(IsOwner)
        {
            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(inputReader.aimPosition);
            Vector2 _diff = _mousePos - (Vector2)tankTurret.position;
            tankTurret.up = _diff;
        }
    }
}
