using Microsoft.WindowsAPICodePack.Dialogs;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class FolderSettingM : ConfigM
	{
		public FolderSettingM()
        {
			this.ConfigDir = "Config";
			this.ConfigFileName = "FoloderSetting.conf";
        }

		#region 引っ越し後のコンフィグファイル情報をセットする関数
		/// <summary>
		/// 引っ越し後のコンフィグファイル情報をセットする関数
		/// </summary>
		public void SetAfterConfig()
		{
			this.ConfigDir = "Config";
			this.ConfigFileName = "FoloderSettingAfter.conf";
		}
		#endregion

		#region リモートPC側のファイル保存ディレクトリ[RemoteDirectory]プロパティ
		/// <summary>
		/// リモートPC側のファイル保存ディレクトリ[RemoteDirectory]プロパティ用変数
		/// </summary>
		string _RemoteDirectory = string.Empty;
		/// <summary>
		/// リモートPC側のファイル保存ディレクトリ[RemoteDirectory]プロパティ
		/// </summary>
		public string RemoteDirectory
		{
			get
			{
				return _RemoteDirectory;
			}
			set
			{
				if (_RemoteDirectory == null || !_RemoteDirectory.Equals(value))
				{
					// 最後の一文字を確認した上でセット
					_RemoteDirectory = CheckLastCharactor(value, "/");
					NotifyPropertyChanged("RemoteDirectory");
				}
			}
		}
		#endregion

		#region ローカルPC側のカレントディレクトリ[LocalDirectory]プロパティ
		/// <summary>
		/// ローカルPC側のカレントディレクトリ[LocalDirectory]プロパティ用変数
		/// </summary>
		string _LocalDirectory = string.Empty;
		/// <summary>
		/// ローカルPC側のカレントディレクトリ[LocalDirectory]プロパティ
		/// </summary>
		public string LocalDirectory
		{
			get
			{
				return _LocalDirectory;
			}
			set
			{
				if (_LocalDirectory == null || !_LocalDirectory.Equals(value))
				{
					// 最後の一文字を確認した上でセット
					_LocalDirectory = CheckLastCharactor(value, @"\");
					NotifyPropertyChanged("LocalDirectory");
				}
			}
		}
		#endregion

		/// <summary>
		/// 最後の文字列を確認して
		/// 指定した文字列でなければ付与して返却する。
		/// 文字数が0の場合は何もしない
		/// </summary>
		/// <param name="text"></param>
		/// <param name="last_caractor"></param>
		/// <returns></returns>
		private static string CheckLastCharactor(string text, string last_caractor)
		{

			if (text.Length > 0)
			{
				// 最後の一文字を切り出して "/"や"\" をチェックする
				var tmp = text.Substring(text.Length - 1).Equals(last_caractor);
				if (!tmp)
				{
					// 無い場合は追加する
					text += last_caractor;
				}
			}

			return text;
		}

		/// <summary>
		/// フォルダ選択ダイアログの表示
		/// </summary>
		public void OpenFileBrowzeDialog()
		{
			using (var cofd = new CommonOpenFileDialog()
			{
				Title = "フォルダを選択してください",
				// フォルダ選択モードにする
				IsFolderPicker = true,
			})
			{
				if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
				{
					this.LocalDirectory = cofd.FileName;
				}
			}
		}

		#region 保存処理
		/// <summary>
		/// 保存処理
		/// </summary>
		public void Save()
		{
			// カレントディレクトリがなければ再帰的に作成する
			DirectoryUtil.CreateCurrentDirectory(ConfigFilePath);

			// ファイルの保存処理
			base.Save<FolderSettingM>(this.ConfigFilePath, this);
		}
		#endregion

		#region ロード処理
		/// <summary>
		/// ロード処理
		/// </summary>
		/// <returns></returns>
		public FolderSettingM Load()
		{
			return base.Load<FolderSettingM>(this.ConfigFilePath);
		}
		#endregion
	}
}
