using UnityEngine;

namespace TriLib
{
    /// <summary>
    /// Internally represents an Unity <see cref="UnityEngine.AnimationCurve"/>.
    /// </summary>
    public class AnimationCurveData
    {
        public readonly Keyframe[] Keyframes;

        private uint _index;

        public AnimationCurveData(uint numKeys)
        {
            Keyframes = new Keyframe[numKeys];
        }

        public void AddKey(float time, float value)
        {
            Keyframes[_index++] = new Keyframe(time, value);    
        }

        public AnimationCurve AnimationCurve;
    }
}