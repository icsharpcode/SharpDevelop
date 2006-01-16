/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 19.04.2005
 * Time: 11:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;


using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

using SharpReport.ReportItems;
 
 /// <summary>
 /// this Class contains all commands belonging to a ReportSection
 /// </summary>
 namespace SharpReportAddin.Commands {
 	
 	///<summary>
 	/// Show the ReportSettings in Property Grid
 	///</summary>
 	public class ShowSettings : AbstractSharpReportCommand{
 		public ShowSettings() {
 			
 		}
 		public override void Run() {
 			try {
 				base.View.ShowReportSettings();
 			} catch (Exception) {
 				throw;
 			}
 		}
 		
 	} // End ShowSettings
 	
 	/// <summary>
 	/// Show Preview
 	/// </summary>
 	public class RunPreview :AbstractSharpReportCommand {
 		
 		public RunPreview(){
 		}
 		
 		public override void Run(){
 			try {
 				base.View.OnPreviewClick ();
 			} catch (Exception) {
 				throw;
 			}
 		}
 	}
 	
 	
 	public class SectionVisible : AbstractSharpReportCommand
 	{
 		/// <summary>
 		/// Creates a new ContextCommands
 		/// </summary>
 		public  SectionVisible(){	
 		}
 		
 		public override bool IsEnabled {
			get {
				return false;
			}
		}
 		
 		/// <summary>
 		/// Starts the command
 		/// </summary>
 		public override void Run()
 		{
 			MessageBox.Show (this.ToString() + " not implemented");
 		}
 	}
 	
 	/// <summary>
 	/// Let user choose from a List of Functions
 	/// </summary>
 	public class InsertFunction : AbstractSharpReportCommand{
 		/// <summary>
 		/// Creates a new ContextCommands
 		/// </summary>
 		public  InsertFunction(){
 		}
 		
 		public override bool IsEnabled {
			get {
				return false;
			}
		}

 		public override void Run()
 		{
 			MessageBox.Show (this.ToString() + " not implemented");
 		}
 	}
 	
 	/*
 	/// <summary>
 	/// Show a list of all Available Fields
 	/// </summary>
 	public class ShowFieldsList : AbstractSharpReportCommand{
 		/// <summary>
 		/// Creates a new ContextCommands
 		/// </summary>
 		public  ShowFieldsList(){
 		}
 		
 		public override bool IsEnabled {
 			get {
 				return true;
 			}
 		}
 		
 		public override void Run(){
 			try {
 				base.View.ListAvailableFields();
 			} catch (Exception) {
 				throw;
 			}
 		}
 		
 		
 	}
 	*/
 }
