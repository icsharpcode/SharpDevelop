// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
