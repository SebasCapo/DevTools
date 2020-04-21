using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.Extensions;
using MEC;
using Log = EXILED.Log;
using UnityEngine;
using Mirror;
using EXILED.ApiObjects;

namespace DevTools {

    public class EventHandlers {
        public Plugin plugin;
        public int count;

        public EventHandlers( Plugin plugin ) {
            this.plugin = plugin;
        }

        public void OnCommand( ref RACommandEvent ev ) {
            try {
                if(ev.Command.Contains("REQUEST_DATA PLAYER_LIST SILENT")) return;
                string[] args = ev.Command.Split(' ');
                ReferenceHub sender = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? Player.GetPlayer(PlayerManager.localPlayer) : Player.GetPlayer(ev.Sender.SenderId);
                if(args[0].EqualsIgnoreCase("dt")) {
                    ev.Allow = false;
                    ev.Sender.RAMessage("<color=green>Thank you for installing the plugin, here's a list of every command:</color>"
                        + "\n \"tpdoor <doorname> <xScale> <yScale> <zScale>\""
                        + "\n \"nuketimer <seconds>\""
                        + "\n \"randompos <RoleType>\""
                        + "\n \"forcedecontamination\""
                        + "\n \"gotoroom <roomname>\""
                        + "\n \"testbadge <player> <text>\""
                        + "\n \"resetbadge <player>\""
                        + "\n \"info <arg>\" (Type \"info\" for more help on every possible argument!)"
                        );
                    return;
                } else if(args[0].EqualsIgnoreCase("randompos")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "randompos")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length == 1) {
                        string a = "";
                        foreach(RoleType i in Extensions.GetValues<RoleType>())
                            a += " - " + i.ToString();
                        ev.Sender.RAMessage(CorrectUsage("randompos <RoleType>"));
                        ev.Sender.RAMessage($"Here's a list with every RoleType: {a}");
                        return;
                    }
                    if(Enum.TryParse(args[1], out RoleType type)) {
                        sender.SetPosition(type.GetRandomSpawnpoint());
                        ev.Sender.RAMessage($"<color=green>You've been teleported to a random {type.ToString()} spawnpoint.</color>");
                    } else {
                        string a = "";
                        foreach(RoleType i in Extensions.GetValues<RoleType>())
                            a += " - " + i.ToString();
                        ev.Sender.RAMessage(CorrectUsage("randompos <RoleType>"));
                        ev.Sender.RAMessage($"Here's a list with every RoleType: {a}");
                    }
                    return;
                } else if(args[0].EqualsIgnoreCase("tpdoor")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "tpdoor")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length < 5) {
                        ev.Sender.RAMessage(CorrectUsage("tpdoor <doorname> <xScale> <yScale> <zScale>"));
                        ev.Sender.RAMessage("<i>If you're having trouble getting door names try typing <b>\"doors\" or \"info alldoors\"</b></i>");
                        return;
                    }
                    foreach(Door d in UnityEngine.Object.FindObjectsOfType<Door>().ToList()) {
                        if(d.DoorName.ToLower().EqualsIgnoreCase(args[1]) || args[1] == "*") {
                            // Used help from Joker's code tyty <3
                            GameObject target = d.gameObject;
                            try {
                                NetworkIdentity identity = target.GetComponent<NetworkIdentity>();

                                target.transform.position = new Vector3(sender.GetPosition().x, sender.GetPosition().y - 1.2f, sender.GetPosition().z);

                                float.TryParse(args[2], out float x);
                                float.TryParse(args[3], out float y);
                                float.TryParse(args[4], out float z);

                                target.transform.localScale = new Vector3(x, y, z);

                                ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage {
                                    netId = identity.netId
                                };

                                foreach(GameObject player in PlayerManager.players) {
                                    if(player == target)
                                        continue;

                                    NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;

                                    playerCon.Send(destroyMessage, 0);

                                    object[] parameters = new object[] { identity, playerCon };
                                    typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
                                }
                            } catch(Exception e) {
                                Log.Error("Command error: " + e.ToString());
                            }
                        }

                    }
                    return;
                } else if(args[0].EqualsIgnoreCase("nuketimer")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "nuketimer")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length == 1) {
                        ev.Sender.RAMessage(CorrectUsage("nuketimer <seconds>"));
                        return;
                    }
                    AlphaWarheadController.Host.timeToDetonation = float.Parse(args[1]);
                    ev.Sender.RAMessage($"<color=green>Time until detonation has been set to {AlphaWarheadController.Host.timeToDetonation}.</color>");
                    return;
                } else if(args[0].EqualsIgnoreCase("forcedecontamination")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "forcedecontamination")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    Map.StartDecontamination();
                    ev.Sender.RAMessage($"<color=green>Decontamination has been forced.</color>");
                    return;
                } else if(args[0].EqualsIgnoreCase("gotoroom")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "gotoroom")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length == 1) {
                        ev.Sender.RAMessage(CorrectUsage("gotoroom <room_name>"));
                        return;
                    }
                    string roomName = ev.Command.Substring(ev.Command.LastIndexOf(args[0]));
                    foreach(Room r in Map.Rooms) if(r.Name.ToLower().EqualsIgnoreCase(roomName)) sender.SetPosition(r.Position.x, r.Position.y + 1, r.Position.z);
                    ev.Sender.RAMessage($"<color=green>You've been teleported to: {roomName}.</color>");
                    return;
                } else if(args[0].EqualsIgnoreCase("testbadge")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "testbadge")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length <= 2) {
                        ev.Sender.RAMessage(CorrectUsage("testbadge <player> <text>"));
                        return;
                    }
                    
                    ReferenceHub p = Player.GetPlayer(args[1]);

                    p?.serverRoles.TargetSetHiddenRole(sender.serverRoles.connectionToClient, args[2]);
                    ev.Sender.RAMessage("<color=green>Set " + p?.GetNickname() + "'s fake rank to: " + args[2]);
                    return;
                } else if(args[0].EqualsIgnoreCase("resetbadge")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "resetbadge")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length == 1) {
                        ev.Sender.RAMessage(CorrectUsage("resetbadge <player>"));
                        return;
                    }

                    ReferenceHub p = Player.GetPlayer(args[1]);
                    string b;
                    if(p?.GetRank() == null) b = "";
                    else b = p?.GetRank().BadgeText;

                    p?.serverRoles.TargetSetHiddenRole(sender.serverRoles.connectionToClient, b);
                    ev.Sender.RAMessage("<color=green>Set " + p?.GetNickname() + "'s fake rank to: " + b);
                    return;
                } else if(args[0].EqualsIgnoreCase("info")) {
                    ev.Allow = false;
                    if(!HasPermission(sender, "info")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length >= 2) {
                        if(args[1].EqualsIgnoreCase("pos")) {
                            if(!HasPermission(sender, "info.pos")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            string t = "";
                            foreach(ReferenceHub player in sender.GetCurrentRoom().GetHubs())
                                t += player.GetNickname() + " - ";
                            ev.Sender.RAMessage($"<color=green>----------</color>" +
                                $"\nPos: <b>X{sender.GetPosition().x} Y{sender.GetPosition().y} Z{sender.GetPosition().z}</b>" +
                                $"\nRotation: <b>{sender.GetRotationVector()}</b>" +
                                $"\nRoom: <b>{sender.GetCurrentRoom()?.Name} -- {sender.GetCurrentRoom()?.Zone}</b>" +
                                $"\nTry using \"info room\" for more information.");
                            return;
                        } else if(args[1].EqualsIgnoreCase("room")) {
                            if(!HasPermission(sender, "info.room")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            string t = "";
                            foreach(ReferenceHub player in sender.GetCurrentRoom().GetHubs())
                                t += player.GetNickname() + " - ";
                            ev.Sender.RAMessage($"<color=green>----------</color>" +
                                $"\nRoom: <b>{sender.GetCurrentRoom().Name} -- {sender.GetCurrentRoom().Zone}</b>" +
                                $"\nRoom Info: <b>X: {sender.GetCurrentRoom().Position.x} Y: {sender.GetCurrentRoom().Position.y} Z: {sender.GetCurrentRoom().Position.z}</b>" +
                                $"\nRoom Players: <b>({sender.GetCurrentRoom().GetHubs().Count()}) {t}</b>");
                            return;
                        } else if(args[1].EqualsIgnoreCase("class")) {
                            if(!HasPermission(sender, "info.class")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            ev.Sender.RAMessage($"<color=green>----------</color>" +
                                $"\nClass: <b>{sender.GetRole()}</b>" +
                                $"\nTeam: <b>{sender.GetTeam()}</b>");
                            return;
                        } else if(args[1].EqualsIgnoreCase("rooms")) {
                            if(!HasPermission(sender, "info.rooms")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            string names = "";
                            foreach(Room room in Map.Rooms) names += $"{room.Name}, ";
                            ev.Sender.RAMessage(names);
                            return;
                        } else if(args[1].EqualsIgnoreCase("version")) {
                            if(!HasPermission(sender, "info.version")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            ev.Sender.RAMessage("You're currently using " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                            return;
                        } else if(args[1].EqualsIgnoreCase("alldoors")) {
                            if(!HasPermission(sender, "info.alldoors")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            ev.Sender.RAMessage("<b>List of Doors found:</b>");
                            string t = "";
                            foreach(Door d in UnityEngine.Object.FindObjectsOfType<Door>().ToList())
                                t += $"\n{d.DoorName}   -   <color=green>Position: X: {d.transform.position.x} Y: {d.transform.position.y} Z: {d.transform.position.z}</color>";
                                ev.Sender.RAMessage(t);
                            return;
                        } else if(args[1].EqualsIgnoreCase("getobjects")) {
                            if(!HasPermission(sender, "info.getobjects")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            if(args.Length == 2) {
                                ev.Sender.RAMessage(CorrectUsage("info getobjects <range>"));
                                return;
                            }
                            if(float.TryParse(args[2], out float range)) {
                                ev.Sender.RAMessage("<b>List of GameObjects found:</b>");
                                string txt = "";
                                foreach(Collider c in Physics.OverlapSphere(sender.GetPosition(), range))
                                    txt += $"\nFound: {c.gameObject.name}     (X: {c.transform.position.x} Y: {c.transform.position.y} Z: {c.transform.position.z})";
                                    ev.Sender.RAMessage(txt);
                                return;
                            } else {
                                ev.Sender.RAMessage(CorrectUsage("info getobjects <range>"));
                                return;
                            }
                        } else if(args[1].EqualsIgnoreCase("reload")) {
                            if(!HasPermission(sender, "info.reload")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            ev.Sender.RAMessage("<b>Config variables reloaded!</b>");
                            plugin.ReloadConfig();
                            return;
                        } else if(args[1].EqualsIgnoreCase("roles")) {
                            if(!HasPermission(sender, "info.roles")) {
                                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                                return;
                            }
                            string a = "";
                            foreach(RoleType i in Extensions.GetValues<RoleType>())
                                a += $" - {i.ToString()} ({(int) i})";
                            ev.Sender.RAMessage($"Here's a list with every RoleType: \n{a}");
                            return;
                        }
                    } 
                    ev.Sender.RAMessage("<color=red>Try typing \"info\" alongside any of these arguments: pos - room - class - version - rooms - alldoors - getobjects - roles - reload</color>");
                }
                return;
            } catch(Exception e) {
                Log.Error("Command error: " + e.StackTrace);
            }
        }

        public bool HasPermission(ReferenceHub hub, string perm) {
            if(plugin.needPermission && (hub.CheckPermission("dt.*") || hub.CheckPermission("dt." + perm))) return true;
            else return !plugin.needPermission && hub.serverRoles.RemoteAdmin;
        }

        public string CorrectUsage( string correctUsage ) {
            return $"<color=red>Please try using: \"{correctUsage}\"</color>";
        }

    }
}