#region Usings

using System;
using System.ComponentModel;

#endregion

namespace ICSharpCode.DatabaseTools.Addin.DatabaseInfo
{
    /// <summary>
    /// Description of DatabaseInfo.
    /// </summary>
    public class DatabaseInfo : INotifyPropertyChanged
    {
        #region Constructor
        
        public DatabaseInfo()
        {
        }
        
        #endregion
        
        #region IPropertyChanged
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private  void NotifyReportView(string property)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property));                     
			}
		}
		
		#endregion
    }
}
