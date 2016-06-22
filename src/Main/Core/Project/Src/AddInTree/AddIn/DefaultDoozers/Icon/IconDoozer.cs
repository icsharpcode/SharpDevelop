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
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates associations between file types or node types in the project browser and
	/// icons in the resource service.
	/// </summary>
	/// <attribute name="resource" use="required">
	/// The name of a bitmap resource in the resource service.
	/// </attribute>
	/// <attribute name="language">
	/// This attribute is specified when a project icon association should be created.
	/// It specifies the language of the project types that use the icon.
	/// </attribute>
	/// <attribute name="extensions">
	/// This attribute is specified when a file icon association should be created.
	/// It specifies the semicolon-separated list of file types that use the icon.
	/// </attribute>
	/// <usage>Only in /Workspace/Icons</usage>
	/// <returns>
	/// An IconDescriptor object that exposes the attributes.
	/// </returns>
	public class IconDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return new IconDescriptor(args.Codon);
		}
	}
}
