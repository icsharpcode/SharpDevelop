namespace DebuggerInterop.Core
{
    using System.Runtime.InteropServices;

    [ComImport, Guid("3D6F5F61-7538-11D3-8D5B-00104B35E7EF"), CoClass(typeof(EmbeddedCLRCorDebugClass))]
    public interface EmbeddedCLRCorDebug : ICorDebug
    {
    }
}

