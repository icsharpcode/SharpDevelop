
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
	/// Base Class for all Formatters
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 27.03.2005 18:09:29
	/// </remarks>
namespace SharpReportCore {	
	public class AbstractFormatter : object {
		
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public AbstractFormatter() {
		}
		
		protected bool CheckFormat (string format) {
			if (String.IsNullOrEmpty(format)) {
				return false;
			} 
			return true;
		}
		
		protected bool CheckValue (string toFormat) {
			if (String.IsNullOrEmpty(toFormat)) {
				return false;
			}
			return true;
		}
	}
}
