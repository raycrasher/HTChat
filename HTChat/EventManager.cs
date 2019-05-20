using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTChat
{
    public interface IEventHandler<T>
        where T: EventArgs
    {
        void HandleEvent(object from, T args);
    }

    public static class EventManagerHelper
    {
        public static void RegisterEventHandler<T>(this IEventHandler<T> obj)
            where T: EventArgs
        {
            EventManager<T>.RegisterEventHandler(obj);
        }

        public static void FireEvent<T>(this object from, T args)
            where T : EventArgs
        {
            EventManager<T>.FireEvent(from, args);
        }
    }

    public class EventManager<T>
        where T: EventArgs
    {
        static Dictionary<Type, HashSet<WeakReference<IEventHandler<T>>>> _handlers = new Dictionary<Type, HashSet<WeakReference<IEventHandler<T>>>>();

        public static void RegisterEventHandler(IEventHandler<T> obj)
        {
            Dispatch.InvokeUI(() =>
            {
                var type = typeof(T);
                if (!_handlers.TryGetValue(type, out var coll))
                {
                    coll = new HashSet<WeakReference<IEventHandler<T>>>();
                    _handlers[type] = coll;
                }
                coll.Add(new WeakReference<IEventHandler<T>>(obj));
            });
        }

        public static void FireEvent(object from, T args)
        {
            Dispatch.InvokeUI(() => {
                var type = typeof(T);
                if (_handlers.TryGetValue(type, out var coll))
                {
                    coll.RemoveWhere(r => {
                        var result = r.TryGetTarget(out var target);
                        if (result)                        
                            target.HandleEvent(from, args);
                        return !result;
                        });
                }                
            });
        }
    }
}
