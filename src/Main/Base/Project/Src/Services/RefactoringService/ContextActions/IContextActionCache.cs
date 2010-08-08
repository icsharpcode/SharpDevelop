// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Data shared by multiple Context actions. Stored in EditorContext.GetCached().
	/// </summary>
	public interface IContextActionCache
	{
		void Initialize(EditorContext context);
	}
}
