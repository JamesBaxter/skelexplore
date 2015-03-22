using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System.Linq;

public class BipedControl : MonoBehaviour {
    
    public Transform[] Joints = new Transform[25];

    private Transform[] jointChild;
    private KinectSensor sensor;
    private BodyFrameReader reader;
    private Body[] data = null;
    private ulong trackedBodyId = 0;
    private int idx = -1;
    private List<PreviousState> prevStates = new List<PreviousState>();

    void Start()
    {
        this.sensor = KinectSensor.GetDefault();
        if (this.sensor != null)
        {
            this.reader = this.sensor.BodyFrameSource.OpenReader();

            if (!this.sensor.IsOpen)
            {
                this.sensor.Open();
            }
        }

        this.SetChildJoints();
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.reader != null)
        {
            var frame = this.reader.AcquireLatestFrame();

            if (frame != null)
            {
                if (this.data == null)
                {
                    this.data = new Body[this.sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(this.data);

                frame.Dispose();
                frame = null;

                if (this.trackedBodyId == 0)
                {

                    for (int i = 0; i < this.sensor.BodyFrameSource.BodyCount; i++)
                    {
                        if (this.data[i].IsTracked)
                        {
                            idx = i;
                            this.trackedBodyId = this.data[i].TrackingId;
                        }
                    }
                }

                if (this.idx > -1)
                {
                    if (this.data[this.idx].TrackingId == this.trackedBodyId)
                    {
                        if (this.data[this.idx].TrackingId == this.trackedBodyId)
                        {
                            foreach (var joint in this.Joints)
                            {
                                this.UpdateObjectPosition(joint);
                                this.UpdateObjectRotation(joint);
                            }
                        }
                    }
                }
            }
        }

        this.UpdateChildBones();
    }

    private void UpdateChildBones()
    {
        for (var i = 0; i < this.jointChild.Length; i++)
        {
            var lookAtJointName = this.GetLookAtJointName(this.jointChild[i].parent.name);
            var lookAtJoint = this.prevStates.FirstOrDefault(x => x.Name == lookAtJointName);
            if (lookAtJoint == null)
            {
                return;
            }
            //var distance = Vector3.Distance(this.jointChild[i].parent.transform.position, lookAtJoint.Pos);
            this.jointChild[i].LookAt(lookAtJoint.Pos);
        }
    }

    private void UpdateObjectRotation(Transform gObject)
    {
        Windows.Kinect.JointOrientation joint = new Windows.Kinect.JointOrientation();
        this.data[this.idx].JointOrientations.TryGetValue(this.GetJointType(gObject.name), out joint);

        var rotx = joint.Orientation.X;
        var roty = joint.Orientation.Y;
        var rotz = joint.Orientation.Z;

        gObject.rotation = Quaternion.Euler(rotx * 100, roty * 100, rotz * 100);

        if (!this.prevStates.Any(x => x.Name == gObject.name))
        {
            this.prevStates.Add(new PreviousState
            {
                Name = gObject.name,
                Rot = gObject.rotation
            });
        }
        else
        {
            this.prevStates.First(x => x.Name == gObject.name).Rot = gObject.rotation;
        }
    }

    private void UpdateObjectPosition(Transform gObject)
    {
        Windows.Kinect.Joint joint = new Windows.Kinect.Joint();
        this.data[this.idx].Joints.TryGetValue(this.GetJointType(gObject.name), out joint);

        var posx = joint.Position.X;
        var posy = joint.Position.Y;
        var posz = joint.Position.Z;
        gObject.position = new Vector3(posx, posy, posz);

        if (!this.prevStates.Any(x => x.Name == gObject.name))
        {
            this.prevStates.Add(new PreviousState
            {
                Name = gObject.name,
                Pos = gObject.position
            });
        }
        else
        {
            this.prevStates.First(x => x.Name == gObject.name).Pos = gObject.position;
        }
    }

    private JointType GetJointType(string type)
    {
        switch (type)
        {
            case "HandRight":
                return JointType.HandRight;
            case "ElbowRight":
                return JointType.ElbowRight;
            case "ShoulderRight":
                return JointType.ShoulderRight;
            case "Head":
                return JointType.Head;
            case "HipLeft":
                return JointType.HipLeft;
            case "HipRight":
                return JointType.HipRight;
            case "SpineBase":
                return JointType.SpineBase;
            case "SpineMid":
                return JointType.SpineMid;
            case "SpineShoulder":
                return JointType.SpineShoulder;
            case "AnkleRight":
                return JointType.AnkleRight;
            case "ElbowLeft":
                return JointType.ElbowLeft;
            case "FootLeft":
                return JointType.FootLeft;
            case "FootRight":
                return JointType.FootRight;
            case "HandLeft":
                return JointType.HandLeft;
            case "HandTipLeft":
                return JointType.HandTipLeft;
            case "HandTipRight":
                return JointType.HandTipRight;
            case "KneeLeft":
                return JointType.KneeLeft;
            case "KneeRight":
                return JointType.KneeRight;
            case "Neck":
                return JointType.Neck;
            case "ShoulderLeft":
                return JointType.ShoulderLeft;
            case "ThumbLeft":
                return JointType.ThumbLeft;
            case "ThumbRight":
                return JointType.ThumbRight;
            case "WristLeft":
                return JointType.WristLeft;
            case "WristRight":
                return JointType.WristRight;
        }
        return JointType.Head;
    }

    private void SetChildJoints()
    {
        this.jointChild = new Transform[this.Joints.Length];
        for (var i = 0; i < this.Joints.Length; i++)
        {
            this.jointChild[i] = this.Joints[i].Find("bone").transform;
        }
    }

    void OnApplicationQuit()
    {
        if (this.reader != null)
        {
            this.reader.Dispose();
            this.reader = null;
        }

        if (this.sensor != null)
        {
            if (this.sensor.IsOpen)
            {
                this.sensor.Close();
            }
            this.sensor = null;
        }
    }

    private string GetLookAtJointName(string type)
    {

        var name = "";
        switch (type)
        {
            case "SpineBase":
                return "SpineMid";
            case "SpineMid":
                return "SpineShoulder";
            case "SpineShoulder":
                return "ShoulderRight";
            case "HipLeft":
                return "SpineBase";
            case "HipRight":
                return "SpineBase";
            case "ElbowRight":
                return "WristRight";
            case "ShoulderRight":
                return "ElbowRight";
            case "WristRight":
                return "WristRight";
            //case "AnkleRight":
            //    return JointType.AnkleRight;
            //case "ElbowLeft":
            //    return JointType.ElbowLeft;
            //case "FootLeft":
            //    return JointType.FootLeft;
            //case "FootRight":
            //    return JointType.FootRight;
            //case "HandLeft":
            //    return JointType.HandLeft;
            //case "HandTipLeft":
            //    return JointType.HandTipLeft;
            //case "HandTipRight":
            //    return JointType.HandTipRight;
            //case "KneeLeft":
            //    return JointType.KneeLeft;
            //case "KneeRight":
            //    return JointType.KneeRight;
            //case "Neck":
            //    return JointType.Neck;
            //case "ShoulderLeft":
            //    return JointType.ShoulderLeft;
            //case "ThumbLeft":
            //    return JointType.ThumbLeft;
            //case "ThumbRight":
            //    return JointType.ThumbRight;
            //case "WristLeft":
            //    return JointType.WristLeft;
        }

        return name;
    }
}


public class PreviousState
{
    public string Name { get; set; }
    public Vector3 Pos { get; set; }
    public Quaternion Rot { get; set; }
}