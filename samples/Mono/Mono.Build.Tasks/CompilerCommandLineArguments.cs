// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Globalization;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;

namespace Mono.Build.Tasks
{
	public class CompilerCommandLineArguments : CommandLineBuilderExtension
	{			
		public CompilerCommandLineArguments()
		{
		}
		
		public static bool IsNetModule(string fileName)
		{
			return Path.GetExtension(fileName).ToLowerInvariant() == ".netmodule";
		}
		
		public void AppendFileNameIfNotNull(string switchName, ITaskItem fileItem)
		{
			if (fileItem != null) {
				AppendFileNameIfNotNull(switchName, fileItem.ItemSpec);
			}
		}
		
		public void AppendTarget(string targetType)
		{
			if (targetType != null) {
				AppendSwitch("-target:", targetType.ToLowerInvariant());
			}
		}
		
		public void AppendSwitchIfTrue(string switchName, bool parameter)
		{
			if (parameter) {
				AppendSwitch(switchName);
			}
		}
		
		public void AppendReferencesIfNotNull(ITaskItem[] references)
		{
			if (references == null) {
				return;
			}
			
			foreach (ITaskItem reference in references) {
				string fileName = reference.ItemSpec;
				if (CompilerCommandLineArguments.IsNetModule(fileName)) {
					AppendFileNameIfNotNull("-addmodule:", reference);
				} else { 	
					AppendFileNameIfNotNull("-r:", reference);
				}
			}
		}
		
		public void AppendItemsIfNotNull(string switchName, ITaskItem[] items)
		{
			if (items == null) {
				return;
			}
			
			foreach (ITaskItem item in items) {
				AppendFileNameIfNotNull(switchName, item);
			}
		}
		
		public void AppendSwitch(string switchName, string parameter)
		{
			AppendSwitchIfNotNull(switchName, parameter);
		}
		
		public void AppendFileNameIfNotNull(string switchName, string fileName)
		{
			if (fileName != null) {
				AppendSpaceIfNotEmpty();
				AppendTextUnquoted(switchName);
				AppendFileNameWithQuoting(fileName);
			}
		}
		
		/// <summary>
		/// Appends and lower cases the switch's value if it is not null.
		/// </summary>
		public void AppendLowerCaseSwitchIfNotNull(string switchName, string parameter)
		{
			if (parameter != null) {
				AppendSwitch(switchName, parameter.ToLower(CultureInfo.InvariantCulture));
			}
		}
	}
}
