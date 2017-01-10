using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Utilities.Helpers;
using Utilities.Properties;

namespace MidiStream.Components.Containers.Messages.Factory
{
    /// <summary>
    /// Provides access to factory methods for creating Midi <see cref="MidiMessage"/>'s using singeleton pattern.
    /// </summary>
    public static class MessageFactory
    {
        public static TMessage CreateMessage<TMessage>([NotNull]byte[] data) where TMessage : MidiMessage
        {
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

                var ctor = typeof(TMessage)
                    .GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, CallingConventions.HasThis,
                        new[] {typeof(byte[])}, null);

                var param = Expression.Parameter(typeof(byte[]), "data");
                var expr = Expression.New(ctor, param);
                var name = Regex.Match(typeof(TMessage).Name, @".*(.*)").Value;
                var lambda = Expression.Lambda<Func<byte[], TMessage>>(expr, name, new[] {param});
                Initializer = lambda.Compile();
            }
        }

        #endregion Container Class
    }
}
