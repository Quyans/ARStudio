using System.Collections.Generic;

namespace TriLib
{
    /// <summary>
    /// Internally represents an intermediate animation data.
    /// </summary>
    public class AnimationChannelData
    {
        public string NodeName;
        public Dictionary<string, AnimationCurveData> CurveData;

        public void SetCurve(string propertyName, AnimationCurveData animationCurve)
        {
            CurveData.Add(propertyName, animationCurve);
        }

        public override string ToString()
        {
            return NodeName;
        }
    }
}