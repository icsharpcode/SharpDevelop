#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

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
		
		static StringComparer GetComparer(string fileName)
		{
			if (System.IO.Path.GetExtension(fileName).ToLower() == ".vb")
				return StringComparer.InvariantCultureIgnoreCase;
			else
				return StringComparer.InvariantCulture;
		}
		
		public bool IsVisualBasic {
			get {
				return System.IO.Path.GetExtension(fileName).ToLower() == ".vb";
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
