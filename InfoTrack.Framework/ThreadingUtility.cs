using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Framework
{
    public class ThreadingUtility : IThreadingUtility
    {
        public ThreadingUtility()
        {
            DegreeOfParallelism = Environment.ProcessorCount;
        }

        public int DegreeOfParallelism { get; set; }

        public void ParallelizeWork(Action[] methodsToRunInParallel)
        {
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = DegreeOfParallelism }, methodsToRunInParallel);
        }

        public void ParallelizeWork<T>(List<T> input, Action<T> codeToExecuteForEachItemInInput)
        {
            Parallel.ForEach(input, new ParallelOptions { MaxDegreeOfParallelism = DegreeOfParallelism }, codeToExecuteForEachItemInInput);
        }
    }
        
}
