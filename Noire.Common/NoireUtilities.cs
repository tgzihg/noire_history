﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common {
    public static class NoireUtilities {

        public static void RaiseEvent<T>(EventHandler<T> handler, object sender, T e) where T : EventArgs {
            handler?.Invoke(sender, e);
        }

        public static void RaiseEvent(EventHandler handler, object sender, EventArgs e) {
            handler?.Invoke(sender, e);
        }

        public static void DisposeNonPublicDeclaredFields<T>(T disposeBase) where T : DisposeBase {
            DisposeNonPublicFields(disposeBase, BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void DisposeNonPublicFields<T>(T disposeBase) where T : DisposeBase {
            DisposeNonPublicFields(disposeBase, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static void DisposeNonPublicFields<T>(T disposeBase, BindingFlags flags) where T : DisposeBase {
            var type = typeof(T);
            var disposeBaseType = typeof(DisposeBase);
            var fields = type.GetFields(flags);
            foreach (var field in fields) {
                if (field.FieldType.IsSubclassOf(disposeBaseType)) {
                    var value = field.GetValue(disposeBase) as DisposeBase;
                    value?.Dispose();
                    field.SetValue(disposeBase, null);
                }
            }
        }

    }
}
