// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;
using System;
using System.Globalization;
using System.Text;
using System.IO;

namespace ICSharpCode.Build.Tasks
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
