This directory contains the "glue code" needed for interacting with .NET Reflector.

Reflector.IpcServer.AddIn:
AddIn for Reflector which provides the ReflectorService via .NET remoting IPC channel.
This addin directly references Reflector.exe.

Reflector.IpcServer:
Contains the Reflector-independent common code that is shared between
Reflector.IpcServer.AddIn and the Reflector addin for SharpDevelop.


Note that the distribution does not include Reflector.exe.
If you want to build those libraries, you must place a copy of Reflector.exe in the src\RequiredLibraries directory.
