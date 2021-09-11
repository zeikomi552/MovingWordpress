using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class CommandListsM : ModelBase
	{
		#region [Commands]プロパティ
		/// <summary>
		/// [Commands]プロパティ用変数
		/// </summary>
		ModelList<CommandM> _Commands = new ModelList<CommandM>();
		/// <summary>
		/// [Commands]プロパティ
		/// </summary>
		public ModelList<CommandM> Commands
		{
			get
			{
				return _Commands;
			}
			set
			{
				if (_Commands == null || !_Commands.Equals(value))
				{
					_Commands = value;
					NotifyPropertyChanged("Commands");
				}
			}
		}
		#endregion


	}
}
