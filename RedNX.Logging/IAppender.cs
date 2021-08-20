using System;

namespace RedNX.Logging {
    public interface IAppender : IDisposable {
        
        void Log(ILogger logger, LogLevel logLevel, object msg, params object[] objs);
        void Log(ILogger logger, LogLevel logLevel, object msg, Exception ex, params object[] objs);
        bool IsEnabled(LogLevel logLevel);

    }
}