// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Boo.Lang.Compiler;

namespace NRefactoryToBooConverter
{
	public sealed class ConverterSettings
	{
		CompilerErrorCollection errors;
		CompilerWarningCollection warnings;
		string fileName;
		StringComparer nameComparer;
		
		public const string DefaultNameGenerationPrefix = "converterGeneratedName";
		public string NameGenerationPrefix = DefaultNameGenerationPrefix;
		public bool SimplifyTypeNames = true;
		public bool RemoveRedundantTypeReferences = true;
		
		static StringComparer GetComparer(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (string.Equals(System.IO.Path.GetExtension(fileName), ".vb", StringComparison.OrdinalIgnoreCase))
				return StringComparer.OrdinalIgnoreCase;
			else
				return StringComparer.Ordinal;
		}
		
		public bool IsVisualBasic {
			get {
				return string.Equals(System.IO.Path.GetExtension(fileName), ".vb", StringComparison.OrdinalIgnoreCase);
			}
		}
		
		public ConverterSettings(string fileName)
			: this(fileName, GetComparer(fileName))
		{
		}
		
		public ConverterSettings(string fileName, StringComparer nameComparer)
			: this(fileName, nameComparer, new CompilerErrorCollection(), new CompilerWarningCollection())
		{
		}
		
		public ConverterSettings(string fileName, CompilerErrorCollection errors, CompilerWarningCollection warnings)
			: this(fileName, GetComparer(fileName), errors, warnings)
		{
		}
		
		public ConverterSettings(string fileName, StringComparer nameComparer, CompilerErrorCollection errors, CompilerWarningCollection warnings)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (nameComparer == null)
				throw new ArgumentNullException("nameComparer");
			if (errors == null)
				throw new ArgumentNullException("errors");
			if (warnings == null)
				throw new ArgumentNullException("warnings");
			this.errors = errors;
			this.warnings = warnings;
			this.fileName = fileName;
			this.nameComparer = nameComparer;
		}
		
		public CompilerErrorCollection Errors {
			get {
				return errors;
			}
		}
		
		public CompilerWarningCollection Warnings {
			get {
				return warnings;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public StringComparer NameComparer {
			get {
				return nameComparer;
			}
		}
	}
}
