﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Const
{
    public static class HttpStatusCode
    {
        public static readonly int Ok = 200;

        public static readonly int BadRequest = 400;

        public static readonly int Unauthorized = 401;

        public static readonly int Forbidden = 403;

        public static readonly int NotFound = 404;

        public static readonly int Conflict = 409;

        public static readonly int InternalServerError = 500;
    }
}
