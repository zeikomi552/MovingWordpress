using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class CommandM : ModelBase
    {
		#region コマンド文字列[Command]プロパティ
		/// <summary>
		/// コマンド文字列[Command]プロパティ用変数
		/// </summary>
		string _Command = string.Empty;
		/// <summary>
		/// コマンド文字列[Command]プロパティ
		/// </summary>
		public string Command
		{
			get
			{
				return _Command;
			}
			set
			{
				if (_Command == null || !_Command.Equals(value))
				{
					_Command = value;
					NotifyPropertyChanged("Command");
				}
			}
		}
		#endregion

		#region 結果文字列[Result]プロパティ
		/// <summary>
		/// 結果文字列[Result]プロパティ用変数
		/// </summary>
		string _Result = string.Empty;
		/// <summary>
		/// 結果文字列[Result]プロパティ
		/// </summary>
		public string Result
		{
			get
			{
				return _Result;
			}
			set
			{
				if (_Result == null || !_Result.Equals(value))
				{
					_Result = value;
					NotifyPropertyChanged("Result");
				}
			}
		}
		#endregion


	}
}
