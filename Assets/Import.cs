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
    void Start()
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

            var firstObjectXAnimationCurve = new AnimationCurve();
            var firstObjectYAnimationCurve = new AnimationCurve();

            var secondObjectXAnimationCurve = new AnimationCurve();
            var secondObjectYAnimationCurve = new AnimationCurve();

            var thirdObjectXAnimationCurve = new AnimationCurve();
            var thirdObjectYAnimationCurve = new AnimationCurve();

            var fourthObjectXAnimationCurve = new AnimationCurve();
            var fourthObjectYAnimationCurve = new AnimationCurve();

            var ObjectAnimationCurve = new AnimationCurve();

            using (var reader = new StreamReader(Path.Join(Application.dataPath, "Animation/ObjectDataset/DevelopmentData.csv")))
            {
                reader.ReadLine();
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

                    var time = float.Parse(values[19], CultureInfo.InvariantCulture);

                    firstObjectXAnimationCurve.AddKey(time, firstDistanceX / 128.0f);
                    firstObjectYAnimationCurve.AddKey(time, firstDistanceY / 128.0f);

                    secondObjectXAnimationCurve.AddKey(time, secondDistanceX / 128.0f);
                    secondObjectYAnimationCurve.AddKey(time, secondDistanceY / 128.0f);

                    thirdObjectXAnimationCurve.AddKey(time, thirdDistanceX / 128.0f);
                    thirdObjectYAnimationCurve.AddKey(time, thirdDistanceY / 128.0f);

                    fourthObjectXAnimationCurve.AddKey(time, fourthDistanceX / 128.0f);
                    fourthObjectYAnimationCurve.AddKey(time, fourthDistanceY / 128.0f);
                }
            }

            firstObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", firstObjectXAnimationCurve);
            firstObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", firstObjectYAnimationCurve);

            secondObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", secondObjectXAnimationCurve);
            secondObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", secondObjectYAnimationCurve);

            thirdObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", thirdObjectXAnimationCurve);
            thirdObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", thirdObjectYAnimationCurve);

            fourthObjectMovement.SetCurve("", typeof(Transform), "localPosition.x", fourthObjectXAnimationCurve);
            fourthObjectMovement.SetCurve("", typeof(Transform), "localPosition.z", fourthObjectYAnimationCurve);

            Directory.CreateDirectory(animationDirectory);

            AssetDatabase.CreateAsset(firstObjectMovement, "Assets/Animation/Generated/Anim1.anim");
            AssetDatabase.CreateAsset(secondObjectMovement, "Assets/Animation/Generated/Anim2.anim");
            AssetDatabase.CreateAsset(thirdObjectMovement, "Assets/Animation/Generated/Anim3.anim");
            AssetDatabase.CreateAsset(fourthObjectMovement, "Assets/Animation/Generated/Anim4.anim");
        }
    }
}
