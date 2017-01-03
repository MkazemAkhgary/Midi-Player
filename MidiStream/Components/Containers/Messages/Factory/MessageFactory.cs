using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Collections;
using Utilities.Properties;

namespace MidiStream.Components.Containers.Messages.Factory
{
    /// <summary>
    /// Provides access to factory methods for creating Midi <see cref="MidiMessage"/>'s using singeleton pattern.
    /// </summary>
    public static class MessageFactory
    {
        private static readonly ListDictionary<Type, Func<byte[], MidiMessage>> Initialize;

        static MessageFactory()
        {
            Initialize = new ListDictionary<Type, Func<byte[], MidiMessage>>
            {
                {typeof(VoiceMessage), data => new VoiceMessage(data)},
                {typeof(MetaMessage), data => new MetaMessage(data)}
            };
        }

        public static TMessage CreateMessage<TMessage>([NotNull]byte[] data) where TMessage : MidiMessage
        {
            TMessage msg;

            if (!Containers<TMessage>.Container.TryGetValue(data, out msg))
            {
                Containers<TMessage>.Container[data] = msg = (TMessage) Initialize[typeof(TMessage)](data);
            }

            return msg;
        }

        #region Container Class

        /// <summary>
        /// container for each message of each type.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        private static class Containers<TMessage> where TMessage : MidiMessage
        {
            internal static readonly Dictionary<byte[], TMessage> Container;

            static Containers()
            {
                Container = new Dictionary<byte[], TMessage>(ArrayComparer<byte>.Create());
            }
        }

        #endregion Container Class
    }
}
