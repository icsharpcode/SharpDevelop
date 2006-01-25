
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
		private string format;
		private string valueToFormat;
		private string nullValue;
		private string formatedValue;
		
		public FormatOutputEventArgs() {
		}
		
		public FormatOutputEventArgs(string valueToFormat,string format, string nullValue )
		{
			this.format = format;
			this.nullValue = nullValue;
			this.valueToFormat = valueToFormat;
		}
		
		#region Property's
		public string Format {
			get {
				return format;
			}
		}
		public string NullValue {
			get {
				return nullValue;
			}
		}
		public string ValueToFormat {
			get {
				return valueToFormat;
			}
		}
		
		public string FormatedValue {
			get {
				return formatedValue;
			}
			set {
				formatedValue = value;
			}
		}
		
		
		#endregion
		
	}
}
