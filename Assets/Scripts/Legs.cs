using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour
{
    [SerializeField] Transform leftLeg;
    [SerializeField] Transform rightLeg;

    [SerializeField] Vector3 originalLocalPositionLeft;
    [SerializeField] Vector3 originalLocalPositionRight;

    float lifeTime = 0;

    private void Start()
    {
    }


    public void HandleRunning(Vector3 directionAndMagnitude, float maxSpeed)
    {

        CalculateLeftAndRightLegWithSin(directionAndMagnitude, maxSpeed);
    }

    float distanceToMoveLeg = .3f;
    public void CalculateLeftAndRightLegWithSin(Vector3 directionAndMagnitude, float maxSpeed)
    {
        lifeTime += Time.deltaTime;
        float newZ = Mathf.Sin(lifeTime * directionAndMagnitude.magnitude * 2) * distanceToMoveLeg;
        if (directionAndMagnitude.magnitude > 1f)
        {

            Vector3 targetLeft = new Vector3(leftLeg.transform.localPosition.x, leftLeg.transform.localPosition.y, newZ);
            leftLeg.transform.localPosition = Vector3.MoveTowards(leftLeg.transform.localPosition, targetLeft, 5 * Time.deltaTime);
            Vector3 targetRight = new Vector3(rightLeg.transform.localPosition.x, rightLeg.transform.localPosition.y, -newZ);
            rightLeg.transform.localPosition = Vector3.MoveTowards(rightLeg.transform.localPosition, targetRight, 5 * Time.deltaTime);
        }
        else
        {
            lifeTime = 0;
        }
    }

    public void HandleStopping()
    {
        leftLeg.transform.localPosition = Vector3.MoveTowards(leftLeg.transform.localPosition, originalLocalPositionLeft, 10 * Time.deltaTime);
        rightLeg.transform.localPosition = Vector3.MoveTowards(rightLeg.transform.localPosition, originalLocalPositionRight, 10 * Time.deltaTime);
    }
}
