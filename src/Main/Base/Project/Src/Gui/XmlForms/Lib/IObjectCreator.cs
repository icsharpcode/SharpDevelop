// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;

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
