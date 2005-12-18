// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
			if (System.IO.Path.GetExtension(fileName).ToLowerInvariant() == ".vb")
				return StringComparer.InvariantCultureIgnoreCase;
			else
				return StringComparer.InvariantCulture;
		}
		
		public bool IsVisualBasic {
			get {
				return System.IO.Path.GetExtension(fileName).ToLowerInvariant() == ".vb";
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
