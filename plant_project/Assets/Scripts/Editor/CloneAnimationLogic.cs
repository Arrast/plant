using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PlantContentFolderInformation
{
    public string PlantName;
    public bool Exists;
    public bool Override = false;

    public override string ToString()
    {
        return $"{PlantName} - Exists? {Exists} - Override: {Override}";
    }
}


public class AnimationFolderInformation : PlantContentFolderInformation
{
    public List<string> PlantStages = new List<string>();
 
    public override string ToString()
    {
        string message = base.ToString() + "Stages: ";
        foreach (var stage in PlantStages)
        {
            message += $"{stage} ";
        }
        return message;
    }
}

public class CloneAnimationLogic
{
    private const string PlantAnimationsPath = "Assets/Addressables/Animations/";
    private const string AnimationPathFormat = PlantAnimationsPath + "{0}/{0}_{1}_idle.anim";
    private const string AnimatorOverrideControllerPathFormat = PlantAnimationsPath + "{0}/{0}.overrideController";


    public static List<AnimationFolderInformation> GetListOfFolders()
    {
        List<AnimationFolderInformation> folderInformationList = new List<AnimationFolderInformation>();

        var folders = Directory.GetDirectories(Const.PlantAssetPath);
        foreach (var folder in folders)
        {
            var plantName = Path.GetRelativePath(Const.PlantAssetPath, folder);

            AnimationFolderInformation folderInformation = new AnimationFolderInformation();
            folderInformation.Exists = Directory.Exists($"{PlantAnimationsPath}{plantName}");
            folderInformation.PlantName = plantName;

            var subfolders = Directory.GetDirectories(folder);
            foreach (var stageFolder in subfolders)
            {
                folderInformation.PlantStages.Add(Path.GetRelativePath(folder, stageFolder));
            }

            folderInformationList.Add(folderInformation);
        }

        return folderInformationList;
    }

    public static void CreateAnimations(AnimationFolderInformation animationFolderInformation)
    {
        Dictionary<string, AnimationClip> animations = new Dictionary<string, AnimationClip>();
        string targetPlant = animationFolderInformation.PlantName;
        foreach (var plantState in animationFolderInformation.PlantStages)
        {
            var resultClip = CloneAnimation(targetPlant, plantState, animationFolderInformation.Override); 
            if (resultClip != null)
            {
                animations.Add(resultClip.name, resultClip);
            }
        }

        var destinationPath = string.Format(AnimatorOverrideControllerPathFormat, targetPlant);
        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(destinationPath)) || animationFolderInformation.Override)
        {
            CreateAnimationOverrideController(targetPlant, animations);
        }
    }

    private static AnimationClip CloneAnimation(string targetPlant, string plantState, bool overrideAnimation)
    {
        string path = string.Format(AnimationPathFormat, Const.DefaultPlantAsset, plantState);
        var animationAsset = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (animationAsset == null)
        {
            Debug.LogError($"Can't find the base animation at {path}");
            return null;
        }

        // We try to open the target.
        string resultClipsPath = ($"{PlantAnimationsPath}{targetPlant}");
        if (!Directory.Exists(resultClipsPath))
        {
            Directory.CreateDirectory(resultClipsPath);
        }

        string resultClipPath = string.Format(AnimationPathFormat, targetPlant, plantState);
        var resultClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(resultClipPath);
        if (resultClip == null)
        {
            resultClip = new AnimationClip();
            resultClip.frameRate = animationAsset.frameRate;
            resultClip.legacy = animationAsset.legacy;

            AssetDatabase.CreateAsset(resultClip, resultClipPath);
        }
        else if (!overrideAnimation) 
        { return resultClip; }

        Dictionary<float, string> keyframeData = new Dictionary<float, string>();

        var curvesBindings = AnimationUtility.GetObjectReferenceCurveBindings(animationAsset);
        foreach (EditorCurveBinding binding in curvesBindings)
        {
            if (binding.type == typeof(SpriteRenderer))
            {
                BuildKeyframesForBinding(targetPlant, plantState, binding, animationAsset, resultClip);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // We return the clip
        return resultClip;
    }

    private static void BuildKeyframesForBinding(string targetPlant, string plantState, EditorCurveBinding binding, AnimationClip animationAsset, AnimationClip resultClip)
    {
        ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(animationAsset, binding);
        List<ObjectReferenceKeyframe> keyFrameList = new List<ObjectReferenceKeyframe>();
        foreach (var keyframe in keyframes)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Const.PlantAssetPath + $"{targetPlant}/{plantState}/{keyframe.value.name}.png");
            if (sprite == null)
            {
                Debug.LogError($"We can't find the sprite for {keyframe.value}");
                continue;
            }

            keyFrameList.Add(new ObjectReferenceKeyframe()
            {
                time = keyframe.time,
                value = sprite
            });

        }

        // Build the clip if valid
        if (keyFrameList.Count > 0)
        {
            // Set the keyframes to the animation
            AnimationUtility.SetObjectReferenceCurve(resultClip, binding, keyFrameList.ToArray());
        }
    }

    private static void CreateAnimationOverrideController(string targetPlant, Dictionary<string, AnimationClip> animations)
    {
        var path = string.Format(AnimatorOverrideControllerPathFormat, Const.DefaultPlantAsset);
        var animatorOverrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);
        if (animatorOverrideController == null)
        { return; }

        var destinationPath = string.Format(AnimatorOverrideControllerPathFormat, targetPlant);
        var destinationOverrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(destinationPath);
        if (destinationOverrideController == null)
        {
            destinationOverrideController = new AnimatorOverrideController(animatorOverrideController.runtimeAnimatorController);
            AssetDatabase.CreateAsset(destinationOverrideController, destinationPath);
        }

        foreach (var animation in animatorOverrideController.animationClips)
        {
            string animationName = animation.name.Replace(Const.DefaultPlantAsset, targetPlant);
            if (animations.TryGetValue(animationName, out var animationClip))
            {
                destinationOverrideController[animation.name] = animationClip;
            }
        }

        var destinationFolder = Path.GetDirectoryName(destinationPath);
        if (!Directory.Exists(destinationFolder))
        {
            Directory.CreateDirectory(destinationFolder);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
}
