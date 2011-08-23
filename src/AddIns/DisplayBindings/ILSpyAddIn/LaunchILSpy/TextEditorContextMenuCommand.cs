// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// Implements a menu command to position .NET ILSpy on a class
	/// or class member.
	/// </summary>
	public sealed class TextEditorContextMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IClass c;
			IMember m;
			
			MemberNode mn = this.Owner as MemberNode;
			if (mn != null) {
				m = mn.Member;
				c = m.DeclaringType;
			} else {
				ClassNode cn = this.Owner as ClassNode;
				if (cn != null) {
					c = cn.Class;
					m = null;
				} else {
					ClassMemberBookmark cmbm = this.Owner as ClassMemberBookmark;
					if (cmbm != null) {
						m = cmbm.Member;
						c = m.DeclaringType;
					} else {
						ClassBookmark cbm = this.Owner as ClassBookmark;
						if (cbm != null) {
							c = cbm.Class;
							m = null;
						} else {
							MessageService.ShowWarning("ILSpy AddIn: Could not determine the class for the selected element. Owner: " + ((this.Owner == null) ? "<null>" : this.Owner.ToString()));
							return;
						}
					}
				}
			}
			
			if (c == null) {
				MessageService.ShowWarning("ILSpy AddIn: Could not determine the class for the selected element (known owner). Owner: " + this.Owner.ToString());
				return;
			}
			
			AbstractEntity entity = m as AbstractEntity;
			if (entity == null)
				entity = c as AbstractEntity;
			if (entity != null) {
				ILSpyController.TryGoTo(entity);
			}
		}
	}
}
