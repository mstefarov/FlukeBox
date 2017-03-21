namespace FlukeBox.Jobs {
    public class ProgressReport {
        public ProgressReport(long itemsDone, long itemsLeftToDo, string status) {
            ItemsDone = itemsDone;
            ItemsLeftToDo = itemsLeftToDo;
            Status = status;
        }

        public long ItemsDone { get; }
        public long ItemsLeftToDo { get; }
        public string Status { get; }

        public override string ToString() {
            return $"{nameof(ProgressReport)}[{ItemsDone}/{ItemsLeftToDo} - {Status}]";
        }
    }
}