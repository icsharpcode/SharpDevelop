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
}
