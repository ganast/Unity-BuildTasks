# Unity Build Tasks
v1.0.1 (20240414)

Automated building of various build targets right from the Unity editor main menu. May be helpful when multiple targets must be available at once, e.g., when developing a client-server multiplayer application.
    
## Installation
Just copy the UnityBuildTasks.cs file wherever it suits you under your project's Assets folder.

## Known issues
- Target availability checks do not work consistently.
- Does not handle target unavailability well.
- Succesfully building a target will cause the editor to switch to that target without feedback. This can be observed on the build settings window.
- An available target must be selected on the build settings window for any build task to proceed.

## License
Unity Build Tasks is licensed under the MIT License. For more information, see LICENSE.txt.

## Copyright
(c) 2024 by ganast <ganast@ganast.com>
