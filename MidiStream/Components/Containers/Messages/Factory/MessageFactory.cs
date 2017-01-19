using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Utilities.Comparers;

namespace MidiStream.Components.Containers.Messages.Factory
{
    /// <summary>
    /// Provides access to factory methods for creating Midi <see cref="MidiMessage"/>'s using singeleton pattern.
    /// </summary>
    internal static class MessageFactory
    {
        [NotNull]
        public static TMessage CreateMessage<TMessage>([NotNull] byte[] data) where TMessage : MidiMessage
        {
            if(data == null) throw new ArgumentNullException(nameof(data));

            TMessage msg;

            if (!Containers<TMessage>.Container.TryGetValue(data, out msg))
            {
                Containers<TMessage>.Container[data] = msg = Containers<TMessage>.Initializer(data);
            }

            return msg;
        }

        #region Container Class

        /// <summary>
        /// container for each message of each type.
        /// </summary>
        private static class Containers<TMessage> where TMessage : MidiMessage
        {
            public static Dictionary<byte[], TMessage> Container { get; }

            public static Func<byte[], TMessage> Initializer { get; }

            static Containers()
            {
                Container = new Dictionary<byte[], TMessage>(ArrayComparer<byte>.Create());

                const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

                var ctor = typeof(TMessage)
                    .GetConstructor(flags, null, CallingConventions.HasThis, new[] {typeof(byte[])}, null);

                var param = Expression.Parameter(typeof(byte[]), "data");
                var expr = Expression.New(ctor, param);
                var lambda = Expression.Lambda<Func<byte[], TMessage>>(expr, typeof(TMessage).Name, new[] {param});
                Initializer = lambda.Compile();
            }
        }

        #endregion Container Class
    }
}
