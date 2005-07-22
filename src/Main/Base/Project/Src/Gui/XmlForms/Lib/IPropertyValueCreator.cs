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
	public interface IPropertyValueCreator
	{
		bool CanCreateValueForType(Type propertyType);
		
		object CreateValue(Type propertyType, string valueString);
		
	}
}
