using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DevTools {
    public static class Extensions {

        // Ty Joker for these beautiful Extensions <3
        public static void RAMessage( this CommandSender sender, string message, bool success = true ) =>
            sender.RaReply("<color=green>DT</color>#" + message, success, true, string.Empty);

        public static void Broadcast( this ReferenceHub rh, uint time, string message ) => 
            rh.GetComponent<Broadcast>().TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, false);

        public static void InvokeStaticMethod( this Type type, string methodName, object [] param ) {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Public;
            MethodInfo info = type.GetMethod(methodName, flags);
            info?.Invoke(null, param);
        }
        public static IEnumerable<T> GetValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static Vector3 GetRandomSpawnpoint( this RoleType type ) => UnityEngine.Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(type).transform.position;

        // Java ftw
        public static bool EqualsIgnoreCase( this string t, string to ) {
            return t.ToLower() == to.ToLower();
        }
    }
}