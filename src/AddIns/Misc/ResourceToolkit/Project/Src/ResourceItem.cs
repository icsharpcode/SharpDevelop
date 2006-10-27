// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Hornung.ResourceToolkit
{
	/// <summary>
	/// Describes a resource item by file name and key.
	/// </summary>
	public class ResourceItem
	{
		readonly string fileName;
		readonly string key;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceItem"/> class.
		/// </summary>
		/// <param name="fileName">The name of the resource file.</param>
		/// <param name="key">The resource key.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="fileName"/> parameter is <c>null</c>.</exception>
		public ResourceItem(string fileName, string key)
		{
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}
			this.fileName = fileName;
			this.key = key;
		}
		
		/// <summary>
		/// Gets the name of the resource file this resource item is contained in.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
		}
		
		/// <summary>
		/// Gets the resource key of this resource item.
		/// </summary>
		public string Key {
			get {
				return key;
			}
		}
	}
}
