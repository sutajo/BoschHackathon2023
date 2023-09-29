using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;
using System.Globalization;

public class Import : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var _animation = GetComponent<Animation>();

        var animationDirectory = Path.Join(Application.dataPath, "Animation/Generated");

        if (!Directory.Exists(animationDirectory))
        {
            var firstObjectMovement = new AnimationClip();
            firstObjectMovement.legacy = true;

            var secondObjectMovement = new AnimationClip();
            secondObjectMovement.legacy = true;

            var thirdObjectMovement = new AnimationClip();
            thirdObjectMovement.legacy = true;

            var fourthObjectMovement = new AnimationClip();
            fourthObjectMovement.legacy = true;

            var vehicleMovement = new AnimationClip();
            vehicleMovement.legacy = true;

            var firstObjectXAnimationKeyfames = new List<Keyframe>();
            var firstObjectYAnimationKeyfames = new List<Keyframe>();

            var secondObjectXAnimationKeyfames = new List<Keyframe>();
            var secondObjectYAnimationKeyfames = new List<Keyframe>();

            var thirdObjectXAnimationKeyfames = new List<Keyframe>();
            var thirdObjectYAnimationKeyfames = new List<Keyframe>();

            var fourthObjectXAnimationKeyfames = new List<Keyframe>();
            var fourthObjectYAnimationKeyfames = new List<Keyframe>();

            var vehicleObjectXAnimationKeyfames = new List<Keyframe>();
            var vehicleObjectYAnimationKeyfames = new List<Keyframe>();
            var vehicleObjectZAnimationKeyfames = new List<Keyframe>();
            var vehicleObjectYawAnimationKeyframes = new List<Keyframe>();

            var vehicleWheelRotation = new List<Keyframe>();
            var vehicleWheelTurn = new List<Keyframe>();

            Vector3 vehiclePos = Vector3.zero;
            vehiclePos.y = 0.74f;
            float vehicleRotation = 0.0f;
            float wheelRotation = 0.0f;

            float prevTime = 0.0f;

            using (var reader = new StreamReader(Path.Join(Application.dataPath, "Animation/ObjectDataset/DevelopmentData.csv")))
            {
                reader.ReadLine();
                int lineIndex = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var firstDistanceX = float.Parse(values[1], CultureInfo.InvariantCulture);
                    var firstDistanceY = float.Parse(values[2], CultureInfo.InvariantCulture);

                    var secondDistanceX = float.Parse(values[3], CultureInfo.InvariantCulture);
                    var secondDistanceY = float.Parse(values[4], CultureInfo.InvariantCulture);

                    var thirdDistanceX = float.Parse(values[5], CultureInfo.InvariantCulture);
                    var thirdDistanceY = float.Parse(values[6], CultureInfo.InvariantCulture);

                    var fourthDistanceX = float.Parse(values[7], CultureInfo.InvariantCulture);
                    var fourthDistanceY = float.Parse(values[8], CultureInfo.InvariantCulture);

                    var vehicleSpeed = float.Parse(values[9], CultureInfo.InvariantCulture);

                    var firstVelocityX = float.Parse(values[10], CultureInfo.InvariantCulture);
                    var firstVelocityY = float.Parse(values[11], CultureInfo.InvariantCulture);

                    var secondVelocityX = float.Parse(values[12], CultureInfo.InvariantCulture);
                    var secondVelocityY = float.Parse(values[13], CultureInfo.InvariantCulture);

                    var thirdVelocityX = float.Parse(values[14], CultureInfo.InvariantCulture);
                    var thirdVelocityY = float.Parse(values[15], CultureInfo.InvariantCulture);

                    var fourthVelocityX = float.Parse(values[16], CultureInfo.InvariantCulture);
                    var fourthVelocityY = float.Parse(values[17], CultureInfo.InvariantCulture);

                    var yawRate = float.Parse(values[18], CultureInfo.InvariantCulture);

                    var time = float.Parse(values[19], CultureInfo.InvariantCulture) - 30;

                    Keyframe key = new Keyframe();
                    key.time = time;

                    if (lineIndex != 0)
                    {
                        var dt = time - prevTime;
                        vehicleRotation -= yawRate * dt;

                        Vector3 forward = new Vector3(Mathf.Cos(-vehicleRotation), 0.0f, Mathf.Sin(-vehicleRotation));
                        vehiclePos += forward * vehicleSpeed / 256.0f * dt;


                        key.value = -Mathf.Rad2Deg * yawRate;
                        vehicleWheelTurn.Add(key);

                        wheelRotation += vehicleSpeed / 256.0f * 1.2f * dt;

                        key.value = Mathf.Rad2Deg * wheelRotation;
                        vehicleWheelRotation.Add(key);
                    }


                    key.value = firstDistanceX / 128.0f;
                    //key.outTangent = firstVelocityX;
                    firstObjectXAnimationKeyfames.Add(key);

                    key.value = firstDistanceY / 128.0f;
                    //key.outTangent = firstVelocityY;
                    firstObjectYAnimationKeyfames.Add(key);

                    key.value = secondDistanceX / 128.0f;
                    //key.outTangent = secondVelocityX;
                    secondObjectXAnimationKeyfames.Add(key);

                    key.value = secondDistanceY / 128.0f;
                    //key.outTangent = secondVelocityY;
                    secondObjectYAnimationKeyfames.Add(key);

                    key.value = thirdDistanceX / 128.0f;
                    //key.outTangent = thirdVelocityX;
                    thirdObjectXAnimationKeyfames.Add(key);

                    key.value = thirdDistanceY / 128.0f;
                    //key.outTangent = thirdVelocityY;
                    thirdObjectYAnimationKeyfames.Add(key);

                    key.value = fourthDistanceX / 128.0f;
                    //key.outTangent = fourthVelocityX;
                    fourthObjectXAnimationKeyfames.Add(key);

                    key.value = fourthDistanceY / 128.0f;
                    //key.outTangent = fourthVelocityY;
                    fourthObjectYAnimationKeyfames.Add(key);

                    key.value = vehiclePos.x;
                    vehicleObjectXAnimationKeyfames.Add(key);

                    key.value = vehiclePos.z;
                    vehicleObjectYAnimationKeyfames.Add(key);

                    key.value = vehiclePos.y;
                    vehicleObjectZAnimationKeyfames.Add(key);

                    key.value = Mathf.Rad2Deg * vehicleRotation;
                    vehicleObjectYawAnimationKeyframes.Add(key);

                    prevTime = time;
                    ++lineIndex;
                }
            }

            var firstObjectXAnimationCurve = new AnimationCurve(firstObjectXAnimationKeyfames.ToArray());
            var firstObjectYAnimationCurve = new AnimationCurve(firstObjectYAnimationKeyfames.ToArray());

            var secondObjectXAnimationCurve = new AnimationCurve(secondObjectXAnimationKeyfames.ToArray());
            var secondObjectYAnimationCurve = new AnimationCurve(secondObjectYAnimationKeyfames.ToArray());

            var thirdObjectXAnimationCurve = new AnimationCurve(thirdObjectXAnimationKeyfames.ToArray());
            var thirdObjectYAnimationCurve = new AnimationCurve(thirdObjectYAnimationKeyfames.ToArray());

            var fourthObjectXAnimationCurve = new AnimationCurve(fourthObjectXAnimationKeyfames.ToArray());
            var fourthObjectYAnimationCurve = new AnimationCurve(fourthObjectYAnimationKeyfames.ToArray());

            firstObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", firstObjectXAnimationCurve);
            firstObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", firstObjectYAnimationCurve);

            secondObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", secondObjectXAnimationCurve);
            secondObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", secondObjectYAnimationCurve);

            thirdObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", thirdObjectXAnimationCurve);
            thirdObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", thirdObjectYAnimationCurve);

            fourthObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", fourthObjectXAnimationCurve);
            fourthObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", fourthObjectYAnimationCurve);

            var vehicleXCurve = new AnimationCurve(vehicleObjectXAnimationKeyfames.ToArray());
            var vehicleYCurve = new AnimationCurve(vehicleObjectYAnimationKeyfames.ToArray());
            var vehicleZCurve = new AnimationCurve(vehicleObjectZAnimationKeyfames.ToArray());
            var vehicleRotationCurve = new AnimationCurve(vehicleObjectYawAnimationKeyframes.ToArray());
            var vehilceAnimation = new AnimationClip();
            vehilceAnimation.legacy = true;

            vehilceAnimation.SetCurve("", typeof(Transform), "localPosition.x", vehicleXCurve);
            vehilceAnimation.SetCurve("", typeof(Transform), "localPosition.y", vehicleZCurve);
            vehilceAnimation.SetCurve("", typeof(Transform), "localPosition.z", vehicleYCurve);
            vehilceAnimation.SetCurve("", typeof(Transform), "localEulerAngles.y", vehicleRotationCurve);

            var rotations = vehicleWheelRotation.ToArray();
            var wheelRotationCurve = new AnimationCurve(rotations);
            var wheelTurnCurve = new AnimationCurve(vehicleWheelTurn.ToArray());

            vehilceAnimation.SetCurve("BMW i8/wheelFrontLeft", typeof(Transform), "localEulerAngles.y", wheelTurnCurve);
            vehilceAnimation.SetCurve("BMW i8/wheelFrontRight", typeof(Transform), "localEulerAngles.y", wheelTurnCurve);

            vehilceAnimation.SetCurve("BMW i8/wheelFrontLeft", typeof(Transform), "localEulerAngles.x", wheelRotationCurve);
            vehilceAnimation.SetCurve("BMW i8/wheelFrontRight", typeof(Transform), "localEulerAngles.x", wheelRotationCurve);
            vehilceAnimation.SetCurve("BMW i8/wheelsBack", typeof(Transform), "localEulerAngles.x", wheelRotationCurve);

            Directory.CreateDirectory(animationDirectory);

            AssetDatabase.CreateAsset(firstObjectMovement, "Assets/Animation/Generated/Anim1.anim");
            AssetDatabase.CreateAsset(secondObjectMovement, "Assets/Animation/Generated/Anim2.anim");
            AssetDatabase.CreateAsset(thirdObjectMovement, "Assets/Animation/Generated/Anim3.anim");
            AssetDatabase.CreateAsset(fourthObjectMovement, "Assets/Animation/Generated/Anim4.anim");
            AssetDatabase.CreateAsset(vehilceAnimation, "Assets/Animation/Generated/Vehicle.anim");
        }
    }
}
