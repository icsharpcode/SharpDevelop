//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.IO;
//using System.Collections;
//using System.Drawing;
//using System.Drawing.Design;
//using System.Reflection;
//using System.Windows.Forms;
//using System.Drawing.Printing;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Xml;
//using System.ComponentModel.Design.Serialization;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Project;
//using ICSharpCode.SharpDevelop.Internal.Undo;
//using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
//
//using ICSharpCode.Core;
//
//using ICSharpCode.FormDesigner.Services;
//using ICSharpCode.TextEditor;
//
//using System.CodeDom;
//using System.CodeDom.Compiler;
//
//using Microsoft.CSharp;
//using Microsoft.VisualBasic;
//
//using ICSharpCode.SharpDevelop.Gui.ErrorDialogs;
//using ICSharpCode.FormDesigner.Gui;
//
//using ICSharpCode.SharpDevelop.Gui.OptionPanels;
//
//namespace ICSharpCode.FormDesigner {
//	
//	public class ComponentChangedUndoAction : ICSharpCode.SharpDevelop.Internal.Undo.IUndoableOperation
//	{
//		IDesignerHost host;
//		
//		string componentName;
//		MemberDescriptor member;
//		bool   isCollection;
//		bool   isComponentCollection;
//		object oldValue = null;
//		object newValue = null;
//		
//		public ComponentChangedUndoAction(IDesignerHost host, ComponentChangedEventArgs ea)
//		{
//			this.host            = host;
//			IComponent component = ea.Component as IComponent;
//			if (component == null) {
//				return;
//			}
//			
//			this.member          = ea.Member;
//			this.componentName   = component.Site.Name;
//			
//			isCollection = ea.NewValue is IList;
//			
//			if (isCollection) {
//				IList oldCol = (IList)ea.OldValue;
//				IList newCol = (IList)ea.NewValue;
//				object[] newArray = new object[newCol.Count];
//				isComponentCollection = false;
//				if (newCol.Count > 0) {
//					isComponentCollection = newCol[0] is IComponent;
//				}
//				
//				if (oldCol != null) {
//					object[] oldArray = new object[oldCol.Count];
//					if (isComponentCollection) {
//						int idx = 0;
//						foreach (IComponent cmp in oldCol) {
//							oldArray[idx++] = cmp.Site.Name;
//						}
//					} else {
//						oldCol.CopyTo(oldArray, 0);
//					}
//					this.oldValue = oldArray;
//				}
//				if (isComponentCollection) {
//					int idx = 0;
//					foreach (IComponent cmp in newCol) {
//						newArray[idx++] = cmp.Site.Name;
//					}
//				} else {
//					newCol.CopyTo(newArray, 0);
//				}
//				
//				this.newValue = newArray;
//			} else {
//				this.oldValue        = ea.OldValue;
//				this.newValue        = ea.NewValue;
//			}
//		}
//		
//		public void Undo()
//		{
//			ComponentChangeService componentChangeService = (ComponentChangeService)host.GetService(typeof(System.ComponentModel.Design.IComponentChangeService));
//			
//			IComponent component = host.Container.Components[componentName];
//			componentChangeService.OnComponentChanging(component, member);
//			
//			
//			Type t = component.GetType();
//			
//			PropertyInfo pInfo = t.Get(member.Name);
//			if (isCollection) {
//				IList coll = (IList)pInfo.GetValue(component, null);
//				
//				if (isComponentCollection) {
//					int idx = 0;
//					foreach (string name in (object[])oldValue) {
//						try {
//							if (coll is Menu.MenuItemCollection) {
//								((Menu.MenuItemCollection)coll).Add(idx++, (MenuItem)host.Container.Components[name]);
//							} else {
//								coll.Add(host.Container.Components[name]);
//							}
//						} catch (Exception e) {
//							
//							MessageService.ShowError(e, "Can't add " + name + " to collection.");
//						}
//					}
//				} else {
//					foreach (object o in (object[])oldValue) {
//						coll.Add(o);
//					}
//				}
//			} else {
//				pInfo.SetValue(component, oldValue, null);
//			}
//			componentChangeService.OnComponentChanged(component, 
//			                                          member,
//			                                          newValue,
//			                                          oldValue);
//		}
//		
//		public void Redo()
//		{
//			ComponentChangeService componentChangeService = (ComponentChangeService)host.GetService(typeof(System.ComponentModel.Design.IComponentChangeService));
//			
//			IComponent component = host.Container.Components[componentName];
//			componentChangeService.OnComponentChanging(component, member);
//			Type t = component.GetType();
//			if (isCollection) {
//				IList coll = (IList)t.InvokeMember(member.Name,
//				               BindingFlags.Public           |
//				               BindingFlags.NonPublic        |
//				               BindingFlags.Instance         |
//				               BindingFlags.FlattenHierarchy |
//				               BindingFlags.GetProperty,
//				               null, 
//				               component, 
//				               null);
//				coll.Clear();
//				if (isComponentCollection) {
//					foreach (string name in (object[])newValue) {
//						coll.Add(host.Container.Components[name]);
//					}
//				} else {
//					foreach (object o in (object[])newValue) {
//						coll.Add(o);
//					}
//				}
//			} else {
//				t.InvokeMember(member.Name,
//				               BindingFlags.Public           |
//				               BindingFlags.NonPublic        |
//				               BindingFlags.Instance         |
//				               BindingFlags.FlattenHierarchy |
//				               BindingFlags.SetProperty,
//				               null, 
//				               component, 
//				               new object[] { newValue });
//			}
//			componentChangeService.OnComponentChanged(component, 
//			                                          member,
//			                                          oldValue,
//			                                          newValue);
//		}
//	}
//}
