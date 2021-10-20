using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.Tweet
{
    public class UserMatchM : ModelBase
    {

		#region 説明文に含まれる文字(カンマ区切りで複数)[DescriptionKeys]プロパティ
		/// <summary>
		/// 説明文に含まれる文字(カンマ区切りで複数)[DescriptionKeys]プロパティ用変数
		/// </summary>
		string _DescriptionKeys = string.Empty;
		/// <summary>
		/// 説明文に含まれる文字(カンマ区切りで複数)[DescriptionKeys]プロパティ
		/// </summary>
		public string DescriptionKeys
		{
			get
			{
				return _DescriptionKeys;
			}
			set
			{
				if (_DescriptionKeys == null || !_DescriptionKeys.Equals(value))
				{
					_DescriptionKeys = value;
					NotifyPropertyChanged("DescriptionKeys");
				}
			}
		}
		#endregion

        #region フォロー率の下限値[MinRatio]プロパティ
        /// <summary>
        /// フォロー率の下限値[MinRatio]プロパティ用変数
        /// </summary>
        double _MinRatio = 98.0;
		/// <summary>
		/// フォロー率の下限値[MinRatio]プロパティ
		/// </summary>
		public double MinRatio
		{
			get
			{
				return _MinRatio;
			}
			set
			{
				if (!_MinRatio.Equals(value))
				{
					_MinRatio = value;
					NotifyPropertyChanged("MinRatio");
				}
			}
		}
		#endregion

		#region フォロー率の上限値[MaxRatio]プロパティ
		/// <summary>
		/// フォロー率の上限値[MaxRatio]プロパティ用変数
		/// </summary>
		double _MaxRatio = 102.0;
		/// <summary>
		/// フォロー率の上限値[MaxRatio]プロパティ
		/// </summary>
		public double MaxRatio
		{
			get
			{
				return _MaxRatio;
			}
			set
			{
				if (!_MaxRatio.Equals(value))
				{
					_MaxRatio = value;
					NotifyPropertyChanged("MaxRatio");
				}
			}
		}
		#endregion

		#region フォロー率のチェック
		/// <summary>
		/// フォロー率のチェック
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <returns>true:フォロー率が範囲内 false:フォロー率が範囲外</returns>
		public bool CheckFollowRatio(TwitterUserM user)
		{
			if (this.MinRatio <= user.FriendshipRatio && user.FriendshipRatio <= this.MaxRatio)
			{
				return true;
			}
			else
            {
				return false;
            }
		}
		#endregion

		#region 説明文の確認
		/// <summary>
		/// 説明文の確認
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <returns>true:含まれる false:含まれない</returns>
		public bool CheckDescription(TwitterUserM user)
		{
			// 説明文に期待するキーワードが含まれるかの確認
			return user.CheckDescription(this.DescriptionKeys.Split(','));
		}
		#endregion

		#region 自分のフォローに含まれているかどうかをチェックする
		/// <summary>
		/// 自分のフォローに含まれているかどうかをチェックする
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <param name="my_follow">自分フォローリスト</param>
		/// <returns>true:含まれている false:含まれていない</returns>
		public bool CheckMyFollow(TwitterUserM user, List<TwitterUserM> my_follow)
		{
			var ret = (from x in my_follow
					   where x.Id.Equals(user.Id)
					   select x).Any();
			return ret;
		}
		#endregion


		#region 説明文でフィルタする
		/// <summary>
		/// 説明文でフィルタする
		/// </summary>
		/// <param name="list">リスト</param>
		/// <returns>フィルタ結果</returns>
		public List<TwitterUserM> FilterdKeys(List<TwitterUserM> list)
		{
			return (from x in list
					where this.CheckDescription(x)
					select x).ToList<TwitterUserM>();
		}
		#endregion
	}
}
