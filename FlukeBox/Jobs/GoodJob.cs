using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FlukeBox.Annotations;

namespace FlukeBox.Jobs {
    /// <summary>
    /// Default implementation of <see cref="IJob"/>
    /// </summary>
    public sealed class GoodJob : IJob, IProgress<ProgressReport>, IDisposable {
        /// <summary>
        /// Default implementation of <see cref="IJobRegistry"/>
        /// </summary>
        public class Registry : IJobRegistry, IDisposable {
            private static readonly TimeSpan MaxJobAge = TimeSpan.FromDays(7);

            private readonly object jobsSyncRoot = new object();
            private readonly Dictionary<Guid, GoodJob> jobsByGuid = new Dictionary<Guid, GoodJob>();
            private readonly Dictionary<string, GoodJob> jobsByTag = new Dictionary<string, GoodJob>();
            private readonly Timer cleanupTimer;

            [UsedImplicitly]
            public Registry() {
                // Clean up old jobs once per hour
                cleanupTimer = new Timer(unused => CleanUpOldJobs(),
                                         null,
                                         TimeSpan.Zero,
                                         TimeSpan.FromHours(1));
            }

            public IJob Create(string tag, BackgroundAction action) {
                if (tag == null) {
                    throw new ArgumentNullException(nameof(tag));
                }
                if (action == null) {
                    throw new ArgumentNullException(nameof(action));
                }
                var job = new GoodJob(tag, action);
                lock (jobsSyncRoot) {
                    jobsByTag[tag] = job;
                    jobsByGuid.Add(job.Guid, job);
                }
                return job;
            }

            public IJob Find(Guid id) {
                GoodJob job;
                lock (jobsSyncRoot) {
                    jobsByGuid.TryGetValue(id, out job);
                }
                return job;
            }

            public IJob Find(string tag) {
                if (tag == null) {
                    throw new ArgumentNullException(nameof(tag));
                }
                GoodJob job;
                lock (jobsSyncRoot) {
                    jobsByTag.TryGetValue(tag, out job);
                }
                return job;
            }

            public IReadOnlyList<IJob> GetRunningJobs() {
                lock (jobsSyncRoot) {
                    return jobsByGuid.Values
                                     .Where(job => job.State == JobState.Running)
                                     .ToArray();
                }
            }

            public IReadOnlyList<IJob> GetRecentJobs(TimeSpan period) {
                DateTime minStartDate = DateTime.UtcNow - period;
                lock (jobsSyncRoot) {
                    return jobsByGuid.Values
                                     .Where(job => job.TimeStarted >= minStartDate)
                                     .ToArray();
                }
            }

            public IReadOnlyList<IJob> GetAllJobs() {
                lock (jobsSyncRoot) {
                    return jobsByGuid.Values.ToArray();
                }
            }

            private void CleanUpOldJobs() {
                lock (jobsSyncRoot) {
                    foreach (GoodJob job in GetRecentJobs(MaxJobAge)) {
                        if (job.HasFinished) {
                            jobsByGuid.Remove(job.Guid);
                            job.Dispose();
                        }
                    }
                    var oldJobsByTag = jobsByTag.Values.Where(job => !jobsByGuid.ContainsKey(job.Guid)).ToArray();
                    foreach (GoodJob backgroundJob in oldJobsByTag) {
                        jobsByTag.Remove(backgroundJob.Tag);
                    }
                }
            }

            public void Dispose() {
                cleanupTimer?.Dispose();
            }
        }

        public DateTime TimeCreated { get; } = DateTime.UtcNow;
        public DateTime TimeStarted { get; private set; }
        public DateTime TimeFinished { get; private set; }
        public Guid Guid { get; } = Guid.NewGuid();
        public BackgroundAction Action { get; }

        public ProgressReport Progress {
            get { return progress; }
            private set {
                progress = value;
                OnPropertyChanged();
            }
        }

        public JobState State {
            get { return state; }
            private set {
                state = value;
                OnPropertyChanged();
            }
        }

        public Exception Error {
            get { return error; }
            private set {
                error = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage => Error?.Message;

        public string Tag { get; }

        private readonly object syncRoot = new object();
        private readonly CancellationTokenSource cancellator = new CancellationTokenSource();
        private ProgressReport progress;
        private JobState state = JobState.Created;
        private Exception error;

        private GoodJob(string tag, BackgroundAction action) {
            Action = action;
            Tag = tag;
        }

        public bool HasStarted => !(State == JobState.Created || State == JobState.Scheduled);
        public bool HasFinished => (State == JobState.Done || State == JobState.Crashed || State == JobState.Canceled);
        public bool IsCancellationRequested => cancellator.IsCancellationRequested;

        public bool Cancel() {
            if (State == JobState.Done || State == JobState.Crashed) return false;
            cancellator.Cancel();
            return true;
        }

        void IProgress<ProgressReport>.Report(ProgressReport value) {
            Progress = value;
        }

        public Task CreateTask() {
            lock (syncRoot) {
                if (State != JobState.Created)
                    throw new InvalidOperationException($"Can't create task: job already {State}");
                State = JobState.Scheduled;
            }
            return new Task(RunImpl);
        }

        public void Run() {
            CreateTask().Start();
        }

        private void RunImpl() {
            try {
                TimeStarted = DateTime.UtcNow;
                State = JobState.Running;
                if (!cancellator.IsCancellationRequested && Action.Invoke(this, cancellator.Token)) {
                    State = JobState.Done;
                } else {
                    State = JobState.Canceled;
                }
            } catch (Exception ex) {
                Error = ex;
                lock (syncRoot) {
                    State = JobState.Crashed;
                }
            }
            TimeFinished = DateTime.UtcNow;
        }

        public void Dispose() {
            cancellator?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
