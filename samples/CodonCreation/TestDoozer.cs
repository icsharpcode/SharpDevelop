using ICSharpCode.Core;
using System;
using System.Collections;

namespace CodonCreation
{
	/// <summary>
	/// Class that can build an object out of a Codon in the .addin file. 
	/// </summary>
	/// <remarks>http://en.wikipedia.org/wiki/Fraggle_Rock#Doozers</remarks>
	public class TestDoozer : IDoozer
	{
		public TestDoozer()
		{
		}
		
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the 
		/// condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		// BuildItem:
		// This method actually builds an object which is used by the add-in
		//
		// The BuildItem gets following arguments :
		// a caller object               : this is the object which creates the item
		// a codon                       : the codon read from the .addin file, the doozer
		//                                 uses the information in the codon to build
		//                                 a custom codon object.
		// A arraylist with the subitems : if this codon has subitems in it's path here are the
		//                                 build items stored for these the codon may use these
		//                                 or not, if not they get lost, if any where there.
		//                                 'Normal' codons don't need them (then the tree-path is a list)
		//                                 But for example menuitems use them.
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new TestCodon(codon.Properties["text"]);
		}
	}
}
