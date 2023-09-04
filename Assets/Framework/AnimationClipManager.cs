using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobsterFramework
{
    public class AnimationClipManager
    {
        private static Dictionary<string, AnimationClip> clips = new();

        public static void Add(RuntimeAnimatorController controller) {
            foreach (AnimationClip clip in controller.animationClips) {
                clips[clip.name] = clip;
            }
        }

        public static AnimationClip Get(string clipName)
        {
            if (!clips.ContainsKey(clipName)) {
                return null;
            }
            return clips[clipName];
        }
    }
}
