using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public static class TimeHelpers
    {
        public static string CalculateTimeDifference(DateTime lastTime)
        {
            // Tính sự khác biệt giữa thời gian hiện tại và thời gian cho trước
            TimeSpan timeDifference = DateTime.Now - lastTime;

            // Kiểm tra sự khác biệt và trả về kết quả phù hợp
            if (timeDifference < TimeSpan.FromMinutes(1))
            {
                // Trả về giây nếu sự khác biệt nhỏ hơn 1 phút
                return $"{(int)timeDifference.TotalSeconds} s";
            }
            else if (timeDifference < TimeSpan.FromHours(1))
            {
                // Trả về phút nếu sự khác biệt nhỏ hơn 1 giờ
                return $"{(int)timeDifference.TotalMinutes} p";
            }
            else if (timeDifference < TimeSpan.FromDays(1))
            {
                // Trả về giờ nếu sự khác biệt nhỏ hơn 1 ngày
                return $"{(int)timeDifference.TotalHours} h";
            }
            else
            {
                // Trả về ngày nếu sự khác biệt từ 1 ngày trở lên
                return $"{(int)timeDifference.TotalDays} d";
            }
        }
    }
}
