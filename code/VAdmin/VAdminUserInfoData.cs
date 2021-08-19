using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin
{
	public class VAdminUserInfoData
	{
		public ulong SteamId { get; set; }

		public string[] Roles { get; set; }

		public static VAdminUserInfoData FromUser(VAdminUserInfo user)
		{
			return new VAdminUserInfoData()
			{
				SteamId = user.SteamId,
				Roles = user.GetRoles(false),
			};
		}
	}
}
