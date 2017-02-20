using System;
using System.Collections.Generic;
using FlukeBox.Annotations;

namespace FlukeBox.Jobs {
    public interface IJobRegistry {
        [NotNull]
        IJob Create([NotNull] string tag, [NotNull] BackgroundAction action);

        [CanBeNull]
        IJob Find(Guid id);

        [CanBeNull]
        IJob Find([NotNull] string tag);

        [NotNull]
        IReadOnlyList<IJob> GetAllJobs();

        [NotNull]
        IReadOnlyList<IJob> GetRunningJobs();

        [NotNull]
        IReadOnlyList<IJob> GetRecentJobs(TimeSpan period);
    }
}
