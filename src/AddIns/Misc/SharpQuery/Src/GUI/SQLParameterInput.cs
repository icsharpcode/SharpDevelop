// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using SharpQuery.Collections;
using SharpQuery.SchemaClass;

namespace SharpQuery.Gui.DataView
{
	public class SQLParameterInput : XmlForm
	{
		private DataGrid _dataGrid = null;

		public DataGrid dataGrid
		{
			get
			{
				if (this._dataGrid == null)
				{
					this._dataGrid = this.ControlDictionary["dataGrid"] as DataGrid;
				}
				return this._dataGrid;
			}
		}

		private void ResetClick(object sender, EventArgs e)
		{

		}

		protected void FillParameters(SharpQueryParameterCollection parameters)
		{
			SharpQueryParameter par = null;
			for (int i = 0; i < parameters.Count; i++)
			{
				par = parameters[i];
				if (par.Type == ParameterDirection.ReturnValue)
				{
					i--;
					parameters.Remove(par);
				}
			}
			this.dataGrid.CaptionVisible = true;
			this.dataGrid.DataSource = parameters;
			this.dataGrid.DataMember = null;
			this.dataGrid.AllowNavigation = false;
		}

		public SQLParameterInput()
		{
			SetupFromXmlResource(PropertyService.DataDirectory + @"\resources\dialogs\SharpQuery\SqlParametersInput.xfrm");
		}

		public SQLParameterInput(SharpQueryParameterCollection parameters)
			: this()
		{
			this.FillParameters(parameters);
		}

		protected override void SetupXmlLoader()
		{
			xmlLoader.StringValueFilter = new SharpDevelopStringValueFilter();
			xmlLoader.PropertyValueCreator = new SharpDevelopPropertyValueCreator();
		}
	}
}
