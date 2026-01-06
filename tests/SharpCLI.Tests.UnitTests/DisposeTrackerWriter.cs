namespace SharpCLI.Tests.UnitTests;

// Helper class to track if a TextWriter has been disposed
internal class DisposeTrackerWriter : StringWriter
{
    public bool IsDisposed { get; private set; }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }
}