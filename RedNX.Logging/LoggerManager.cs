﻿using System;
using RedNX.Logging.Appenders;
using RedNX.Logging.Internal;

namespace RedNX.Logging {
    public class LoggerManager {

        internal static IAppender DefaultAppender;

        static LoggerManager() {
            DefaultAppender = new NullAppender();
        }

        public static void Init(IAppender appender) {
            DefaultAppender = appender;
        }

        public static void Dispose() {
            DefaultAppender.Dispose();
            DefaultAppender = new NullAppender();
        }

        public static ILogger GetLogger(Type type) {
            return new Logger(type);
        }
    }
}
