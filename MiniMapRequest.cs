using System;
using System.IO;
using System.Threading;

namespace Q3MinimapGenerator
{
    //struct so (default) one passes default values
    public struct MiniMapRequest
    {
        public string OutputFolderPath { get; init; } = null;
        public float PixelsPerUnit { get; init; } = 0.1f;
        public int MaxWidth { get; init; } = 4000;
        public int MaxHeight { get; init; } = 4000;
        public int ExtraBorderUnits { get; init; } = 10;
        public MiniMapAxisPlane AxisPlane { get; init; } = MiniMapAxisPlane.All;
        public bool MakeMeta { get; init; } = true;
        public Func<string, string, MiniMapMeta, string> ImageFilePathFormatter { get; init; }
        public Func<string, MiniMapMeta, string> MetaFilePathFormatter { get; init; }
        public Func<string, FileInfo[], bool> Predicate { get; init; }
        public Action<float> ProgressCallback { get; init; }
        public CancellationToken? CancellationToken { get; init; }
        public MiniMapRequest() {}
    }

    [Flags]
    public enum MiniMapAxisPlane
    {
        XY = 1 << 0,
        YZ = 1 << 1,
        XZ = 1 << 2,
        All = XY | YZ | XZ
    }
}

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

