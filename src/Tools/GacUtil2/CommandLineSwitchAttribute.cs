using System;

namespace RJH.CommandLineHelper
{
	/// <summary>Implements a basic command-line switch by taking the
	/// switching name and the associated description.</summary>
	/// <remark>Only currently is implemented for properties, so all
	/// auto-switching variables should have a get/set method supplied.</remark>
	[AttributeUsage( AttributeTargets.Property )]
	public class CommandLineSwitchAttribute : System.Attribute
	{
		#region Private Variables
		private string m_name = "";
		private string m_description = "";
		#endregion

		#region Public Properties
		/// <summary>Accessor for retrieving the switch-name for an associated
		/// property.</summary>
		public string Name 			{ get { return m_name; } }

		/// <summary>Accessor for retrieving the description for a switch of
		/// an associated property.</summary>
		public string Description	{ get { return m_description; } }

		#endregion

		#region Constructors
		/// <summary>Attribute constructor.</summary>
		public CommandLineSwitchAttribute( string name,
													  string description )
		{
			m_name = name;
			m_description = description;
		}
		#endregion
	}

	/// <summary>
	/// This class implements an alias attribute to work in conjunction
	/// with the <see cref="CommandLineSwitchAttribute">CommandLineSwitchAttribute</see>
	/// attribute.  If the CommandLineSwitchAttribute exists, then this attribute
	/// defines an alias for it.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property )]
	public class CommandLineAliasAttribute : System.Attribute
	{
		#region Private Variables
		protected string m_Alias = "";
		#endregion

		#region Public Properties
		public string Alias 
		{
			get { return m_Alias; }
		}

		#endregion

		#region Constructors
		public CommandLineAliasAttribute( string alias ) 
		{
			m_Alias = alias;
		}
		#endregion
	}

}
