// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using Reflector.IpcServer;

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
			
			
			CodeElementInfo element = new CodeElementInfo();
			
			// Try to find the assembly which contains the resolved type
			IProjectContent pc = c.ProjectContent;
			ReflectionProjectContent rpc = pc as ReflectionProjectContent;
			if (rpc != null) {
				element.AssemblyLocation = rpc.AssemblyLocation;
			} else {
				IProject project = pc.Project as IProject;
				if (project != null) {
					element.AssemblyLocation = project.OutputAssemblyFullPath;
				}
			}
			
			if (String.IsNullOrEmpty(element.AssemblyLocation)) {
				MessageService.ShowWarning("Reflector AddIn: Could not determine the assembly location for " + c.ToString() + ".");
				return;
			}
			
			
			SetNamespaceAndTypeNames(element.Type, c);
			
			if (m != null) {
				element.MemberName = m.Name;
				element.MemberReturnType = ConvertType(m.ReturnType);
				
				IMethod method = m as IMethod;
				if (method != null) {
					if (method.IsConstructor) {
						element.MemberType = MemberType.Constructor;
						element.MemberName = method.IsStatic ? ".cctor" : ".ctor";
					} else {
						element.MemberType = MemberType.Method;
					}
					element.MemberTypeArgumentCount = method.TypeParameters.Count;
					element.MemberParameters = ConvertParameters(method.Parameters);
				} else {
					
					IField field = m as IField;
					if (field != null && !field.IsLocalVariable) {
						element.MemberType = MemberType.Field;
					} else {
						
						IProperty property = m as IProperty;
						if (property != null) {
							element.MemberType = MemberType.Property;
							element.MemberParameters = ConvertParameters(property.Parameters);
							if (property.IsIndexer) {
								// TODO: Support indexers with different names using DefaultMemberAttribute
								element.MemberName = "Item";
							}
						} else {
							
							IEvent e = m as IEvent;
							if (e != null) {
								// Currently I consider events to be uniquely identified by name and containing type.
								element.MemberType = MemberType.Event;
							}
							
						}
						
					}
					
				}
			}
			
			
			try {
				
				LoggingService.Debug("ReflectorAddIn: Trying to go to " + element.ToString());
				
				Application.DoEvents();
				Cursor.Current = Cursors.WaitCursor;
				
				ReflectorController.TryGoTo(element, WorkbenchSingleton.MainForm);
				
			} finally {
				Cursor.Current = Cursors.Default;
			}
		}
		
		static void SetNamespaceAndTypeNames(CodeTypeInfo t, IClass c)
		{
			Stack<IClass> outerClasses = new Stack<IClass>();
			do {
				outerClasses.Push(c);
			} while ((c = c.DeclaringType) != null);
			
			t.Namespace = outerClasses.Peek().Namespace;
			
			List<string> types = new List<string>();
			List<int> typeArgumentCount = new List<int>();
			while (outerClasses.Count > 0) {
				c = outerClasses.Pop();
				types.Add(c.Name);
				typeArgumentCount.Add(c.TypeParameters.Count);
			}
			t.TypeNames = types.ToArray();
			t.TypeArgumentCount = typeArgumentCount.ToArray();
		}
		
		static CodeTypeInfo ConvertType(IReturnType rt)
		{
			if (rt == null) return null;
			
			if (rt.IsArrayReturnType) {
				ArrayReturnType art = rt.CastToArrayReturnType();
				CodeTypeInfo cti = new CodeTypeInfo();
				cti.ArrayElementType = ConvertType(art.ArrayElementType);
				cti.ArrayDimensions = art.ArrayDimensions;
				return cti;
			}
			
			IClass c = rt.GetUnderlyingClass();
			if (c != null) {
				CodeTypeInfo cti = new CodeTypeInfo();
				SetNamespaceAndTypeNames(cti, c);
				return cti;
			}
			
			return null;
		}
		
		static CodeTypeInfo[] ConvertParameters(IList<IParameter> parameters)
		{
			CodeTypeInfo[] cti = new CodeTypeInfo[parameters.Count];
			for (int i = 0; i < parameters.Count; ++i) {
				cti[i] = ConvertType(parameters[i].ReturnType);
			}
			return cti;
		}
	}
}
