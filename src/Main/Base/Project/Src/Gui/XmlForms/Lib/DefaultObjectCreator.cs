// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
	[Obsolete("XML Forms are obsolete")]
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
