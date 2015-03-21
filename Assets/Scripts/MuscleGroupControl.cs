using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MuscleGroupControl : MonoBehaviour {

    public Transform TopPoint;
    public Transform BottomPoint;
    public float EstimatedMax = 5;
    public float EstimatedMin = 1;
    public float MaxSize = 2;
    public float MinSize = 0.1f;
    
    private float distance;

    private List<Transform> muscleBendPoints;
    private List<MuscleBendSection> muscleObjs;

    public void Start()
    {
        this.muscleBendPoints = this.gameObject.GetComponentsInChildren<Transform>()
                                               .Where(x => x.tag == "muscleBend")
                                               .OrderBy(x => Vector3.Distance(x.position, this.TopPoint.position))
                                               .ToList();

        this.BuildMuscleObjects();
    }

	public void FixedUpdate () 
    {
        this.distance = Vector3.Distance(this.TopPoint.position, this.BottomPoint.position);
        foreach (var section in this.muscleObjs)
        {
            section.FixedUpdate();
        }
	}

    /// <summary>
    /// Init muscle prfabs between each muscle bend point.
    /// </summary>
    private void BuildMuscleObjects()
    {
        this.muscleObjs = new List<MuscleBendSection>();

        for (var i = 0; i < this.muscleBendPoints.Count() + 1; i++)
        {
            var muscle = (GameObject)Instantiate(Resources.Load("muscleBendSection"));
            muscle.transform.parent = this.transform;

            this.muscleObjs.Add(new MuscleBendSection
            {
                Muscle = muscle,
                TopPoint = (i == 0) ? this.TopPoint : this.muscleBendPoints[i - 1],
                BottomPoint = (i == this.muscleBendPoints.Count()) ? this.BottomPoint : this.muscleBendPoints[i]
            });
        }

        foreach (var section in this.muscleObjs)
        {
            section.Start();
        }
    }
}


public class MuscleBendSection
{
    public GameObject Muscle { get; set; }
    public Transform TopPoint { get; set; }
    public Transform BottomPoint { get; set; }

    private float distance = 0f;
    private GameObject muscleObj;

    public void Start()
    {
        this.muscleObj = this.Muscle.transform.Find("MuscleObj").gameObject;
        this.muscleObj.GetComponent<MeshRenderer>().enabled = true;
    }

    public void FixedUpdate()
    {
        this.distance = Vector3.Distance(this.TopPoint.position, this.BottomPoint.position);
        this.UpdateMusclePosition();
    }

    private void UpdateMusclePosition()
    {
        // Move the parent object.
        this.Muscle.transform.position = this.TopPoint.position;
        this.Muscle.transform.LookAt(this.BottomPoint);

        //// resize & re-position the child obj.
        //var muscleScale = this.GetMuscleSize();
        this.muscleObj.transform.localScale = new Vector3(.05f, .05f, this.distance);
        this.muscleObj.transform.localPosition = new Vector3(0f, 0f, this.distance / 2);
    }
}