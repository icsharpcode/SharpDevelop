// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CheckFileHeaders
{
	class MainClass
	{
		const string copyrightHeaderOld = @"// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)";
		const string licenseHeaderLGPL = @"// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)";
		
		const string copyrightHeader = @"// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team";
		const string licenseHeaderMIT = @"// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the ""Software""), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.";
		
		string currentLicense = licenseHeaderMIT;
		
		static int Main(string[] args)
		{
			try {
				MainClass m = new MainClass();
				
				int count;
				if (args.Length == 0) {
					Environment.CurrentDirectory = Path.GetFullPath(@"..\..\..\..\..");
					Console.WriteLine("Checking files in {0}  ...", Path.GetFullPath("src"));
					count = m.Run("src");
				} else {
					Environment.CurrentDirectory = Path.GetFullPath(args[0]);
					Console.WriteLine("Checking files in {0}  ...", Path.GetFullPath("."));
					count = m.Run(".");
				}
				Console.WriteLine("Finished! (checked {0} files, changed {1} files, ignored {2} files)", count, m.changeCount, m.ignoreCount);
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
			Console.Write("Press any key to continue...");
			Console.ReadKey(true);
			return 0;
		}
		int Run(string dir)
		{
			string oldLicense = currentLicense;
			if (File.Exists(Path.Combine(dir, "license.txt"))) {
				if (File.ReadAllText(Path.Combine(dir, "license.txt")).Contains("Redistributions  of  source code must retain the above copyright notice")) {
					//currentLicense = @"// This code is distributed under the BSD license (for details please see \" + dir + @"\license.txt)";
					// move from BSD to MIT
				}
			}
			int count = 0;
			foreach (string file in Directory.GetFiles(dir, "*.cs")) {
				count++;
				ProcessFile(file);
			}
			foreach (string subdir in Directory.GetDirectories(dir)) {
				if (subdir.EndsWith("\\.svn") || subdir.EndsWith("\\obj"))
					continue;
				if (subdir.EndsWith("Libraries\\NRefactory"))
					continue;
				if (subdir.EndsWith("Libraries\\AvalonDock"))
					continue;
				if (subdir.EndsWith("Libraries\\log4net"))
					continue;
				if (subdir.EndsWith("Libraries\\NUnit.Framework"))
					continue;
				if (subdir.EndsWith("Libraries\\IQToolkit"))
					continue;
				if (subdir.EndsWith("Libraries\\TreeViewAdv"))
					continue;
				if (subdir.EndsWith("Libraries\\cecil"))
					continue;
				if (subdir.EndsWith("CSharpBinding\\Project\\Resources"))
					continue;
				count += Run(subdir);
			}
			currentLicense = oldLicense;
			return count;
		}
		
		int changeCount, ignoreCount;
		
		void ProcessFile(string file)
		{
			string content = GetFileContent(file);
			int lastLine;
			int headerType = AnalyzeHeader(content, out lastLine);
			if (headerType == 8 || headerType == 9)
				return;
			if (headerType != 7)
				return;
			if (headerType == 4) {
				Console.WriteLine("unknown file: " + file);
				ignoreCount++;
				return;
			}
			if (headerType == 5) {
				Console.WriteLine("commented out file: " + file);
				ignoreCount++;
				return;
			}
			if (headerType == 6) {
				Console.WriteLine("file with explicit license: " + file);
				ignoreCount++;
				return;
			}
			
			StringBuilder builder = new StringBuilder();
			builder.AppendLine(copyrightHeader);
			builder.AppendLine(currentLicense);
			builder.AppendLine();
			
			int offset = FindLineOffset(content, lastLine + 1);
			builder.Append(content.Substring(offset).Trim());
			builder.AppendLine();
			string newContent = builder.ToString();
			if (newContent.TrimEnd() != content.TrimEnd()) {
				using (StreamWriter w = new StreamWriter(file, false, GetOptimalEncoding(newContent))) {
					changeCount++;
					w.Write(newContent);
				}
			}
		}
		
		Encoding GetOptimalEncoding(string content)
		{
			return Encoding.UTF8;
		}
		
		int FindLineOffset(string content, int lineNumber)
		{
			int num = 0;
			for (int i = 0; i < content.Length; i++) {
				if (num == lineNumber)
					return i;
				if (content[i] == '\n')
					num++;
			}
			throw new ApplicationException("Cannot find line " + lineNumber);
		}
		
		#region AnalyzeHeader
		Regex xmlRegex = new Regex(@"<owner name=""(\w[\w\s\-]*\w)"" email=""([\w\s@\.\-]*)""\s?/>");
		Regex xmlRegex2 = new Regex(@"<(author|owner) name=""(\w[\w\s\-]*\w)""\s?/>");
		Regex sdRegex = new Regex(@"\* User: (.*)");
		
		// Returns:
		// 0 = no header
		// 1 = XML header
		// 2 = SharpDevelop header
		// 3 = GPL header (unused)
		// 4 = unknown header
		// 5 = outcommented file
		// 6 = XML header followed by explicit license
		// 7 = LGPL header (copyrightOld + LGPL)
		// 8 = MIT header
		// 9 = auto-generated file
		int AnalyzeHeader(string content, out int lastLine)
		{
			string content2 = content;
			
			string author = null;
			string email = null;
			int lineNumber = -1;
			lastLine = -1;
			
			byte state = 0;
			// state:
			// 0 = start
			// 1 = parse XML header
			// 2 = parse SharpDevelop header
			// 3 = search end of GPL header
			// 4 = block comment start
			using (StringReader r = new StringReader(content)) {
				string line;
				while ((line = r.ReadLine()) != null) {
					lineNumber++;
					line = line.Trim();
					if (state == 0) {
						if (line.Length == 0)
							continue;
						if (line == copyrightHeaderOld || line == "//" + copyrightHeaderOld) {
							lastLine = 1;
							return 7;
						} else if (line == copyrightHeader) {
							return 8;
						} else if (line.StartsWith("// Generated by ") || line.StartsWith("#line  1 \"") || line.StartsWith("// $ANTLR")) {
							return 9;
						} else if (line.StartsWith("using ")) {
							lastLine = -1;
							return 0;
						} else if (line == "// <file>") {
							state = 1;
						} else if (line.StartsWith("////")) {
							lastLine = -1;
							return 5;
						} else if (line == "/*") {
							state = 4;
						} else if (line == "//------------------------------------------------------------------------------") {
							// TlbImp auto-generated style (preserve)
							lastLine = -1;
							return 9;
						} else if (line.StartsWith("//")) {
							// ignore
						} else {
							break;
						}
					} else if (state == 1) {
						if (line == "// </file>") {
							lastLine = lineNumber;
							return 1;
						} else if (xmlRegex.IsMatch(line)) {
							Match m = xmlRegex.Match(line);
							author = m.Groups[1].Value;
							email = m.Groups[2].Value;
						} else if (xmlRegex2.IsMatch(line)) {
							Match m = xmlRegex2.Match(line);
							author = m.Groups[2].Value;
							email = null;
						}
					} else if (state == 2) {
						if (line == "*/") {
							lastLine = lineNumber;
							return 2;
						} else if (sdRegex.IsMatch(line)) {
							Match m = sdRegex.Match(line);
							author = m.Groups[1].Value;
						}
					} else if (state == 3) {
						if (line.Length == 0)
							continue;
						if (!line.StartsWith("//")) {
							lastLine = lineNumber - 1;
							return 3;
						}
					} else if (state == 4) {
						if (line == "* Created by SharpDevelop.") {
							state = 2;
						} else if (line == "* Created by SharpDevelop") {
							state = 2;
						} else {
							break;
						}
					} else {
						throw new NotSupportedException();
					}
				}
			}
			lastLine = -1;
			return 4;
		}
		#endregion
		
		#region Reading files
		string GetFileContent(string file)
		{
			using (StreamReader r = OpenFile(file)) {
				return r.ReadToEnd();
			}
		}
		
		StreamReader OpenFile(string fileName)
		{
			bool autodetectEncoding = true;
			FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			if (autodetectEncoding && fs.Length > 3) {
				// the autodetection of StreamReader is not capable of detecting the difference
				// between ISO-8859-1 and UTF-8 without BOM.
				int firstByte = fs.ReadByte();
				int secondByte = fs.ReadByte();
				switch ((firstByte << 8) | secondByte) {
					case 0x0000: // either UTF-32 Big Endian or a binary file; use StreamReader
					case 0xfffe: // Unicode BOM (UTF-16 LE or UTF-32 LE)
					case 0xfeff: // UTF-16 BE BOM
					case 0xefbb: // start of UTF-8 BOM
						// StreamReader autodetection works
						fs.Position = 0;
						return new StreamReader(fs);
					default:
						return AutoDetect(fs, (byte)firstByte, (byte)secondByte);
				}
			} else {
				return new StreamReader(fs);
			}
		}
		
		StreamReader AutoDetect(FileStream fs, byte firstByte, byte secondByte)
		{
			int max = (int)Math.Min(fs.Length, 500000); // look at max. 500 KB
			const int ASCII = 0;
			const int Error = 1;
			const int UTF8  = 2;
			const int UTF8Sequence = 3;
			int state = ASCII;
			int sequenceLength = 0;
			byte b;
			for (int i = 0; i < max; i++) {
				if (i == 0) {
					b = firstByte;
				} else if (i == 1) {
					b = secondByte;
				} else {
					b = (byte)fs.ReadByte();
				}
				if (b < 0x80) {
					// normal ASCII character
					if (state == UTF8Sequence) {
						state = Error;
						break;
					}
				} else if (b < 0xc0) {
					// 10xxxxxx : continues UTF8 byte sequence
					if (state == UTF8Sequence) {
						--sequenceLength;
						if (sequenceLength < 0) {
							state = Error;
							break;
						} else if (sequenceLength == 0) {
							state = UTF8;
						}
					} else {
						state = Error;
						break;
					}
				} else if (b > 0xc2 && b < 0xf5) {
					// beginning of byte sequence
					if (state == UTF8 || state == ASCII) {
						state = UTF8Sequence;
						if (b < 0xe0) {
							sequenceLength = 1; // one more byte following
						} else if (b < 0xf0) {
							sequenceLength = 2; // two more bytes following
						} else {
							sequenceLength = 3; // three more bytes following
						}
					} else {
						state = Error;
						break;
					}
				} else {
					// 0xc0, 0xc1, 0xf5 to 0xff are invalid in UTF-8 (see RFC 3629)
					state = Error;
					break;
				}
			}
			fs.Position = 0;
			switch (state) {
				case Error:
					return new StreamReader(fs, Encoding.Default);
				default:
					return new StreamReader(fs);
			}
		}
		#endregion
	}
}
