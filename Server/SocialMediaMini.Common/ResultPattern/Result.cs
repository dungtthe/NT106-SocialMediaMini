using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaMini.Common.ResultPattern
{
    public class Result<T>
    {
        public int HttpStatusCode { get; set; }
        public T Value { get; init; }
        public string Error { get; init; }

        public static Result<T> Success(T value)
        {
            return new Result<T> { HttpStatusCode = SocialMediaMini.Shared.Const.HttpStatusCode.Ok, Value = value };
        }

        public static Result<T> Failure(int httpStatusCode, string error)
        {
            return new Result<T> { HttpStatusCode = httpStatusCode, Error = error };
        }
    }
}
