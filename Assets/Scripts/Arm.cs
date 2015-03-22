using System;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Windows.Kinect;

public class Arm : MonoBehaviour
{
    public Transform[] ArmObject = new Transform[8];

    private KinectSensor sensor;
    private BodyFrameReader reader;
    private Body[] data = null;
    private ulong trackedBodyId = 0;
    private int idx = -1;

    // Use this for initialization
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

                if (idx > -1)
                {
                    if (this.data[idx].TrackingId == trackedBodyId)
                    {
                        foreach (var armTransform in this.ArmObject)
                        {
                            this.UpdateObjectPosition(armTransform, this.data[this.idx].Joints);
                            this.UpdateObjectRotation(armTransform, this.data[this.idx].JointOrientations);
                        }
                    }
                }
            }
        }

    }

    private void UpdateObjectRotation(Transform gObject, Dictionary<JointType, Windows.Kinect.JointOrientation> joints)
    {
        JointOrientation jointOrientation = new Windows.Kinect.JointOrientation();
        joints.TryGetValue(this.GetJointType(gObject.name), out jointOrientation);

        var rotx = jointOrientation.Orientation.X;
        var roty = jointOrientation.Orientation.Y;
        var rotz = jointOrientation.Orientation.Z;

   
        gObject.rotation = Quaternion.Euler(rotx *100, roty *100, rotz*100);
                        
    }

    private void UpdateObjectPosition(Transform gObject, Dictionary<JointType, Windows.Kinect.Joint> joints)
    {
        Windows.Kinect.Joint joint = new Windows.Kinect.Joint();
        joints.TryGetValue(this.GetJointType(gObject.name), out joint);
        
        //var posTest = (joints[JointType].Position.X);
        var posx = joint.Position.X;
        var posy = joint.Position.Y;
        var posz = joint.Position.Z;
        var rotx = joint.Position.X;
        var roty = joint.Position.Y;
        var rotz = joint.Position.Z;
        
        gObject.position = new Vector3(posx, posy, posz);
    }

    private JointType GetJointType(String type)
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


        }
        //Unreachable?
        return JointType.Head;
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
}
