// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;

/// <summary>
/// This Class is used for Databased items
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 22.08.2005 00:12:59
/// </remarks>
namespace ICSharpCode.Reports.Core
{
	public interface IDataItem
	{
		string ColumnName { get; set; }
		string MappingName { get; }
		string BaseTableName { get; set; }
		string DBValue {get;set;}
		string Name {get;set;}
		string DataType {get;set;}
	}
}
