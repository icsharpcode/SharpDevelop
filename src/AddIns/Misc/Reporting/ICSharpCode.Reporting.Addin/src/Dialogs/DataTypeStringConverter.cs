/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.04.2014
 * Time: 20:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using ICSharpCode.Reporting.Addin.Globals;

namespace ICSharpCode.Reporting.Addin.Dialogs
{
	/// <summary>
	/// Description of DataTypeStringConverter.
	/// </summary>
	class DataTypeStringConverter:StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context){
			//true means show a combobox
			return true;
		}
		
		
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context){
			//true will limit to list. false will show the list, but allow free-form entry
				return true;
		}

		
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context){
			return new StandardValuesCollection(GlobalLists.DataTypeList());
		}
	}
}
