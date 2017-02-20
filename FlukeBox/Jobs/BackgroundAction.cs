using System;
using System.Threading;
using FlukeBox.Annotations;

namespace FlukeBox.Jobs {
    public delegate bool BackgroundAction([NotNull] IProgress<ProgressReport> progress, CancellationToken cancellationToken);
}