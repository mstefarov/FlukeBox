using System;
using System.ComponentModel;
using System.Threading.Tasks;
using FlukeBox.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FlukeBox.Jobs {
    public interface IJob : INotifyPropertyChanged {
        Guid Guid { get; }

        [NotNull]
        string Tag { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        JobState State { get; }

        [CanBeNull]
        ProgressReport Progress { get; }

        DateTime TimeCreated { get; }

        DateTime TimeStarted { get; }

        DateTime TimeFinished { get; }

        [NotNull]
        [JsonIgnore]
        BackgroundAction Action { get; }

        [CanBeNull]
        [JsonIgnore]
        Exception Error { get; }

        [CanBeNull]
        string ErrorMessage { get; }

        bool Cancel();

        [NotNull]
        Task CreateTask();

        void Run();
    }
}
