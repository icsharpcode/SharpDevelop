// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This class defines the SharpDevelop display binding interface, it is a factory
	/// structure, which creates IViewContents.
	/// </summary>
	public interface ISecondaryDisplayBinding
	{
		bool CanAttachTo(IViewContent content);
		
		ISecondaryViewContent [] CreateSecondaryViewContent(IViewContent viewContent);
	}
}
