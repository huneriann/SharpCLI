namespace SharpCLI.Tests.UnitTests;

internal class DisposeTrackerWriter : StringWriter
{
    public bool IsDisposed { get; private set; }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }
}