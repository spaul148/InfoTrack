using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Framework
{
    public interface IThreadingUtility
    {
        int DegreeOfParallelism { get; set; }
        void ParallelizeWork(Action[] methodsToRunInParallel);
        void ParallelizeWork<T>(List<T> input, Action<T> codeToExecuteForEachItemInInput);
    }
}
