using System.Collections.Generic;
using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Internally represents an Unity <see cref="UnityEngine.AnimationClip"/>.
    /// </summary>
    public class AnimationData
    {
        public string Name;
        public bool Legacy;
        public float Length;
        public float FrameRate;
        public WrapMode WrapMode;
        public AnimationChannelData[] ChannelData;
        public List<float> KeyTimes;

        public AnimationClip AnimationClip;
    }
}