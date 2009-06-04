// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;

namespace ReflectorAddIn
{
	/// <summary>
	/// Implements a menu command to position .NET Reflector on a class
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
							MessageService.ShowWarning("Reflector AddIn: Could not determine the class for the selected element. Owner: " + ((this.Owner == null) ? "<null>" : this.Owner.ToString()));
							return;
						}
					}
				}
			}
			
			if (c == null) {
				MessageService.ShowWarning("Reflector AddIn: Could not determine the class for the selected element (known owner). Owner: " + this.Owner.ToString());
				return;
			}
			
			
			// Try to find the assembly which contains the resolved type
			IProjectContent pc = c.ProjectContent;
			ReflectionProjectContent rpc = pc as ReflectionProjectContent;
			string assemblyLocation = null;
			if (rpc != null) {
				assemblyLocation = rpc.AssemblyLocation;
			} else {
				IProject project = pc.Project as IProject;
				if (project != null) {
					assemblyLocation = project.OutputAssemblyFullPath;
				}
			}
			
			if (String.IsNullOrEmpty(assemblyLocation)) {
				MessageService.ShowWarning("Reflector AddIn: Could not determine the assembly location for " + c.ToString() + ".");
				return;
			}
			
			
			AbstractEntity entity = m as AbstractEntity;
			if (entity == null)
				entity = c as AbstractEntity;
			if (entity != null) {
				LoggingService.Debug("ReflectorAddIn: Trying to go to " + entity.DocumentationTag + " in " + assemblyLocation);
				ReflectorController.TryGoTo(assemblyLocation, entity);
			}
		}
	}
}
