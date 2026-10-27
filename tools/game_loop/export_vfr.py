"""Export a lossless review clip without changing frame PTS or held durations.

The source must have increasing video PTS and an explicit last-packet duration.
Intermediate durations come from the next PTS, never from a nominal frame rate.
Reports preserve failures; use a new output path for each attempt.
"""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess


def packets(probe, path):
    return json.loads(subprocess.check_output([
        str(probe), "-v", "error", "-select_streams", "v:0", "-show_packets",
        "-show_entries", "packet=pts_time,duration_time", "-of", "json", str(path)
    ]))["packets"]


def pixels(ffmpeg, path):
    data = subprocess.check_output([
        str(ffmpeg), "-v", "error", "-i", str(path), "-map", "0:v:0",
        "-fps_mode", "passthrough", "-f", "framemd5", "-"
    ]).decode()
    return [line.split(",")[-1].strip() for line in data.splitlines()
            if not line.startswith("#")]


def extent(probe, path):
    return json.loads(subprocess.check_output([
        str(probe), "-v", "error", "-select_streams", "v:0",
        "-show_entries", "format=duration,start_time:stream=duration,start_time",
        "-of", "json", str(path)
    ]))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source", type=Path)
    parser.add_argument("output", type=Path)
    parser.add_argument("--ffmpeg", type=Path, required=True)
    parser.add_argument("--ffprobe", type=Path, required=True)
    args = parser.parse_args()
    report_path = args.output.with_suffix(".audit.json")
    if args.output.exists() or report_path.exists():
        parser.error("Output/report already exists; preserve the previous attempt")
    source = packets(args.ffprobe, args.source)
    # Native MediaRecorder evidence uses a millisecond clock. Reject higher
    # precision instead of silently rounding or forcing a constant frame rate.
    pts = [round(float(p["pts_time"]) * 1000) for p in source]
    if any(abs(float(p["pts_time"]) - t / 1000) > .0000001
           for p, t in zip(source, pts)):
        parser.error("Source PTS cannot be represented on a millisecond clock")
    if not pts or any(b <= a for a, b in zip(pts, pts[1:])):
        parser.error("Source requires strictly increasing presentation order")
    tail = float(source[-1]["duration_time"])
    last_ms = round(tail * 1000)
    if last_ms <= 0 or abs(tail - last_ms / 1000) > .0000001:
        parser.error("Source needs an explicit positive millisecond ending hold")
    subprocess.run([
        str(args.ffmpeg), "-hide_banner", "-loglevel", "error", "-copyts",
        "-i", str(args.source), "-map", "0:v:0", "-c:v", "libx264",
        "-qp", "0", "-preset", "fast", "-bf", "0", "-g", "1",
        "-fps_mode", "passthrough", "-enc_time_base", "1:1000",
        "-bsf:v", f"setts=duration=if(eq(N\\,{len(pts)-1})\\,{last_ms}\\,NEXT_PTS-PTS)",
        "-video_track_timescale", "1000", "-movflags", "+faststart", str(args.output)
    ], check=True)
    output = packets(args.ffprobe, args.output)
    actual = [float(p["pts_time"]) for p in output]
    mismatches = [i for i in range(len(output) - 1)
                  if abs(float(output[i]["duration_time"]) - (actual[i+1] - actual[i])) > .0000001]
    end = actual[-1] + float(output[-1]["duration_time"])
    expected_end = pts[-1] / 1000 + tail
    source_extent, output_extent = extent(args.ffprobe, args.source), extent(args.ffprobe, args.output)
    span = expected_end - pts[0] / 1000
    report = dict(source=str(args.source.resolve()), output=str(args.output.resolve()),
                  sha256=hashlib.sha256(args.output.read_bytes()).hexdigest(),
                  sourceFrames=len(source), outputFrames=len(output),
                  ptsMatch=actual == [t / 1000 for t in pts],
                  durationMismatches=mismatches, expectedEnd=expected_end, actualEnd=end,
                  sourceExtent=source_extent, outputExtent=output_extent,
                  pixelsMatch=pixels(args.ffmpeg, args.source) == pixels(args.ffmpeg, args.output))
    report["passed"] = (report["ptsMatch"] and not mismatches and report["pixelsMatch"]
                        and len(source) == len(output) and abs(end - expected_end) < .0000001
                        and abs(float(output_extent["streams"][0]["duration"]) - span) < .0000001
                        and abs(float(output_extent["format"]["duration"]) - span) < .0000001)
    report_path.write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(json.dumps(report))
    return 0 if report["passed"] else 1


if __name__ == "__main__":
    raise SystemExit(main())
