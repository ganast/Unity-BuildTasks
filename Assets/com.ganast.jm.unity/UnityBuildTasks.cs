// Copyright(c) 2024 by ganast<ganast@ganast.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the “Software”), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.IO;

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor.Build.Reporting;

namespace com.ganast.jm.unity {

    /// <summary>
    /// Provides functionality and main menu additions for automated building of various build targets right from
    /// the Unity editor main menu. May be helpful when multiple targets must be available at once, e.g., when
    /// developing a client-server multiplayer application.
    /// </summary>
    public class UnityBuildTasks {

        // various key constants...
        private const string ABOUT_TITLE = "About";
        private const string ABOUT_TEXT = "Unity Build Tasks v1.0\n\n(c) 2024 by ganast <ganast@ganast.com>\n" +
            "\nFor more information see https://github.com/ganast/Unity-BuildTasks";
        private const string BUILD_SUBDIR = "Build";
        private const string BUILDALL_TITLE = "Build all targets";
        private const string BUILDALL_TEXT = "Are you sure you want to build all targets? This might take some " +
            "time and a lot of disk space.";

        // the prefix to all menu items, parts separated by /, first part is the main meny entry...
        private const string MENU_PREFIX = "BuildTasks/";

        // menu item titles for some standard items..
        private const string MENUITEM_BUILD_ALL = "Build all targets";
        private const string MENUITEM_ABOUT = "About UnityBuildTasks";

        // menu item titles for build targets...
        private const string WIN64_STANDALONE_PLAYER = "Windows 64 standalone player";
        private const string LNX64_STANDALONE_PLAYER = "Linux 64 standalone player";
        private const string WIN64_STANDALONE_SERVER = "Windows 64 standalone server";
        private const string LNX64_STANDALONE_SERVER = "Linux 64 standalone server";

        // associates build target menu items with their respective build options...
        private static readonly Dictionary<string, BuildOption> buildOptions = new Dictionary<string, BuildOption>() {

            // Windows standalone player...
            { WIN64_STANDALONE_PLAYER, new BuildOption(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Player) },

            // Linux standalone player...
            { LNX64_STANDALONE_PLAYER, new BuildOption(BuildTarget.StandaloneLinux64, StandaloneBuildSubtarget.Player) },

            // Windows standalone server (headless)...
            { WIN64_STANDALONE_SERVER, new BuildOption(BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Server) },

            // Linux standalone server (headless)...
            { LNX64_STANDALONE_SERVER, new BuildOption(BuildTarget.StandaloneLinux64, StandaloneBuildSubtarget.Server) }
        };

        /// <summary>
        /// Handler for the menu item for building all targets.
        /// </summary>
        [MenuItem(MENU_PREFIX + MENUITEM_BUILD_ALL + "...", false, 10)]
        public static void BuildAllTargets() {
            if (!EditorUtility.DisplayDialog(BUILDALL_TITLE, BUILDALL_TEXT, "OK", "Cancel")) {
                return;
            }
            foreach (KeyValuePair<string, BuildOption> i in buildOptions) {
                i.Value.Build();
            }
        }

        /// <summary>
        /// Handler for a Windows standalone player build task menu item.
        /// </summary>
        [MenuItem(MENU_PREFIX + WIN64_STANDALONE_PLAYER, false, 110)]
        public static void BuildWin64StandalonePlayer() {
            buildOptions[WIN64_STANDALONE_PLAYER].Build();
        }

        /// <summary>
        /// Handler for a Linux standalone player build task menu item.
        /// </summary>
        [MenuItem(MENU_PREFIX + LNX64_STANDALONE_PLAYER, false, 111)]
        public static void BuildLinux64StandalonePlayer() {
            buildOptions[LNX64_STANDALONE_PLAYER].Build();
        }

        /// <summary>
        /// Handler for a Windows standalone server build task menu item.
        /// </summary>
        [MenuItem(MENU_PREFIX + WIN64_STANDALONE_SERVER, false, 210)]
        public static void BuildWin64StandaloneServer() {
            buildOptions[WIN64_STANDALONE_SERVER].Build();
        }

        /// <summary>
        /// Handler for a Linux standalone server build task menu item.
        /// </summary>
        [MenuItem(MENU_PREFIX + LNX64_STANDALONE_SERVER, false, 211)]
        public static void BuildLinux64StandaloneServer() {
            buildOptions[LNX64_STANDALONE_SERVER].Build();
        }

        /// <summary>
        /// Handler for the About dialog menu item.
        /// </summary>
        [MenuItem(MENU_PREFIX + MENUITEM_ABOUT + "...", false, 9010)]
        public static void About() {
            EditorUtility.DisplayDialog(ABOUT_TITLE, ABOUT_TEXT, "OK");
        }

        /// <summary>
        /// A class that represents a set of build options, namely, a build target, a standalone guild subtarget
        /// and an automatically-generated file path for the actual build.
        /// </summary>
        private class BuildOption {

            // build target and standalone build subtarget for this build option...
            private readonly BuildPlayerOptions buildPlayerOptions;

            /// <summary>
            /// Creates a <see cref="BuildOption"/> for the specified <see cref="BuildTarget"/> and
            /// <see cref="StandaloneBuildSubtarget"/>.
            /// </summary>
            /// <param name="buildTarget">The <see cref="BuildTarget"/> for the instance.</param>
            /// <param name="standaloneBuildSubtarget">The <see cref="StandaloneBuildSubtarget"/> for the instance.</param>
            public BuildOption(BuildTarget buildTarget, StandaloneBuildSubtarget standaloneBuildSubtarget) {
                this.buildPlayerOptions = GetBuildPlayerOptions(buildTarget, standaloneBuildSubtarget);
            }

            /// <summary>
            /// Builds the target represented by this <see cref="BuildOption"/>.
            /// </summary>
            /// <returns>A <see cref="BuildReport"/> if the build task was succesful, <c>null</c> otherwise.</returns>
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
            /// Generates a <see cref="BuildPlayerOptions"/> instance for the specified <see cref="BuildTarget"/> and
            /// <see cref="StandaloneBuildSubtarget"/>.
            /// </summary>
            /// <param name="buildTarget">The <see cref="BuildTarget"/> for the <see cref="BuildPlayerOptions"/>
            /// to be generated.</param>
            /// <param name="buildSubtarget">The <see cref="StandaloneBuildSubtarget"/> for the <see cref="BuildPlayerOptions"/>
            /// to be generated.</param>
            /// <returns>A <see cref="BuildPlayerOptions"/> instance.</returns>
            protected static BuildPlayerOptions GetBuildPlayerOptions(BuildTarget buildTarget, StandaloneBuildSubtarget buildSubtarget) {
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
                    target = buildTarget,
                    subtarget = (int) (object) buildSubtarget,
                    locationPathName = GetLocationPathName(buildTarget, buildSubtarget)
                };
                return buildPlayerOptions;
            }

            /// <summary>
            /// Generates a file path for a build of the specified <see cref="BuildTarget"/> and
            /// <see cref="StandaloneBuildSubtarget"/>.
            /// </summary>
            /// <param name="buildTarget">A <see cref="BuildTarget"/> for a build at the generated path.</param>
            /// <param name="buildSubtarget">A <see cref="StandaloneBuildSubtarget"/> for a build at the generated
            /// path.</param>
            /// <returns>A file path for a build of the specified <see cref="BuildTarget"/> and <see cref="StandaloneBuildSubtarget"/>.</returns>
            protected static string GetLocationPathName(BuildTarget buildTarget, StandaloneBuildSubtarget buildSubtarget) {
                return Path.GetFullPath(
                    Path.Combine(
                        "..",
                        BUILD_SUBDIR,
                        GetBuildTargetName(buildTarget) + "-" + GetBuildSubtargetName(buildSubtarget),
                        GetExecutableFilename()
                    ),
                    Application.dataPath
                );
            }

            /// <summary>
            /// Generates a name for the specified <see cref="BuildTarget"/>.
            /// </summary>
            /// <param name="buildTarget">The <see cref="BuildTarget"/> to generate a name for.</param>
            /// <returns>A name for the specified <see cref="BuildTarget"/>.</returns>
            protected static string GetBuildTargetName(BuildTarget buildTarget) {
                return buildTarget.ToString();
            }

            /// <summary>
            /// Generates a name for the specified <see cref="StandaloneBuildSubtarget"/>.
            /// </summary>
            /// <param name="buildSubtarget">The <see cref="StandaloneBuildSubtarget"/> to generate a name for.</param>
            /// <returns>A name for the specified <see cref="StandaloneBuildSubtarget"/>.</returns>
            protected static string GetBuildSubtargetName(StandaloneBuildSubtarget buildSubtarget) {
                return Enum.GetName(buildSubtarget.GetType(), buildSubtarget);
            }

            /// <summary>
            /// Generates a name for the project's executable.
            /// </summary>
            /// <returns>A name for the project's executable./></returns>
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
