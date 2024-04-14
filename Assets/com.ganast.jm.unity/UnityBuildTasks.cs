using System.IO;

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor.Build.Reporting;

namespace com.ganast.jm.unity {

    /// <summary>
    /// 
    /// </summary>
    public class UnityBuildTasks {

        /// <summary>
        /// 
        /// </summary>
        private const string MENU_PREFIX = "BuildTasks/";

        /// <summary>
        /// 
        /// </summary>
        private const string MENUITEM_BUILD_ALL = "Build all targets";
        private const string MENUITEM_ABOUT = "About UnityBuildTasks";

        /// <summary>
        /// 
        /// </summary>
        private const string WIN64_STANDALONE_PLAYER = "Windows 64 standalone player";
        private const string LNX64_STANDALONE_PLAYER = "Linux 64 standalone player";
        private const string WIN64_STANDALONE_SERVER = "Windows 64 standalone server";
        private const string LNX64_STANDALONE_SERVER = "Linux 64 standalone server";

        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, BuildOption> buildOptions = new Dictionary<string, BuildOption>() {

            { WIN64_STANDALONE_PLAYER, new BuildOption(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Player) },

            { LNX64_STANDALONE_PLAYER, new BuildOption(BuildTarget.StandaloneLinux64, StandaloneBuildSubtarget.Player) },

            { WIN64_STANDALONE_SERVER, new BuildOption(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Server) },

            { LNX64_STANDALONE_SERVER, new BuildOption(BuildTarget.StandaloneLinux64, StandaloneBuildSubtarget.Server) }
        };

        /// <summary>
        /// 
        /// </summary>
        [MenuItem(MENU_PREFIX + MENUITEM_BUILD_ALL + "...", false, 10)]
        public static void BuildAllTargets() {
            if (!EditorUtility.DisplayDialog("Build all targets", "Are you sure you want to build all targets? " +
                "This might take some time and take up a lot of disk space.", "OK", "Cancel"))
            {
                return;
            }
            foreach (KeyValuePair<string, BuildOption> i in buildOptions) {
                i.Value.Build();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem(MENU_PREFIX + WIN64_STANDALONE_PLAYER, false, 110)]
        public static void BuildWin64StandalonePlayer() {
            buildOptions[WIN64_STANDALONE_PLAYER].Build();
        }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem(MENU_PREFIX + LNX64_STANDALONE_PLAYER, false, 111)]
        public static void BuildLinux64StandalonePlayer() {
            buildOptions[LNX64_STANDALONE_PLAYER].Build();
        }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem(MENU_PREFIX + WIN64_STANDALONE_SERVER, false, 210)]
        public static void BuildWin64StandaloneServer() {
            buildOptions[WIN64_STANDALONE_SERVER].Build();
        }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem(MENU_PREFIX + LNX64_STANDALONE_SERVER, false, 211)]
        public static void BuildLinux64StandaloneServer() {
            buildOptions[LNX64_STANDALONE_SERVER].Build();
        }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem(MENU_PREFIX + MENUITEM_ABOUT + "...", false, 9010)]
        public static void About() {
            EditorUtility.DisplayDialog("About", "Unity Build Tasks v1.0\n\n(c) 2024 by ganast <ganast@ganast." +
                "com>\n\nFor more information see https://github.com/ganast/Unity-BuildTasks", "OK");
        }

        /// <summary>
        /// 
        /// </summary>
        private class BuildOption {

            /// <summary>
            /// 
            /// </summary>
            private readonly BuildPlayerOptions buildPlayerOptions;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buildTarget"></param>
            /// <param name="standaloneBuildSubtarget"></param>
            public BuildOption(BuildTarget buildTarget, StandaloneBuildSubtarget standaloneBuildSubtarget) {
                this.buildPlayerOptions = GetBuildPlayerOptions(buildTarget, standaloneBuildSubtarget);
            }

            /// <summary>
            /// 
            /// </summary>
            public BuildReport Build() {
                BuildTarget target = buildPlayerOptions.target;
                if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, target)) {
                    Debug.Log("Build target " + GetBuildTargetName(target) + " not supported, build aborted.");
                    return null;
                }
                Debug.Log("Building " + buildPlayerOptions.locationPathName);
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                Debug.Log("Build " + (report.summary.result == BuildResult.Succeeded ? " succeeded" : "failed"));
                return report;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buildTarget"></param>
            /// <param name="buildSubtarget"></param>
            /// <returns></returns>
            protected static BuildPlayerOptions GetBuildPlayerOptions(BuildTarget buildTarget, StandaloneBuildSubtarget buildSubtarget) {
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
                    target = buildTarget,
                    subtarget = (int) (object) buildSubtarget,
                    locationPathName = GetLocationPathName(buildTarget, buildSubtarget)
                };
                return buildPlayerOptions;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buildTarget"></param>
            /// <param name="buildSubtarget"></param>
            /// <returns></returns>
            protected static string GetLocationPathName(BuildTarget buildTarget, StandaloneBuildSubtarget buildSubtarget) {
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

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buildTarget"></param>
            /// <returns></returns>
            protected static string GetBuildTargetName(BuildTarget buildTarget) {
                return buildTarget.ToString();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="buildSubtarget"></param>
            /// <returns></returns>
            protected static string GetBuildSubtargetName(StandaloneBuildSubtarget buildSubtarget) {
                return Enum.GetName(buildSubtarget.GetType(), buildSubtarget);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected static string GetExecutableFilename() {
#if UNITY_EDITOR_WIN
                return Application.productName + ".exe";
#else
            return Application.productName;
#endif
            }
        }
    }
}

#endif
