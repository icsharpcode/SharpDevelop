// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms 
{
	/// <summary>
	/// This interface is used to create the objects which are given by name in 
	/// the xml definition.
	/// </summary>
	public interface IObjectCreator
	{
		/// <summary>
		/// Creates a new instance of object name. 
		/// </summary>
		object CreateObject(string name, XmlElement el);
		
		Type GetType(string name);
	}
}
