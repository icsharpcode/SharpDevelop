// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms 
{
	public interface IPropertyValueCreator
	{
		bool CanCreateValueForType(Type propertyType);
		
		object CreateValue(Type propertyType, string valueString);
		
	}
}
