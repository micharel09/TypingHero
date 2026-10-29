#if TYPINGHERO_AUTOMATION
using System;
using System.Diagnostics;
using UnityEngine;

// Fixed storage: collection does not serialize or allocate on the playback path.
internal static class TutorialPlaybackTrace
{
#if TYPINGHERO_AUTOMATION
    [Serializable] internal struct Record
    {
        public string kind, atlas;
        public int unityFrame, step, displayedFrame, session, slots, loopPhase;
        public float alpha;
        public double now, requested, accepted;
        public bool waiting;
    }
    [Serializable] internal sealed class Export
    {
        public Record[] records;
        public int count;
        public bool overflow;
        public double now;
    }
    static readonly Record[] records = new Record[16384];
    static int count;
    static bool overflow;

    static void Add(Record record)
    {
        if (count == records.Length) { overflow = true; return; }
        record.now = Time.realtimeSinceStartupAsDouble;
        record.unityFrame = Time.frameCount;
        records[count++] = record;
    }

    // Export only after measurement. Overflow invalidates the evidence.
    internal static string ReadAndClear()
    {
        var copy = new Record[count];
        Array.Copy(records, copy, count);
        string json = JsonUtility.ToJson(new Export {
            records = copy, count = count, overflow = overflow, now = Time.realtimeSinceStartupAsDouble
        });
        Array.Clear(records, 0, count);
        count = 0; overflow = false;
        return json;
    }
#endif

    [Conditional("TYPINGHERO_AUTOMATION")]
    internal static void Resource(string kind, string atlas, int session, int slots)
    {
#if TYPINGHERO_AUTOMATION
        Add(new Record { kind = kind, atlas = atlas, session = session, slots = slots, displayedFrame = -1 });
#endif
    }

    [Conditional("TYPINGHERO_AUTOMATION")]
    internal static void Playback(string kind, int step, double requested, double accepted,
        int frame, string atlas, bool waiting, int session, int slots, int loopPhase, float alpha)
    {
#if TYPINGHERO_AUTOMATION
        Add(new Record { kind = kind, step = step, requested = requested, accepted = accepted,
            displayedFrame = frame, atlas = atlas, waiting = waiting, session = session, slots = slots, loopPhase = loopPhase, alpha = alpha });
#endif
    }
}
#endif
