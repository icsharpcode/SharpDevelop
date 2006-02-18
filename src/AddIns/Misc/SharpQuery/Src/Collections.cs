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
    public class SharpQueryParameterCollection : List<SharpQueryParameter>{
    	
    	public SharpQueryParameterCollection () {
    	}
    	/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='.SharpQueryParameterCollection'/> based on another <see cref='.SharpQueryParameterCollection'/>.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///       A <see cref='.SharpQueryParameterCollection'/> from which the contents are copied
		/// </param>
		public SharpQueryParameterCollection(SharpQueryParameterCollection value) {
			this.AddRange(value);
		}
    	
    	/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='.SharpQueryParameterCollection'/> containing any array of <see cref='.SharpQueryParameter'/> objects.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///       A array of <see cref='.SharpQueryParameter'/> objects with which to intialize the collection
		/// </param>
		public SharpQueryParameterCollection(SharpQueryParameter[] value) {
			this.AddRange(value);
		}
		
		
    	public SharpQuerySchemaClassCollection ToBaseSchemaCollection(){
    		SharpQuerySchemaClassCollection returnValues = new SharpQuerySchemaClassCollection();
    		foreach( SharpQueryParameter par in this ){
    			returnValues.Add( par );
    		}
    		return returnValues;
    	}
    }

    [Serializable()]
    public class SharpQueryStringDictionary : Dictionary<string, string>
    {
    }
}
