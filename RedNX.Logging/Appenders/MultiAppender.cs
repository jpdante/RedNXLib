using System;
using System.Collections.Generic;

namespace RedNX.Logging.Appenders {
    public class MultiAppender : IAppender {

        private readonly List<IAppender> _appenders;

        public MultiAppender() {
            _appenders = new List<IAppender>();
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, params object[] objs) {
            foreach (var appender in _appenders) {
                try {
                    appender.Log(logger, logLevel, msg, objs);
                } catch {
                    // ignored
                }
            }
        }

        public void Log(ILogger logger, LogLevel logLevel, string msg, Exception ex, params object[] objs) {
            foreach (var appender in _appenders) {
                try {
                    appender.Log(logger, logLevel, msg, ex, objs);
                } catch {
                    // ignored
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void AddAppender(IAppender appender) {
            _appenders.Add(appender);
        }

        public void RemoveAppender(IAppender appender) {
            _appenders.Remove(appender);
        }

        public void ClearLoggers() {
            _appenders.Clear();
        }

        public IReadOnlyList<IAppender> Appenders => _appenders;

        public void Dispose() {
            foreach (var appender in _appenders) {
                appender.Dispose();
            }
            _appenders.Clear();
        }
    }
}