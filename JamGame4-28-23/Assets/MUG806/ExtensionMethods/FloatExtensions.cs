using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions{
    public static float WrapAngle(this float degrees) {
        degrees = degrees % 360f;
        if (degrees >= 180f)
            return degrees - 360f;
        if (degrees <= -180f)
            return degrees + 360f;
        return degrees;
    }
}
