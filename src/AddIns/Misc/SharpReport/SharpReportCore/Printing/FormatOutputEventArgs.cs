//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Peter Forstmeier (Peter.Forstmeier@t-online.de)


using System;
	/// <summary>
	/// This Delegate is used to format the output from textBased items
	/// </summary>
namespace SharpReportCore {
	public delegate void FormatOutputEventHandler (object sender,FormatOutputEventArgs e);
	
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
			set {
				formatString = value;
			}
		}
		public string NullValue {
			get {
				return nullValue;
			}
			set {
				nullValue = value;
			}
		}
		public string StringToFormat {
			get {
				return stringToFormat;
			}
			set {
				stringToFormat = value;
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
