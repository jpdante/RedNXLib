using RedNX.Logging.Internal;

namespace RedNX.Logging.Config {
    public class FormatterConfig {
        
        public string Format { get; set; } = null;

        public IFormatter GetFormatter() {
            return Format == null ? new Formatter() : new Formatter(Format);
        }
    }
}