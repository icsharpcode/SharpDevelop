using System;

namespace CodonCreation
{
	/// <summary>
	/// A custom class that will be created by the TestDoozer.
	/// </summary>
	/// <remarks>
	/// SharpDevelop2 now uses custom Doozers instead of custom codons (as
	/// in SharpDevelop 1.x).
	/// </remarks>
	public class TestCodon
	{
        string text  = String.Empty;
        
        public TestCodon(string text)
        {
        	this.text = text;
        }
        
        public string Text {
        	get {
        		return text;
        	}
        }
	}
}
