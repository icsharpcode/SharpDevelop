// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NRefactoryToBooConverter;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace StandaloneConverter
{
	class MainClass
	{
		/// <returns>
		/// 0 = conversion successful
		/// 1 = internal error
		/// 2 = invalid command line parameters
		/// 3 = no command line parameters
		/// 4 = conversion error
		/// </returns>
		public static int Main(string[] args)
		{
			Console.WriteLine("Standalone C# to Boo converter");
			Console.WriteLine("(C) Daniel Grunwald, 2005");
			Console.WriteLine();
			return new MainClass().Run(args);
		}
		
		bool overwriteFiles = false;
		
		public int Run(string[] args)
		{
			try {
				bool failed = false;
				string outputdir = null;
				List<string> input = new List<string>();
				for (int i = 0; i < args.Length; i++) {
					string arg = args[i];
					switch (arg.ToLower()) {
						case "--help":
						case "-?":
						case "/?":
							ShowHelp();
							return 0;
						case "-o":
						case "--output":
							outputdir = args[++i];
							break;
						case "--overwrite":
						case "--force":
						case "-f":
							overwriteFiles = true;
							break;
						case "--version":
							Console.WriteLine("Version " + typeof(MainClass).Assembly.GetName().Version.ToString());
							return 0;
						case "--keepredundanttypereferences":
							removeRedundantTypeReferences = false;
							break;
						case "--noprimitivenames":
							simplifyTypeNames = false;
							break;
						default:
							if (arg.StartsWith("-")) {
								Console.WriteLine("Invalid argument: " + arg);
								failed = true;
							} else {
								input.Add(arg);
							}
							break;
					}
				}
				if (failed)
					return 2;
				if (input.Count == 0) {
					Console.WriteLine("No input(s) specified.");
					Console.WriteLine("Use the parameter '--help' to display the possible command line arguments.");
					return 3;
				}
				if (Convert(input, outputdir)) {
					Console.WriteLine();
					Console.WriteLine("Conversion successful.");
					return 0;
				} else {
					Console.WriteLine();
					Console.WriteLine("Conversion failed.");
					return 4;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex);
				return 1;
			}
		}
		
		void ShowHelp()
		{
			Console.Write(Path.GetFileName(typeof(MainClass).Assembly.Location));
			Console.WriteLine(" [-o outputdirectory] [switches] input1 input2 ...");
			Console.WriteLine();
			Console.WriteLine("An input parameter can be either a C# or VB.NET file or a directory.");
			Console.WriteLine("When a directory is specified, all .cs and .vb files inside that");
			Console.WriteLine("directory are converted recursivly.");
			Console.WriteLine();
			Console.WriteLine(" --output (short: -o)");
			Console.WriteLine("    Specifies the output directory to store the generated files in.");
			Console.WriteLine("    When no output directory is specified, the generated files are stored");
			Console.WriteLine("    in the directory of the source files.");
			Console.WriteLine();
			Console.WriteLine("Switches:");
			Console.WriteLine(" --overwrite or --force (short: -f)");
			Console.WriteLine("    Overwrite existing .boo files.");
			Console.WriteLine(" --keepredundanttypereferences");
			Console.WriteLine("    Don't remove type declarations like 'as void' or in local variable declarations where the type is clearly inferred.");
			Console.WriteLine(" --noPrimitiveNames");
			Console.WriteLine("    Use the fully type names (System.Int32) instead of the short names (int).");
			Console.WriteLine(" --help (short: -? or /?)");
			Console.WriteLine("    Shows this help text");
		}
		
		bool simplifyTypeNames = true;
		bool removeRedundantTypeReferences = true;
		
		ConverterSettings ApplySettings(string fileName, CompilerErrorCollection errors, CompilerWarningCollection warnings)
		{
			ConverterSettings settings = new ConverterSettings(fileName, errors, warnings);
			settings.SimplifyTypeNames = simplifyTypeNames;
			settings.RemoveRedundantTypeReferences = removeRedundantTypeReferences;
			return settings;
		}
		
		bool Convert(List<string> inputs, string outputdir)
		{
			bool success = true;
			foreach (string input in inputs) {
				if (Directory.Exists(input)) {
					success &= ConvertDirectory(input, GetOutputName(outputdir, input));
				} else {
					success &= ConvertFile(input, GetOutputName(outputdir, input));
				}
			}
			return success;
		}
		
		string GetOutputName(string outputdir, string input)
		{
			return outputdir == null ? input : Path.Combine(outputdir, Path.GetFileName(input));
		}
		
		bool ConvertDirectory(string inputdir, string outputdir)
		{
			bool success = true;
			foreach (string file in Directory.GetFiles(inputdir, "*.cs")) {
				success &= ConvertFile(file, Path.Combine(outputdir, Path.GetFileName(file)));
			}
			foreach (string file in Directory.GetFiles(inputdir, "*.vb")) {
				success &= ConvertFile(file, Path.Combine(outputdir, Path.GetFileName(file)));
			}
			foreach (string dir in Directory.GetDirectories(inputdir)) {
				success &= ConvertDirectory(dir, Path.Combine(outputdir, Path.GetFileName(dir)));
			}
			return success;
		}
		
		bool ConvertFile(string input, string output)
		{
			try {
				return ConvertFileInternal(input, Path.ChangeExtension(output, ".boo"));
			} catch (Exception ex) {
				Console.WriteLine("Error converting " + input + ":");
				Console.WriteLine("  " + ex.Message);
				return false;
			}
		}
		
		bool ConvertFileInternal(string input, string output)
		{
			string directory = Path.GetDirectoryName(output);
			if (directory.Length > 0) {
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
			}
			bool isFailed = false;
			if (!overwriteFiles && File.Exists(output)) {
				FailFile(ref isFailed, input, "The output file '{0}' already exists.", output);
				return false;
			}
			try {
				CompilerErrorCollection errors = new CompilerErrorCollection();
				CompilerWarningCollection warnings = new CompilerWarningCollection();
				Module module;
				IList<ICSharpCode.NRefactory.Parser.ISpecial> specials;
				CompileUnit compileUnit = new CompileUnit();
				using (StreamReader r = OpenFile(input)) {
					module = Parser.ParseModule(compileUnit, r, ApplySettings(input, errors, warnings), out specials);
				}
				foreach (CompilerError error in errors) {
					FailFile(ref isFailed, input, error.ToString());
				}
				if (!isFailed && warnings.Count > 0) {
					Console.WriteLine(input + ":");
					foreach (CompilerWarning warning in warnings) {
						Console.WriteLine("  " + warning.ToString());
					}
				}
				using (StreamWriter w = new StreamWriter(output, false, Encoding.UTF8)) {
					foreach (CompilerError error in errors) {
						w.WriteLine("ERROR: " + error.ToString());
					}
					if (errors.Count > 0)
						w.WriteLine();
					foreach (CompilerWarning warning in warnings) {
						w.WriteLine("# WARNING: " + warning.ToString());
					}
					if (warnings.Count > 0)
						w.WriteLine();
					BooPrinterVisitorWithComments printer = new BooPrinterVisitorWithComments(specials, w);
					if (module != null) {
						printer.OnModule(module);
					}
					printer.Finish();
				}
			} catch (Exception ex) {
				FailFile(ref isFailed, input, ex);
			}
			return !isFailed;
		}
		
		void FailFile(ref bool isFailed, string input, Exception ex)
		{
			FailFile(ref isFailed, input, "Internal error:");
			CompilerError cerr = ex as CompilerError;
			if (cerr != null)
				Console.WriteLine(cerr.ToString(true));
			else
				Console.WriteLine(ex.ToString());
		}
		
		void FailFile(ref bool isFailed, string input, string message, params object[] args)
		{
			if (!isFailed) {
				isFailed = true;
				Console.WriteLine(input + " failed:");
			}
			if (args.Length == 0) {
				Console.WriteLine("  " + message);
			} else {
				Console.WriteLine("  " + message, args);
			}
		}
		
		#region Open file with encoding autodetection
		StreamReader OpenFile(string fileName)
		{
			FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			if (fs.Length > 3) {
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
				} else if (b >= 0xc2 && b < 0xf5) {
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
				case ASCII:
					// when the file seems to be ASCII, we read it using the user-specified encoding
					// so it is saved again using that encoding.
					return new StreamReader(fs, GetDefaultEncoding());
				case Error:
					return new StreamReader(fs, GetDefaultEncoding());
				default:
					return new StreamReader(fs);
			}
		}
		
		Encoding GetDefaultEncoding()
		{
			Encoding encoding = Encoding.Default;
			int codepage = encoding.CodePage;
			if (codepage == 65001 || codepage == 65000 || codepage == 1200 || codepage == 1201) {
				return Encoding.GetEncoding("ISO-8859-1");
			} else {
				return encoding;
			}
		}
		#endregion
	}
}
