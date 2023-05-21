using UnityEngine;
using System;
using Object = UnityEngine.Object;

public static class TinyDebug
{
    private static void DoLog(Action<string, Object> LogFunction, string prefix, Object component, params object[] message)
    {
#if UNITY_EDITOR
        LogFunction($"{prefix}<b>[{component.name}]</b>: {String.Join(" | ", message)}", component);
#endif
    }

    public static void Log(this Object component, params object[] message)
    {
        DoLog(Debug.Log, "", component, message);
    }

    public static void LogWarning(this Object component, params object[] message)
    {
        DoLog(Debug.LogWarning, "<color=yellow>\u26A0</color> ", component, message);
    }

    public static void LogError(this Object component, params object[] message)
    {
        DoLog(Debug.LogError, "<color=red><b>!!! ERROR !!!</b></color> ", component, message);
    }

    public static void Test(this Object component, params object[] message)
    {
        DoLog(Debug.Log, "<color=lime><b>\u25AATESTING\u25AA</b></color> ", component, message);
    }

    public static void LogSave(this Object component, params object[] message)
    {
        DoLog(Debug.Log, "<color=green><b>TINY SAVE</b></color> ", component, message);
    }
}

