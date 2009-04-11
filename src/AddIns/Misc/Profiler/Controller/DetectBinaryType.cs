// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Detects whether an .exe will run as 32-bit or 64-bit process.
	/// </summary>
	static class DetectBinaryType
	{
		public static bool IsDotNetExecutable(string exeName)
		{
			try {
				using (FileStream f = new FileStream(exeName, FileMode.Open, FileAccess.Read)) {
					BinaryReader r = new BinaryReader(f);
					
					f.Position = 0x3c; // jump to position in MSDOS stub that specifies the PE header location
					int peHeaderStart = r.ReadInt32(); // jump to pe header
					f.Position = peHeaderStart;
					if (r.ReadInt32() != 0x00004550) {
						Debug.WriteLine(".NET detection failed: invalid PE signature");
						return false;
					}
					
					int optionalHeaderStart = peHeaderStart + 4 + 20;
					f.Position = optionalHeaderStart; // jump to 'Optional' Header
					
					int dotNetHeaderOffset;
					
					switch (r.ReadInt16()) {
						case 0x10b: // PE32 Header
							dotNetHeaderOffset = 208;
							break;
						case 0x20b: // PE32+ Header
							dotNetHeaderOffset = 224;
							break;
						default:
							Debug.WriteLine(".NET detection failed: invalid PE magic number");
							return false;
					}
					
					f.Position = peHeaderStart + 4 + 16;
					ushort sizeOfOptionalHeader = r.ReadUInt16();
					
					if (sizeOfOptionalHeader < dotNetHeaderOffset + 8) {
						Debug.WriteLine(".NET detection failed: optional header too short, this can't be a .NET image");
						return false;
					}
										
					f.Position = optionalHeaderStart + dotNetHeaderOffset;
					uint cliHeaderRVA = r.ReadUInt32();
					uint cliHeaderSize = r.ReadUInt32();
					
					if (cliHeaderRVA == 0 || cliHeaderSize == 0) {
						Debug.WriteLine(".NET detection failed: image has no CLI header");
						return false;
					}
					
					return true;
				}
			} catch (IOException ex) {
				Debug.WriteLine(".NET detection failed: " + ex.ToString());
				return false;
			} catch (ArgumentException ex) {
				Debug.WriteLine(".NET detection failed: " + ex.ToString());
				return false;
			}
		}
		
		public static bool RunsAs64Bit(string exeName)
		{
			if (!ExtendedRegistry.Is64BitWindows) {
				// this Windows installation is a 32-bit windows, so the app can never run as 64-bit.
				return false;
			}
			// running as 64-bit process or inside WOW6432: this is 64-bit Windows
			
			Debug.WriteLine("64-bit autodetection for " + exeName + "...");
			try {
				using (FileStream f = new FileStream(exeName, FileMode.Open, FileAccess.Read)) {
					BinaryReader r = new BinaryReader(f);
					f.Position = 0x3c; // jump to position in MSDOS stub that specifies the PE header location
					int peHeaderStart = r.ReadInt32(); // jump to pe header
					f.Position = peHeaderStart;
					if (r.ReadInt32() != 0x00004550) {
						Debug.WriteLine("64-bit detection failed: invalid PE signature");
						return false;
					}
					int optionalHeaderStart = peHeaderStart + 4 + 20;
					f.Position = optionalHeaderStart; // jump to 'Optional' Header
					switch (r.ReadInt16()) {
						case 0x10b:
							// 32bit, but might be AnyCPU
							break;
						case 0x20b:
							Debug.WriteLine("Detected 64-bit image using PE32+ magic number");
							return true;
						default:
							Debug.WriteLine("64-bit detection failed: invalid PE magic number");
							return false;
					}
					f.Position = peHeaderStart + 4 + 16;
					ushort sizeOfOptionalHeader = r.ReadUInt16();
					if (sizeOfOptionalHeader < 216) {
						Debug.WriteLine("64-bit detection failed: optional header too short, this can't be a .NET image");
						// and if it's not a .NET image, it can't be AnyCPU
						return false;
					}
					
					f.Position = peHeaderStart + 4 + 2;
					ushort numberOfSections = r.ReadUInt16();
					
					f.Position = optionalHeaderStart + 208;
					uint cliHeaderRVA = r.ReadUInt32();
					uint cliHeaderSize = r.ReadUInt32();
					
					if (cliHeaderRVA == 0 || cliHeaderSize == 0) {
						Debug.WriteLine("64-bit detection failed: image has no CLI header");
						return false;
					}
					
					// now parse the section headers to find the section containing the cliHeaderRVA
					for (int i = 0; i < numberOfSections; i++) {
						f.Position = optionalHeaderStart + sizeOfOptionalHeader + 40 * i + 8;
						uint virtualSize = r.ReadUInt32();
						uint virtualAddress = r.ReadUInt32();
						if (cliHeaderRVA >= virtualAddress && cliHeaderRVA < virtualAddress + virtualSize) {
							// we found the correct section!
							/* uint sizeOfRawData = */ r.ReadUInt32();
							uint pointerToRawData = r.ReadUInt32();
							
							uint cliHeaderStart = pointerToRawData + (cliHeaderRVA - virtualAddress);
							f.Position = cliHeaderStart + 16;
							int runtimeFlags = r.ReadInt32();
							if ((runtimeFlags & 2) == 2) {
								Debug.WriteLine("64-bit detection: detected 32BITREQUIRED flag");
								return false;
							} else {
								Debug.WriteLine("64-bit detection: detected AnyCPU file. Will run as 64-bit process");
								return true;
							}
						}
					}
					Debug.WriteLine("64-bit detection failed: could not find section containing CLI header");
					return false;
				}
			} catch (IOException ex) {
				Debug.WriteLine("64-bit detection failed: " + ex.ToString());
				return false;
			} catch (ArgumentException ex) {
				Debug.WriteLine("64-bit detection failed: " + ex.ToString());
				return false;
			}
		}
	}
}
