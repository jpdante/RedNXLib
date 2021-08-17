﻿using System;

namespace RedNX.Logging {
    public interface IFormatter {

        public string FormatLog(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs);

    }
}