
// 
/// <remarks>
/// 	created by - Forstmeier Peter
///     Peter Forstmeier (Peter.Forstmeier@t-online.de)
/// 	created on - 12.06.2005 18:17:46
/// </remarks>

using System;
	/// <summary>
	/// This Delegate is used to format the output from TextBased Items
	/// </summary>
namespace SharpReportCore {
	
	public class FormatOutputEventArgs : System.EventArgs {
		
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		private string formatString = String.Empty;
		private string stringToFormat = String.Empty;
		private string nullValue = String.Empty;
		private string formatedString = String.Empty;
		
		public FormatOutputEventArgs() {
		}
		
		public FormatOutputEventArgs(string stringToFormat,string formatString, string nullValue )
		{
			this.formatString = formatString;
			this.nullValue = nullValue;
			this.stringToFormat = stringToFormat;
		}
		
		#region Property's
		public string FormatString {
			get {
				return formatString;
			}
		}
		public string NullValue {
			get {
				return nullValue;
			}
		}
		public string StringToFormat {
			get {
				return stringToFormat;
			}
		}
		
		public string FormatedString {
			get {
				return formatedString;
			}
			set {
				formatedString = value;
			}
		}
		
		
		#endregion
		
	}
}
