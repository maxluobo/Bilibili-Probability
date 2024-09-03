using System;
using System.Collections.Generic;

namespace RC.Main
{
    public class ReferenceCollectorDataComparer : IComparer<ReferenceCollectorData>
    {
        public int Compare(ReferenceCollectorData x, ReferenceCollectorData y) =>
            string.Compare(y?.Key ?? "", x?.Key ?? "", StringComparison.Ordinal);
    }
}