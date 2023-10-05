using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CribbageCar : NetworkBehaviour
{
    //Temp list for now -- should be databased
    public Color[] colors;

    public CribbageLane lane;
    public int position;

    [SyncVar(hook = nameof(OnColorChanged))]
    public int colorId = 0;

    public override void OnStartClient()
    {
        OnColorChanged(0, colorId);
    }

    public void OnColorChanged(int _oldValue, int _newValue) => SetColor(colors[_newValue]);

    public void SetColor(Color color)
    {
        if (color == null)
            return;

        GetComponent<MeshRenderer>().material.color = color;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetPositionInLane(int _position)
    {
        position = _position;

        transform.position = lane.positions[Mathf.Clamp(position, 0, lane.positions.Length - 1)].position;
    }

    public void IncreasePosition(int amount)
    {
        position += amount;

        transform.position = lane.positions[Mathf.Clamp(position, 0, lane.positions.Length - 1)].position;
    }
}
