// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	public class UndoStep
	{
		public XElement Tree { get; set; }
		public XElement RowDefinitions { get; set; }
		public XElement ColumnDefinitions { get; set; }
		public IList<XElement> AdditionalProperties { get; set; }
		
		public static UndoStep CreateStep(XElement tree, XElement rows, XElement cols, IEnumerable<XElement> properties)
		{
			XElement rowCopy = new XElement(rows);
			XElement colCopy = new XElement(cols);
			XElement treeCopy = new XElement(tree);
			
			IList<XElement> propertiesCopy = properties.Select(item => new XElement(item)).ToList();
			
			return new UndoStep() {
				Tree = treeCopy,
				RowDefinitions = rowCopy,
				ColumnDefinitions = colCopy,
				AdditionalProperties = propertiesCopy
			};
		}
	}
}
