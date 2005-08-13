// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractSecondaryViewContent : AbstractBaseViewContent, ISecondaryViewContent
	{
		public virtual void NotifyBeforeSave()
		{
		}
		
		public virtual void NotifyAfterSave(bool successful)
		{
		}
	}
}
