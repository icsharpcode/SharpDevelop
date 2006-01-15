// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using SharpQuery.SchemaClass;

namespace SharpQuery.Collections
{
	[Serializable()]
	public class SharpQuerySchemaClassCollection : List<ISchemaClass>
	{
	}

	[Serializable()]
	public class SharpQueryListDictionary : Dictionary<string, SharpQuerySchemaClassCollection>
	{
	}

	[Serializable()]
	public class SharpQueryParameterCollection : List<SharpQueryParameter>
	{
	}

	[Serializable()]
	public class SharpQueryStringDictionary : Dictionary<string, string>
	{
	}
}
