# DevTools
This plugin gives both developers and administrators some really nice features that should help you configure or develop most if not every plugin.

For example, if you're having trouble with [CommonUtils](https://github.com/KadeDev/Common-Utils/ "CommonUtils")' configuration variables and wanna know each class ID, you can use `info roles`. Or you wanna know where center of a certain room is for a plugin you're making? Just use `info room` and/or `info gotoroom <RoomName>` to get it!

### Installation
As with any EXILED plugin, you must place the DevTools.dll file inside of your "%appdata%/Roaming/Plugins" folder.

### Commands
Arguments inside &lt;&gt; are required.
| Command | Description | Arguments | Permission |
| ------------- | ----------------------- | -------------------- | -------------------- |
| `dt` | Gives you a list of every tool you get! | - | None |
| `tpdoor` | Teleports a door to your position while being able to change | &lt;DoorName&gt; &lt;xScale&gt; &lt;yScale&gt; &lt;zScale&gt; | `dt.tpdoor` |
| `nuketimer` | Set's the nuke timer to whatever you input | &lt;seconds&gt; | `dt.nuketimer` |
| `forcedecontamination` | Long names need no descriptions! (Jk, it immediately closes LCZ, no timers) | - | `dt.forcedecontamination` |
| `gotoroom` | Go to the center of a specified room | &lt;RoomName&gt; | `dt.gotoroom` |
| `testbadge` | Give a player a hidden tag that only **you should** be able to see | &lt;player&gt; &lt;text&gt; | `dt.testbadge` |
| `resetbadge` | Give that poor player his badge back :( | &lt;player&gt; | `dt.resetbadge` |
| `randompos` | Teleports you to a random spawnpoint of a specified RoleType (Type `info roles` to get a list of every role) | &lt;RoleType&gt; | `dt.randompos` |
| `info` | Source of useful information | Any of the specified below | `dt.info` |
| `info pos` | Get information on your current position | - | `dt.info.pos` |
| `info room` | Get information about the room you're in | - | `dt.info.room` |
| `info rooms` | Get every room name | - | `dt.info.rooms` |
| `info class` | Gets your current RoleType and hidden Team you're in | - | `dt.info.class` |
| `info alldoors` | Gets every door's name and their position (More complete than `door` command) | - | `dt.info.alldoors` |
| `info getobjects` | Get every GameObject's name that's around you | &lt;distance&gt; | `dt.info.getobjects` |
| `info roles` | Get every RoleType there is | - | `dt.info.roles` |
| `info reload` | Reload's this plugin's config variables (I'll move this to `dt reload` later) | - | `dt.info.reload` |
| `info version` | Get this plugin's version | - | `dt.info.version` |

### Configuration
These are the variables that should be added to your 7777-config.yml.
| Variable  | Description | Default value |
| ------------- | ------------- | ------------- |
| dt_permissionsneed | **Whether these commands require specific permissions or just RemoteAdmin access** | `false` |

### That'd be all
Needless to say, the code is completely open-source and it's intention is to help developers make better plugins, so feel free to copy whatever you find useful from my code!

Thanks for passing by, have a nice day! :)
