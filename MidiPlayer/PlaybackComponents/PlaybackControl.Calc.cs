using System.Linq;
using MidiStream.Enums;

namespace MidiPlayer.PlaybackComponents
{
    using Dispatchers;

    internal partial class PlaybackControl
    {
        /// <summary>
        /// calculates runtime position from given static position.
        /// </summary>
        /// <param name="position">static position</param>
        /// <returns>runtime position</returns>
        private double CalculateRuntimePosition(double position)
        {
            double runtime, fixedtime;
            ApproximatePosition(out runtime, out fixedtime, kind: out fixedtime, target: position);
            return runtime - (fixedtime - position) * _data.TickLength; // calculate exact position
        }
        
        /// <summary>
        /// calculates static position from given runtime position.
        /// </summary>
        /// <param name="position">runtime position</param>
        /// <returns>static position</returns>
        private double CalculateStaticPosition(double position)
        {
            double runtime, fixedtime;
            ApproximatePosition(out runtime, out fixedtime, kind: out runtime, target: position);
            return fixedtime - (runtime - position) / _data.TickLength; // calculate exact position
        }

        private void ApproximatePosition(out double runtime, out double fixedtime, out double kind, double target)
        {
            kind = runtime = fixedtime = 0;
            foreach (var e in _tracks
                .SelectMany(sequencer => sequencer.GetMetaEvents(keys: MetaType.SetTempo))
                .OrderBy(e => e.AbsoluteTicks))
            {
                runtime += (e.AbsoluteTicks - fixedtime)*_data.TickLength;
                fixedtime = e.AbsoluteTicks;

                if (kind > target) // aproximate up to target whether is runtime or fixedtime.
                    break;

                e.Message.DispatchTo(this); // take tempo changes in effect.
            }
        }
    }
}
