using System.IO;

using UnityEngine;
using UnityEditor;
using System;

/**
 * 
 */
public class UnityBuildTasks {

    /**
     * 
     */
#if UNITY_EDITOR
    [MenuItem("Build/Windows 64 standalone player")]
    public static void BuildWin64StandalonePlayer() {
        Build(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Player);
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    [MenuItem("Build/Windows 64 standalone server")]
    public static void BuildWin64StandaloneServer() {
        Build(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Server);
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    protected static void Build<T>(BuildTarget buildTarget, T buildSubtarget) {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.subtarget = (int)(object)buildSubtarget;
        buildPlayerOptions.locationPathName = GetLocationPathName(buildTarget, buildSubtarget);
        DoBuild(buildPlayerOptions);
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    protected static void DoBuild(BuildPlayerOptions[] buildPlayerOptions) {
        foreach (BuildPlayerOptions o in buildPlayerOptions) {
            DoBuild(o);
        }
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    protected static void DoBuild(BuildPlayerOptions buildPlayerOptions) {
        Debug.Log("Building in " + buildPlayerOptions.locationPathName);
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    protected static string GetLocationPathName<T>(BuildTarget buildTarget, T buildSubtarget) {
        return Path.GetFullPath(
            Path.Combine(
                "..",
                "Build",
                GetBuildTargetName(buildTarget) + "-" + GetBuildSubtargetName(buildSubtarget),
                GetExecutableFilename()
            ),
            Application.dataPath
        );
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    protected static string GetBuildTargetName(BuildTarget buildTarget) {
        return buildTarget.ToString();
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    protected static string GetBuildSubtargetName<T>(T buildSubtarget) {
        return Enum.GetName(buildSubtarget.GetType(), buildSubtarget);
    }
#endif

    /**
     * 
     */
#if UNITY_EDITOR
    protected static string GetExecutableFilename() {
#if UNITY_EDITOR_WIN
        return Application.productName + ".exe";
#else
        return Application.productName;
#endif
    }
#endif
}
