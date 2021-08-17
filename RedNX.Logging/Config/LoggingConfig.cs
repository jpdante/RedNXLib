using RedNX.Logging.Appenders;

namespace RedNX.Logging.Config {
    public class LoggingConfig {
        public bool Enabled { get; set; } = true;
        public AppenderConfig Appender { get; set; } = new AppenderConfig {
            Type = AppenderType.Multi
        };

        public IAppender GetAppender() {
            return Appender == null || !Enabled ? new NullAppender() : Appender.GetAppender();
        }
    }
}