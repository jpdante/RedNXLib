namespace RedNX.Process {
    public enum State {
        Running,
        Sleeping,
        Waiting,
        Zombie,
        Stopped,
        Tracing,
        Dead,
        Unknown
    }
}