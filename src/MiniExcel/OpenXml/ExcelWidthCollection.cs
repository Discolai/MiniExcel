using MiniExcelLibs.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MiniExcelLibs.OpenXml
{
    internal sealed class ExcelWidthCollection
    {
        internal class ExcelColumnInfo
        {
            public int Index { get; set; }
            public double Width { get; set; }
            public long PlaceholderPosition { get; set; }
        }

        private readonly Dictionary<int,ExcelColumnInfo> _columnWidths = new Dictionary<int, ExcelColumnInfo>();
        private readonly double _minWidth;
        private readonly double _maxWidth;

        public IEnumerable<ExcelColumnInfo> Columns => _columnWidths.Values;

        public ExcelWidthCollection(double minWidth, double maxWidth)
        {
            _minWidth = minWidth;
            _maxWidth = maxWidth;
        }

        public void Add(int index, double? initialWidth, long placeholderPosition) 
        {
            _columnWidths.Add(index, new ExcelColumnInfo
            {
                Index = index,
                Width = initialWidth ?? _minWidth,
                PlaceholderPosition = placeholderPosition
            });
        }

        public void AdjustWidth(int columnIndex, string columnValue)
        {
            if (!_columnWidths.TryGetValue(columnIndex, out var currentWidth) )
            {
                return;
            }

            currentWidth.Width = Math.Min(_maxWidth, Math.Max(currentWidth.Width, GetExcelColumnWidth(columnValue)));
        }

        private const double _averageCalibriPt11PixelSize = 6.2;
        private const double _excelWidthFactor = 5.0 / 7.0;
        private const int _padding = 5;

        private static double GetExcelColumnWidth(string text)
        {
            // Calculate approximate width in pixels
            double textWidthInPixels = text.Length * _averageCalibriPt11PixelSize + (_padding * 2);

            // Convert to Excel column width
            double excelColumnWidth = textWidthInPixels * _excelWidthFactor;
            return excelColumnWidth;
        }

    }
}
