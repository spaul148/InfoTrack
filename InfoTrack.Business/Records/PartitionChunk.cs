using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrack.Business.Records
{
    public record PartitionChunk<T>(int Index, T Data);
}
