// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CheckFileHeaders
{
	class MainClass
	{
		static int Main(string[] args)
		{
			int count;
			try {
				MainClass m = new MainClass();
				if (args.Length == 0) {
					Console.WriteLine("Checking files in {0}  ...", Path.GetFullPath(@"..\..\..\..\"));
					count = m.Run(@"..\..\..\..\");
				} else {
					Console.WriteLine("Checking files in {0}  ...", Path.GetFullPath(args[0]));
					count = m.Run(args[0]);
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
			int count = 0;
			foreach (string file in Directory.GetFiles(dir, "*.cs")) {
				if (file.EndsWith(".Designer.cs"))
					continue;
				count++;
				ProcessFile(file);
			}
			foreach (string subdir in Directory.GetDirectories(dir)) {
				if (subdir.EndsWith("\\.svn"))
					continue;
				if (subdir.EndsWith("Libraries\\DockPanel_Src"))
					continue;
				if (subdir.EndsWith("Libraries\\log4net"))
					continue;
				if (subdir.EndsWith("Libraries\\NUnit.Framework"))
					continue;
				if (Path.GetFullPath(subdir).EndsWith("src\\Tools"))
					continue;
				// Disabled addins:
				if (subdir.EndsWith("AddIns\\Misc\\Debugger\\TreeListView\\Project"))
					continue;
				if (subdir.EndsWith("AddIns\\Misc\\SharpReport"))
					continue;
				if (subdir.EndsWith("AddIns\\Misc\\ComponentInspector"))
					continue;
				count += Run(subdir);
			}
			return count;
		}
		
		// must be splitted because this file is under version control, too
		Regex resetVersionRegex = new Regex(@"//     <version>\$Revi" + @"sion: \d+ \$</version>", RegexOptions.Compiled);
		
		int changeCount, ignoreCount;
		
		void ProcessFile(string file)
		{
			string content = GetFileContent(file);
			string author, email;
			int lastLine;
			int headerType = AnalyzeHeader(content, out author, out email, out lastLine);
			if (headerType == 5) {
				//Console.WriteLine("unknown file: " + file);
				ignoreCount++;
				return;
			}
			if (author == null)
				author = "";
			if (author == "") {
				Console.Write(file);
				char ch;
				do {
					Console.WriteLine();
					Console.Write("  Daniel/Matt/Other/None/Ignore (D/M/O/N/I): ");
				}
				while ((ch = char.ToUpper(Console.ReadKey().KeyChar)) != 'M'
				       && ch != 'N' && ch != 'I' && ch != 'O' && ch != 'D');
				Console.WriteLine();
				if (ch == 'M') {
					author = "Matthew Ward";
				} else if (ch == 'D') {
					author = "Daniel Grunwald";
				} else if (ch == 'O') {
					bool ok;
					do {
						Console.Write("Enter author name: ");
						author = Console.ReadLine();
						if (author == "David") author = "David Srbecky";
						if (author == "Markus") author = "Markus Palme";
						if (author == "Peter") author = "Peter Forstmeier";
						email = CheckAuthor(ref author);
						ok = author != null;
					} while (!ok);
				} else if (ch == 'I') {
					ignoreCount++;
					return;
				} else {
					author = "none";
				}
			}
			string oldAuthor = author;
			email = CheckAuthor(ref author);
			if (author == null) {
				Console.WriteLine("Unknown author: " + oldAuthor + " in " + file);
				Console.WriteLine("    File was ignored.");
				return;
			}
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("// <file>");
			builder.AppendLine("//     <copyright see=\"prj:///doc/copyright.txt\"/>");
			builder.AppendLine("//     <license see=\"prj:///doc/license.txt\"/>");
			builder.Append("//     <owner name=\"");
			builder.Append(author);
			builder.Append("\" email=\"");
			builder.Append(email);
			builder.AppendLine("\"/>");
			
			// must be splitted because this file is under version control, too
			const string versionLine = "//     <version>$Revi" + "sion$</version>";
			builder.AppendLine(versionLine);
			builder.AppendLine("// </file>");
			builder.AppendLine();
			int offset = FindLineOffset(content, lastLine + 1);
			builder.Append(content.Substring(offset).Trim());
			builder.AppendLine();
			string newContent = builder.ToString();
			string resettedVersion = resetVersionRegex.Replace(content, versionLine);
			if (newContent != resettedVersion) {
				using (StreamWriter w = new StreamWriter(file, false, GetOptimalEncoding(newContent))) {
					changeCount++;
					w.Write(newContent);
				}
			}
		}
		
		string CheckAuthor(ref string author)
		{
			switch (author) {
				case "Mike Krüger":
					author = "Mike Krüger";
					return "mike@icsharpcode.net";
				case "Daniel Grunwald":
					return "daniel@danielgrunwald.de";
				case "David Srbecký":
				case "David Srbecky":
					author = "David Srbecký";
					return "dsrbecky@gmail.com";
				case "Andrea Paatz":
					author = "Andrea Paatz";
					return "andrea@icsharpcode.net";
				case "Matthew Ward":
					return "mrward@users.sourceforge.net";
				case "Mathias Simmack":
					return "mathias@simmack.de";
				case "Poul Staugaard":
					return "poul@staugaard.dk";
				case "Roman Taranchenko":
					return "rnt@smtp.ru";
				case "Markus Palme":
					return "MarkusPalme@gmx.de";
				case "David McCloskey":
					return "dave_a_mccloskey@hotmail.com";
				case "Shinsaku Nakagawa":
					return "shinsaku@users.sourceforge.jp";
				case "Denis ERCHOFF":
					return "d_erchoff@hotmail.com";
				case "Georg Brandl":
					return "g.brandl@gmx.net";
				case "Ivo Kovacka":
					return "ivok@internet.sk";
				case "Scott Ferrett":
					return "surf@softvelocity.com";
				case "David Alpert":
					return "david@spinthemoose.com";
				case "Luc Morin":
					return "";
				case "Peter Forstmeier":
				case "Forstmeier Peter":
					author = "Peter Forstmeier";
					return "peter.forstmeier@t-online.de";
				case "John Simons":
					return "johnsimons007@yahoo.com.au";
				case "Dickon Field":
					return "";
				case "Robert Zaunere":
					return "";
				case "Christian Hornung":
					return "";
				case "none":
					return "";
				default:
					author = null;
					return null;
			}
		}
		
		Encoding GetOptimalEncoding(string content)
		{
			foreach (char ch in content) {
				if ((int)ch >= 128)
					return Encoding.UTF8;
			}
			return Encoding.ASCII;
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
		Regex xmlRegex = new Regex(@"<owner name=""(\w[\w\s]*\w)"" email=""([\w\s@\.\-]*)""\s?/>");
		Regex sdRegex = new Regex(@"\* User: (.*)");
		
		// Returns:
		// 0 = no header
		// 1 = XML header
		// 2 = SharpDevelop header
		// 3 = GPL header (unused)
		// 4 = unknown header
		// 5 = outcommented file
		int AnalyzeHeader(string content, out string author, out string email, out int lastLine)
		{
			string content2 = content;
			
			author = null;
			email = null;
			int lineNumber = -1;
			
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
						if (line.StartsWith("using ")) {
							lastLine = -1;
							return 0;
						} else if (line == "// <file>") {
							state = 1;
						} else if (line.StartsWith("////") || line.StartsWith("#line  1 \"")) {
							lastLine = -1;
							return 5;
						} else if (line == "/*") {
							state = 4;
						} else if (line == "//------------------------------------------------------------------------------") {
							// TlbImp auto-generated style (preserve)
							lastLine = -1;
							return 5;
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
