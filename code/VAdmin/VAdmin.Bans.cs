using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAdmin
{
	public static partial class VAdminSystem
	{
		private readonly static (string, ulong)[] TIMEFRAME_KEYWORDS = new (string, ulong)[]
		{
			("y", 31536000),
			("w", 604800),
			("d", 86400),
			("h", 3600),
			("m", 60)
		};

		public static bool ParseTimeframe( string timeframe, out ulong seconds )
		{
			seconds = 0;

			if ( string.IsNullOrWhiteSpace( timeframe ) )
			{
				return false;
			}

			foreach ( var pair in TIMEFRAME_KEYWORDS )
			{
				var k = pair.Item1;

				var split = timeframe.Split( k );
				if ( split.Length > 1 )
				{
					if ( split.Length > 2 )
					{
						return false;
					}

					if ( !int.TryParse( split[0], out var value ) )
					{
						return false;
					}

					seconds += (ulong)value * pair.Item2;
					timeframe = split[1];
				}
			}

			if (timeframe.Length > 0)
			{
				if (!int.TryParse(timeframe, out var sec))
				{
					return false;
				}

				seconds += (ulong)sec;
			}

			return true;
		}

		public static string FormatTimeframe(ulong seconds)
		{
			var timeFrame = TimeSpan.FromSeconds( seconds );
			var format = String.Empty;

			var days = Math.Floor( timeFrame.TotalDays );
			if ( days > 0 )
			{
				format += $"{days} Days";
				seconds -= (ulong)(days * 86400);
				timeFrame = TimeSpan.FromSeconds( seconds );
			}

			var hours = Math.Floor( timeFrame.TotalHours );
			if ( hours > 0 )
			{
				format += $"{(format.Length > 0 ? ", " : "")}{hours} Hours";
				seconds -= (ulong)(hours * 3600);
				timeFrame = TimeSpan.FromSeconds( seconds );
			}

			var minutes = Math.Floor( timeFrame.TotalMinutes );
			if ( minutes > 0 )
			{
				format += $"{(format.Length > 0 ? ", " : "")}{minutes} Minutes";
				seconds -= (ulong)(minutes * 60);
				timeFrame = TimeSpan.FromSeconds( seconds );
			}

			var sec = timeFrame.TotalSeconds;
			if ( sec > 0 )
			{
				format += $"{(format.Length > 0 ? ", " : "")}{sec} Seconds";
			}

			return format;
		}
	}
}
