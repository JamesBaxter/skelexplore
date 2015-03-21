using UnityEngine;
using System.Collections;

public class MuscleControl : MonoBehaviour {

    public Transform TopPoint;
    public Transform BottomPoint;
    public float EstimatedMax = 5;
    public float EstimatedMin = 1;
    public float MaxSize = 2;
    public float MinSize = 0.1f;
    
    private GameObject muscleObj;
    private float distance;

    public void Start()
    {
        this.muscleObj = (GameObject)this.transform.Find("MuscleObj").gameObject;
        this.muscleObj.GetComponent<MeshRenderer>().enabled = true;
    }

	public void FixedUpdate () 
    {
        this.distance = Vector3.Distance(this.TopPoint.position, this.BottomPoint.position);
        this.UpdateMusclePosition();
	}

    /// <summary>
    /// Centers the muscle between the two points and resizes it
    /// to reach the gap.
    /// </summary>
    private void UpdateMusclePosition()
    {
        // Move the parent object.
        this.transform.position = this.TopPoint.position;
        this.transform.LookAt(this.BottomPoint);

        // resize & re-position the child obj.
        var muscleScale = this.GetMuscleSize();
        this.muscleObj.transform.localScale = new Vector3(muscleScale, muscleScale, this.distance);
        this.muscleObj.transform.localPosition = new Vector3(0f, 0f, this.distance / 2);
    }

    /// <summary>
    /// This function will scale the size related to the distance
    /// to mimic how a muscle works.
    /// </summary>
    private float GetMuscleSize()
    {
        if (this.distance >= this.EstimatedMax)
        {
            return this.MinSize;
        }
        else if (this.distance <= this.EstimatedMin)
        {
            return this.MaxSize;
        }

        var distancePercentage = ((this.distance / (this.EstimatedMax - this.EstimatedMin)) - 1) * -1;
        var growthPercentage = ((this.MaxSize - this.MinSize) * distancePercentage) + this.MinSize;

        if (growthPercentage < this.MinSize)
        {
            return this.MinSize;
        }
        if (growthPercentage > this.MaxSize)
        {
            return this.MaxSize;
        }

        return growthPercentage;
    }
}
