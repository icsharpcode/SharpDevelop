using System;

namespace SharpReportCore
{
	public class DisplayNameAttribute : Attribute
	{
		string name;
		public string Name
		{
			get { return name; }
		}
		public DisplayNameAttribute(string name)
		{
			this.name = name;
		}
	}
}
