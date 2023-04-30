using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions {
    public static T RandomChoice<T>(T[] array) {
        return array[Random.Range(0, array.Length)];
    }
}
