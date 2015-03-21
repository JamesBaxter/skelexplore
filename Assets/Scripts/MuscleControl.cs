using UnityEngine;
using System.Collections;

public class MuscleControl : MonoBehaviour {

    public Transform TopPoint;
    public Transform BottomPoint;
    private GameObject muscleObj;
    private float distance;

    public void Start()
    {
        this.muscleObj = (GameObject)this.transform.Find("MuscleObj").gameObject;
    }

	public void FixedUpdate () 
    {
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

        this.distance = Vector3.Distance(this.TopPoint.position, this.BottomPoint.position);

        // resize & re-position the child obj.
        this.muscleObj.transform.localScale = new Vector3(
            this.muscleObj.transform.localScale.x,
            this.muscleObj.transform.localScale.y,
            this.distance);

        this.muscleObj.transform.localPosition = new Vector3(0, 0, this.distance / 2);
    }
}
