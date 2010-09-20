// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms 
{
	/// <summary>
	/// Default implementation of the IObjectCreator interface.
	/// </summary>
	public class DefaultObjectCreator : IObjectCreator
	{
		public virtual Type GetType(string name)
		{
			Type t = typeof(Control).Assembly.GetType(name);
			
			// try to create System.Drawing.* objects
			if (t == null) {
				t = typeof(Point).Assembly.GetType(name);
			}
			
			// try to create System.* objects
			if (t == null) {
				t = typeof(String).Assembly.GetType(name);
			}
			
			// try to create the object from some assembly which is currently
			// loaded by the running application.
			if (t == null) {
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				
				foreach (Assembly assembly in assemblies) {
					t = assembly.GetType(name);
					if (t != null) {
						break;
					}
				}
			}
			
			return t;
		}
		public virtual object CreateObject(string name, XmlElement el)
		{
			try {
				// try to create System.Windows.Forms.* objects
				object newObject = typeof(Control).Assembly.CreateInstance(name);
				
				// try to create System.Drawing.* objects
				if (newObject == null) {
					newObject = typeof(Point).Assembly.CreateInstance(name);
				}
				
				// try to create System.* objects
				if (newObject == null) {
					newObject = typeof(String).Assembly.CreateInstance(name);
				}
				
				// try to create the object from some assembly which is currently
				// loaded by the running application.
				if (newObject == null) {
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					
					foreach (Assembly assembly in assemblies) {
						newObject = assembly.CreateInstance(name);
						if (newObject != null) {
							break;
						}
					}
				}
				
				if (newObject is Control) {
					((Control)newObject).SuspendLayout();
				}
				
				return newObject;			
			} catch (Exception) {
				return null;
			}
		}
	}
}
