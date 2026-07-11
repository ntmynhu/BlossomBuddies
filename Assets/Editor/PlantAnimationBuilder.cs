using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace BlossomBuddies.EditorTools
{
    /// <summary>
    /// Creates the shared plant feedback animations as editable assets:
    ///   - PlantFeedback_BounceStrong (springy squash-stretch while thriving)
    ///   - PlantFeedback_BounceLight  (gentle droop while declining)
    /// and a PlantFeedback controller that switches between them on the "IsGrowing" bool.
    /// Saved under Assets/Resources/Animations so Plant can load it at runtime. Tweak the
    /// clips afterwards in the Animation window.
    /// </summary>
    public static class PlantAnimationBuilder
    {
        private const string Folder = "Assets/Resources/Animations";
        private const string StrongPath = Folder + "/PlantFeedback_BounceStrong.anim";
        private const string LightPath = Folder + "/PlantFeedback_BounceLight.anim";
        private const string ControllerPath = Folder + "/PlantFeedback.controller";

        [MenuItem("Tools/Plants/Build Feedback Animations")]
        public static void Build()
        {
            EnsureFolder("Assets/Resources");
            EnsureFolder(Folder);

            var strong = CreateStrongBounce();
            var light = CreateLightBounce();

            AssetDatabase.DeleteAsset(StrongPath);
            AssetDatabase.DeleteAsset(LightPath);
            AssetDatabase.CreateAsset(strong, StrongPath);
            AssetDatabase.CreateAsset(light, LightPath);

            AssetDatabase.DeleteAsset(ControllerPath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            controller.AddParameter("IsGrowing", AnimatorControllerParameterType.Bool);

            var sm = controller.layers[0].stateMachine;

            var strongState = sm.AddState("BounceStrong");
            strongState.motion = strong;
            var lightState = sm.AddState("BounceLight");
            lightState.motion = light;
            sm.defaultState = lightState;

            var toStrong = sm.AddAnyStateTransition(strongState);
            toStrong.AddCondition(AnimatorConditionMode.If, 0f, "IsGrowing");
            toStrong.hasExitTime = false;
            toStrong.duration = 0.15f;
            toStrong.canTransitionToSelf = false;

            var toLight = sm.AddAnyStateTransition(lightState);
            toLight.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsGrowing");
            toLight.hasExitTime = false;
            toLight.duration = 0.15f;
            toLight.canTransitionToSelf = false;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[PlantAnimationBuilder] Built PlantFeedback animations + controller in " + Folder);
        }

        // Strong, springy squash-stretch, ~0.5s loop.
        private static AnimationClip CreateStrongBounce()
        {
            var clip = new AnimationClip { frameRate = 60 };

            SetScaleCurves(clip,
                xz: new[] { (0f, 1f), (0.12f, 1.12f), (0.3f, 0.95f), (0.5f, 1f) },
                y: new[] { (0f, 1f), (0.12f, 0.9f), (0.3f, 1.08f), (0.5f, 1f) });

            MakeLooping(clip);
            return clip;
        }

        // Gentle, slow droop, ~1.6s loop.
        private static AnimationClip CreateLightBounce()
        {
            var clip = new AnimationClip { frameRate = 60 };

            SetScaleCurves(clip,
                xz: new[] { (0f, 1f), (0.8f, 1.015f), (1.6f, 1f) },
                y: new[] { (0f, 1f), (0.8f, 0.96f), (1.6f, 1f) });

            MakeLooping(clip);
            return clip;
        }

        private static void SetScaleCurves(AnimationClip clip, (float t, float v)[] xz, (float t, float v)[] y)
        {
            var cx = Curve(xz);
            var cy = Curve(y);
            clip.SetCurve("", typeof(Transform), "m_LocalScale.x", cx);
            clip.SetCurve("", typeof(Transform), "m_LocalScale.z", cx);
            clip.SetCurve("", typeof(Transform), "m_LocalScale.y", cy);
        }

        private static AnimationCurve Curve((float t, float v)[] keys)
        {
            var curve = new AnimationCurve();
            foreach (var (t, v) in keys)
                curve.AddKey(new Keyframe(t, v));
            for (int i = 0; i < curve.length; i++)
                curve.SmoothTangents(i, 0f);
            return curve;
        }

        private static void MakeLooping(AnimationClip clip)
        {
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
        }

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                var name = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }
}
