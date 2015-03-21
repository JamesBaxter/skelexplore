using System;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Windows.Kinect;

public class Arm : MonoBehaviour
{
    public Transform[] ArmObject = new Transform[3];

    private KinectSensor sensor;
    private BodyFrameReader reader;
    private Body[] data = null;

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

                int idx = -1;
                for (int i = 0; i < this.sensor.BodyFrameSource.BodyCount; i++)
                {
                    if (this.data[i].IsTracked)
                    {
                        idx = i;
                    }
                }
                foreach (var armTransform in this.ArmObject)
                {
                    this.UpdateObjectPosition(armTransform, this.data[idx].Joints);
                }
            }
        }

    }

    private void UpdateObjectPosition(Transform gObject, Dictionary<JointType, Windows.Kinect.Joint> joints)
    {
        Windows.Kinect.Joint joint = new Windows.Kinect.Joint();
        joints.TryGetValue(this.GetJointType(gObject.name), out joint);
        
        //var posTest = (joints[JointType].Position.X);
        var posx = joint.Position.X;
        var posy = joint.Position.Y;
        var posz = joint.Position.Z;

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
