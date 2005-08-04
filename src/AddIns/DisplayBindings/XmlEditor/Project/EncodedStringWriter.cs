//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
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
// Matthew Ward (mrward@users.sourceforge.net)

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// A string writer that allows you to specify the text encoding to
	/// be used when generating the string.
	/// </summary>
	/// <remarks>
	/// This class is used when generating xml strings using a writer and
	/// the encoding in the xml processing instruction needs to be changed.
	/// The xml encoding string will be the encoding specified in the constructor 
	/// of this class (i.e. UTF-8, UTF-16)</remarks>
	public class EncodedStringWriter : StringWriter
	{
		Encoding encoding = Encoding.UTF8;
		
		/// <summary>
		/// Creates a new string writer that will generate a string with the
		/// specified encoding.  
		/// </summary>
		/// <remarks>The encoding will be used when generating the 
		/// xml encoding header (i.e. UTF-8, UTF-16).</remarks>
		public EncodedStringWriter(Encoding encoding)
		{
			this.encoding = encoding;
		}
		
		/// <summary>
		/// Gets the text encoding that will be used when generating
		/// the string.
		/// </summary>
		public override Encoding Encoding {
			get {
				return encoding;
			}
		}
	}
}
