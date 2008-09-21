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
								element.MemberName = GetRealIndexerPropertyName(c, property);
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
		
		static string GetRealIndexerPropertyName(IClass c, IProperty property)
		{
			// Support indexers with different names using IndexerNameAttribute or DefaultMemberAttribute
			
			string name = GetSingleAttributeWithStringParameter(c, property.Attributes, "System.Runtime.CompilerServices.IndexerNameAttribute");
			if (name != null) {
				LoggingService.Debug("Reflector AddIn: IndexerNameAttribute on property found: '" + name + "'");
				return name;
			}
			
			name = GetSingleAttributeWithStringParameter(c, c.Attributes, "System.Reflection.DefaultMemberAttribute");
			if (name != null) {
				LoggingService.Debug("Reflector AddIn: DefaultMemberAttribute on class with indexer found: '" + name + "'");
				return name;
			} else {
				if (property.Name == "Indexer") {
					LoggingService.Debug("Reflector AddIn: Neither DefaultMemberAttribute nor IndexerNameAttribute found, assuming default name");
					return "Item";
				} else {
					LoggingService.Debug("Reflector AddIn: Neither DefaultMemberAttribute nor IndexerNameAttribute found, but the property has name '" + property.Name + "', using that");
					return property.Name;
				}
			}
		}
		
		static string GetSingleAttributeWithStringParameter(IClass c, IEnumerable<IAttribute> attributeList, string attributeTypeName)
		{
			IReturnType attributeType = GetDomType(attributeTypeName, c.ProjectContent);
			if (attributeType == null) {
				return null;
			}
			var attributes = attributeList.Where(att => attributeType.Equals(att.AttributeType) && att.PositionalArguments.Count == 1 && att.PositionalArguments[0] is string);
			if (attributes.Count() == 1) {
				return (string)attributes.Single().PositionalArguments[0];
			}
			return null;
		}
		
		static IReturnType GetDomType(string className, IProjectContent pc)
		{
			IClass c = pc.GetClass(className, 0);
			if (c == null) {
				LoggingService.Warn("Reflector AddIn: Could not find the class for '" + className + "'");
				return null;
			}
			return c.DefaultReturnType;
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
