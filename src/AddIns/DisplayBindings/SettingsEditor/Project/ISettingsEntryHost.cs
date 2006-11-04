// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SettingsEditor
{
	public interface ISettingsEntryHost
	{
		string GetDisplayNameForType(Type type);
		Type GetTypeByDisplayName(string displayName);
	}
	
	sealed class DummySettingsEntryHost : ISettingsEntryHost
	{
		public readonly static DummySettingsEntryHost Instance = new DummySettingsEntryHost();
		
		public string GetDisplayNameForType(Type type)
		{
			return type.AssemblyQualifiedName;
		}
		
		public Type GetTypeByDisplayName(string displayName)
		{
			return Type.GetType(displayName);
		}
	}
}
