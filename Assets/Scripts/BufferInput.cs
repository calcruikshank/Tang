using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TangData;

public struct BufferInput
{
    public InputActionType actionType;
    public Vector3 directionOfAction;
    public float timeOfInput;
    public BufferInput(InputActionType actionTypeSent, Vector3 directionSent, float timeOfInputSent)
    {
        actionType = actionTypeSent;
        directionOfAction = directionSent;
        timeOfInput = timeOfInputSent;
    }
}
