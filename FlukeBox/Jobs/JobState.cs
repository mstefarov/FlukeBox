namespace FlukeBox.Jobs {
    public enum JobState {
        Created,
        Scheduled,
        Running,
        Done,
        Canceled,
        Crashed
    }
}