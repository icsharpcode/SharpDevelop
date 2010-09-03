// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Static methods to help with designer operations which require access to internal Xaml elements.
	/// </summary>
	public static class XamlStaticTools
    {
        /// <summary>
        /// Gets the Xaml string of the <paramref name="xamlObject"/>
        /// </summary>
        /// <param name="xamlObject">The object whose Xaml is requested.</param>
        public static string GetXaml(XamlObject xamlObject)
        {
            if (xamlObject != null)
                return xamlObject.XmlElement.OuterXml;
            return null;
        }
    }
}
