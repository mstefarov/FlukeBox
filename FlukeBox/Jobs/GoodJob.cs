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

            private readonly object _jobsSyncRoot = new object();
            private readonly Dictionary<Guid, GoodJob> _jobsByGuid = new Dictionary<Guid, GoodJob>();
            private readonly Dictionary<string, GoodJob> _jobsByTag = new Dictionary<string, GoodJob>();
            private readonly Timer _cleanupTimer;

            [UsedImplicitly]
            public Registry() {
                // Clean up old jobs once per hour
                _cleanupTimer = new Timer(unused => CleanUpOldJobs(),
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
                lock (_jobsSyncRoot) {
                    _jobsByTag[tag] = job;
                    _jobsByGuid.Add(job.Guid, job);
                }
                return job;
            }

            public IJob Find(Guid id) {
                GoodJob job;
                lock (_jobsSyncRoot) {
                    _jobsByGuid.TryGetValue(id, out job);
                }
                return job;
            }

            public IJob Find(string tag) {
                if (tag == null) {
                    throw new ArgumentNullException(nameof(tag));
                }
                GoodJob job;
                lock (_jobsSyncRoot) {
                    _jobsByTag.TryGetValue(tag, out job);
                }
                return job;
            }

            public IReadOnlyList<IJob> GetRunningJobs() {
                lock (_jobsSyncRoot) {
                    return _jobsByGuid.Values
                                     .Where(job => job.State == JobState.Running)
                                     .ToArray();
                }
            }

            public IReadOnlyList<IJob> GetRecentJobs(TimeSpan period) {
                DateTime minStartDate = DateTime.UtcNow - period;
                lock (_jobsSyncRoot) {
                    return _jobsByGuid.Values
                                     .Where(job => job.TimeStarted >= minStartDate)
                                     .ToArray();
                }
            }

            public IReadOnlyList<IJob> GetAllJobs() {
                lock (_jobsSyncRoot) {
                    return _jobsByGuid.Values.ToArray();
                }
            }

            private void CleanUpOldJobs() {
                lock (_jobsSyncRoot) {
                    foreach (GoodJob job in GetRecentJobs(MaxJobAge)) {
                        if (job.HasFinished) {
                            _jobsByGuid.Remove(job.Guid);
                            job.Dispose();
                        }
                    }
                    var oldJobsByTag = _jobsByTag.Values.Where(job => !_jobsByGuid.ContainsKey(job.Guid)).ToArray();
                    foreach (GoodJob backgroundJob in oldJobsByTag) {
                        _jobsByTag.Remove(backgroundJob.Tag);
                    }
                }
            }

            public void Dispose() {
                _cleanupTimer?.Dispose();
            }
        }

        public DateTime TimeCreated { get; } = DateTime.UtcNow;
        public DateTime TimeStarted { get; private set; }
        public DateTime TimeFinished { get; private set; }
        public Guid Guid { get; } = Guid.NewGuid();
        public BackgroundAction Action { get; }

        public ProgressReport Progress {
            get { return _progress; }
            private set {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public JobState State {
            get { return _state; }
            private set {
                _state = value;
                OnPropertyChanged();
            }
        }

        public Exception Error {
            get { return _error; }
            private set {
                _error = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage => Error?.Message;

        public string Tag { get; }

        private readonly object _syncRoot = new object();
        private readonly CancellationTokenSource _cancellator = new CancellationTokenSource();
        private ProgressReport _progress;
        private JobState _state = JobState.Created;
        private Exception _error;

        private GoodJob(string tag, BackgroundAction action) {
            Action = action;
            Tag = tag;
        }

        public bool HasStarted => !(State == JobState.Created || State == JobState.Scheduled);
        public bool HasFinished => (State == JobState.Done || State == JobState.Crashed || State == JobState.Canceled);
        public bool IsCancellationRequested => _cancellator.IsCancellationRequested;

        public bool Cancel() {
            if (State == JobState.Done || State == JobState.Crashed) return false;
            _cancellator.Cancel();
            return true;
        }

        void IProgress<ProgressReport>.Report(ProgressReport value) {
            Progress = value;
        }

        public Task CreateTask() {
            lock (_syncRoot) {
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
                if (!_cancellator.IsCancellationRequested && Action.Invoke(this, _cancellator.Token)) {
                    State = JobState.Done;
                } else {
                    State = JobState.Canceled;
                }
            } catch (Exception ex) {
                Error = ex;
                lock (_syncRoot) {
                    State = JobState.Crashed;
                }
            }
            TimeFinished = DateTime.UtcNow;
        }

        public void Dispose() {
            _cancellator?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
