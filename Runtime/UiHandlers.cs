using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Framework.Utility
{
    /// <summary>
    /// Use this to add global listeners for <see cref="IEventSystemHandler"/>
    /// </summary>
    public static class UiHandlers
    {
        private static readonly (Type genericType, FieldInfo field)[] EventsFields;

        //lookups and typeof(T) in AddListener/RemoveListener and could be removed by making the whole class generic and only storing a static field of type FieldInfo and calling that but i am not sure how generic sharing would work in that case (id like to keep code size small)
        //if you want to know how this really works check ExecuteEvents source code

        static UiHandlers()
        {
            var fields = typeof(ExecuteEvents).GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            var fieldsLength = fields.Length;
            var array = new (Type, FieldInfo)[fieldsLength];

            var targetType = typeof(ExecuteEvents.EventFunction<>);

            var length = 0;
            for (var fieldIndex = 0; fieldIndex < fieldsLength; fieldIndex++)
            {
                var fieldInfo = fields[fieldIndex];

                var fieldInfoFieldType = fieldInfo.FieldType;
                if (fieldInfoFieldType.IsConstructedGenericType == false)
                    continue;

                var genericType = fieldInfoFieldType.GetGenericTypeDefinition();
                if (genericType != targetType)
                    continue;

                var arguments = fieldInfoFieldType.GetGenericArguments();
                array[length] = (arguments[0], fieldInfo);
                length++;
            }

            EventsFields = new (Type, FieldInfo)[length];
            for (var index = 0; index < length; index++)
                EventsFields[index] = array[index];
        }

        public static void AddListener<T>(ExecuteEvents.EventFunction<T> value) where T : IEventSystemHandler
        {
            var type = typeof(T);
            var eventsFieldsLength = EventsFields.Length;
            for (var index = 0; index < eventsFieldsLength; index++)
            {
                var eventsField = EventsFields[index];
                if (eventsField.genericType != type)
                    continue;

                var currentValue = (ExecuteEvents.EventFunction<T>)eventsField.field.GetValue(null);
                currentValue += value;
                eventsField.field.SetValue(null, currentValue);
                return;
            }

            throw new Exception($"Could not find field of type {typeof(ExecuteEvents.EventFunction<T>)}");
        }

        public static void RemoveListener<T>(ExecuteEvents.EventFunction<T> value) where T : IEventSystemHandler
        {
            var type = typeof(T);
            var eventsFieldsLength = EventsFields.Length;
            for (var index = 0; index < eventsFieldsLength; index++)
            {
                var eventsField = EventsFields[index];
                if (eventsField.genericType != type)
                    continue;

                var currentValue = (ExecuteEvents.EventFunction<T>)eventsField.field.GetValue(null);
                currentValue -= value;
                eventsField.field.SetValue(null, currentValue);
                return;
            }

            throw new Exception($"Could not find field of type {typeof(ExecuteEvents.EventFunction<T>)}");
        }

        public static async Task<Tuple<T, BaseEventData>> WaitGlobalEvent<T>(Func<T, BaseEventData, bool> shouldIgnore, CancellationToken token) where T : IEventSystemHandler
        {
            Tuple<T, BaseEventData> result = null;
            void Invoked(T handler, BaseEventData eventData)
            {
                if (shouldIgnore != null && shouldIgnore(handler, eventData))
                    return;

                result = new Tuple<T, BaseEventData>(handler, eventData);
            }

            try
            {
                AddListener<T>(Invoked);

                while (result == null)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Yield();
                }

                return result;
            }
            finally
            {
                RemoveListener<T>(Invoked);
            }
        }

        public static async Task<Tuple<T, BaseEventData>> WaitGlobalEvent<T>(Func<IEventSystemHandler, BaseEventData, bool> shouldIgnore, CancellationToken token) where T : IEventSystemHandler
        {
            Tuple<T, BaseEventData> result = null;
            void Invoked(T handler, BaseEventData eventData)
            {
                if (shouldIgnore != null && shouldIgnore(handler, eventData))
                    return;

                result = new Tuple<T, BaseEventData>(handler, eventData);
            }

            try
            {
                AddListener<T>(Invoked);

                while (result == null)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Yield();
                }

                return result;
            }
            finally
            {
                RemoveListener<T>(Invoked);
            }
        }
    }

}