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
using System.Collections.Generic;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// A wrapper for markup extensions if custom behavior of <see cref="System.Windows.Markup.MarkupExtension.ProvideValue"/> is needed in the designer.
	/// </summary>
	public abstract class MarkupExtensionWrapper
	{
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="xamlObject">The <see cref="XamlObject"/> object that represents the markup extension.</param>
		protected MarkupExtensionWrapper(XamlObject xamlObject)
		{
			if (xamlObject == null) {
				throw new ArgumentNullException("xamlObject");
			}
			
			XamlObject = xamlObject;
		}
		
		/// <summary>
		/// Gets the <see cref="XamlObject"/> object that represents the markup extension.
		/// </summary>
		public XamlObject XamlObject { get; private set; }
		
		/// <summary>
		/// Returns an object that should be used as the value of the target property in the designer. 
		/// </summary>
		/// <returns>An object that should be used as the value of the target property in the designer.</returns>
		public abstract object ProvideValue();
		
		static readonly Dictionary<Type, Type> s_MarkupExtensionWrappers = new Dictionary<Type, Type>();

		/// <summary>
		/// Registers a markup extension wrapper.
		/// </summary>
		/// <param name="markupExtensionType">The type of the markup extension.</param>
		/// <param name="markupExtensionWrapperType">The type of the markup extension wrapper.</param>
		public static void RegisterMarkupExtensionWrapper(Type markupExtensionType, Type markupExtensionWrapperType)
		{
			if (markupExtensionType == null) {
				throw new ArgumentNullException("markupExtensionType");
			}
			
			if (!markupExtensionType.IsSubclassOf(typeof(MarkupExtension))) {
				throw new ArgumentException("The specified type must derive from MarkupExtension.", "markupExtensionType");
			}
			
			if (markupExtensionWrapperType == null) {
				throw new ArgumentNullException("markupExtensionWrapperType");
			}
			
			if (!markupExtensionWrapperType.IsSubclassOf(typeof(MarkupExtensionWrapper))) {
				throw new ArgumentException("The specified type must derive from MarkupExtensionWrapper.", "markupExtensionWrapperType");
			}
			
			s_MarkupExtensionWrappers.Add(markupExtensionType, markupExtensionWrapperType);
		}
		
		internal static MarkupExtensionWrapper CreateWrapper(Type markupExtensionWrapperType, XamlObject xamlObject)
		{
			return Activator.CreateInstance(markupExtensionWrapperType, xamlObject) as MarkupExtensionWrapper;
		}
		
		internal static MarkupExtensionWrapper TryCreateWrapper(Type markupExtensionType, XamlObject xamlObject)
		{
			Type markupExtensionWrapperType;
			if (s_MarkupExtensionWrappers.TryGetValue(markupExtensionType, out markupExtensionWrapperType)) {
				return CreateWrapper(markupExtensionWrapperType, xamlObject);
			}
			
			return null;
		}
	}
}
