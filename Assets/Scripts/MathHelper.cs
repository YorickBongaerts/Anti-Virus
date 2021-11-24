using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class MathHelper
    {
        public MathHelper()
        {
        }
        #region MathHelpers
        public bool IsApproximatelyEqual(float a, float b, float treshold)
        {
            return (Mathf.Abs(a - b) < treshold);
        }

        /// <summary>
        /// Does not look at the y-component of the Vector3
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="treshold"></param>
        /// <returns></returns>
        public bool IsApproximatelyEqual(Vector3 a, Vector3 b, Vector3 treshold)
        {
            return (Mathf.Abs(a.x - b.x) < treshold.x && Mathf.Abs(a.z - b.z) < treshold.z);
        }
        #endregion MathHelpers

    }
}
