using System.ComponentModel;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace MidiStream.Enums
{
    public enum MidiType : short
    {
        Sequential,
        Synchronous,
        Asynchronous
    }

    public enum MetaType : byte
    {
        SequenceNumber = 0x00,
        Text = 0x01,
        CopyrightNotice = 0x02,
        TrackName = 0x03,
        InstrumentName = 0x04,
        Lyrics = 0x05,
        Marker = 0x06,
        CuePoint = 0x07,
        ChannelPrefix = 0x20,
        EndOfTrack = 0x2F,
        SetTempo = 0x51,
        SMPTE_Offset = 0x54,
        TimeSignature = 0x58,
        KeySignature = 0x59,
        SequencerSpecific = 0x7F,
        Reset = 0xFF
    }

    public enum VoiceType : byte
    {
        NoteOff = 0x80,
        NoteOn = 0x90,
        KeyPressure = 0xA0,
        Controller = 0xB0,
        ProgramChange = 0xC0,
        ChannelPressure = 0xD0,
        PitchWheel = 0xE0
    }

    public enum SystemCommon : byte
    {
        SysexMessage = 0xF0,
        MidiTimeCodeQuarterFrame = 0xF1,
        SongPositionPointer = 0xF2,
        SongSelect = 0xF3,
        TuneRequest = 0xF6
    }

    public enum SystemRealtime : byte
    {
        MIDIClock = 0xF8,
        MIDIStart = 0xFA,
        MIDIContinue = 0xFB,
        MIDIStop = 0xFC,
        ActiveSense = 0xFE,
        Reset = 0xFF
    }

    public enum Controller : byte
    {
        // Coarse

        BankSelect_Coarse,
        ModulationWheel_Coarse,
        BreathController_Coarse,
        FootController_Coarse = 4,
        PortamentoTime_Coarse,
        DataEntry_Coarse,
        ChannelVolume_Coarse,
        Balance_Coarse,
        Pan_Coarse = 10,
        Expression_Coarse,

        EffectControl1_Coarse,
        EffectControl2_Coarse,

        GeneralPurposeController1_Coarse = 16,
        GeneralPurposeController2_Coarse = 17,
        GeneralPurposeController3_Coarse = 18,
        GeneralPurposeController4_Coarse = 19,

        // Fine

        BankSelect_Fine = 32,
        ModulationWheel_Fine,
        BreathController_Fine,
        FootController_Fine = 36,
        PortamentoTime_Fine,
        DataEntry_Fine,
        ChannelVolume_Fine,
        Balance_Fine,
        Pan_Fine = 42,
        Expression_Fine,

        EffectControl1_Fine,
        EffectControl2_Fine,

        // Pedals

        HoldPedal1 = 64,
        DamperPedal1 = 64,
        SustainPedal1 = 64,

        PortamentoPedal,
        SostenutoPedal,
        SoftPedal,
        LegatoPedal,

        HoldPedal2 = 69,
        DamperPedal2 = 69,
        SustainPedal2 = 69,

        // other controllers

        SoundController1,
        SoundController2,
        SoundController3,
        SoundController4,
        SoundController5,
        SoundController6,
        SoundController7,
        SoundController8,
        SoundController9,
        SoundController10,

        GeneralPurposeController5,
        GeneralPurposeController6,
        GeneralPurposeController7,
        GeneralPurposeController8,

        PortamentoControl,
        HighResolutionVelocityPrefix = 88,

        Effect1Depth = 91,
        Effect2Depth,
        Effect3Depth,
        Effect4Depth,
        Effect5Depth,

        ExternalEffectDepth = 91,
        TremoloDepth,
        ChorusDepth,
        CelesteDepth,
        PhaserLevel,

        DataButtonIncrement,
        DataButtonDecrement,

        NonRegisteredParameter_Coarse,
        NonRegisteredParameter_Fine,
        RegisteredParameter_Coarse,
        RegisteredParameter_Fine,

        AllSoundOff = 120,
        AllControllersOff,
        LocalControl,
        AllNotesOff,
        OmniModeOff,
        OmniModeOn,
        MonoOperationAndAllNotesOff,
        MonoOperationAndAllNotesOn
    }

    public enum DivisionType
    {
        PPQN,
        FPS
    }

    internal enum MidiException
    {
        [Description("Not a midi file")]
        NotMidi,
        [Description("Could not find track header")]
        TrackMissing,
        //[Description("End of track reached unexpectedly.")]
        //EndOfTrack,
        //[Description("Track was not finished as expected.")]
        //TrackNotComplete,
        [Description("This file is not supported")]
        NotSupported,
        [Description("file contains invalid midi message.")]
        InvalidMessage,
        [Description("Variable Length Quantity (VLQ) overflow")]
        VLQ_Overflow
    }
}
