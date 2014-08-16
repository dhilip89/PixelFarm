using System;

namespace MatterHackers.Agg
{
    public struct ScanlineSpan
    {
        public int x;
        public int len;
        public int cover_index;

#if DEBUG
        public override string ToString()
        {
            return "x:" + x + ",len:" + len + ",cover:" + cover_index;
        }
#endif

    }

    public interface IScanlineCache
    {
        void finalize(int y);
        void reset(int min_x, int max_x);
        void ResetSpans();
        int num_spans();
        ScanlineSpan begin();
        ScanlineSpan GetNextScanlineSpan();
        int y();
        byte[] GetCovers();
        void add_cell(int x, int cover);
        void add_span(int x, int len, int cover);
    }
}
