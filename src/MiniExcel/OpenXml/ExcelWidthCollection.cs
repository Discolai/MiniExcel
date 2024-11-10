using MiniExcelLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniExcelLibs.OpenXml
{
    internal class ExcelColumnWidth
    {
        public int Index { get; set; }
        public double Width { get; set; }

        public static IEnumerable<ExcelColumnWidth> FromProps(IEnumerable<ExcelColumnInfo> props, double? minWidth = null)
        {
            foreach (var p in props)
            {
                if (p == null || (p.ExcelColumnWidth == null && minWidth == null))
                {
                    continue;
                }
                var colIndex = p.ExcelColumnIndex.GetValueOrDefault() + 1;
                yield return new ExcelColumnWidth
                {
                    Index = colIndex,
                    Width = p.ExcelColumnWidth ?? minWidth.Value,
                };
            }
        }
    }

    internal sealed class ExcelWidthCollection
    {
        private readonly Dictionary<int, ExcelColumnWidth> _columnWidths;
        private readonly double _maxWidth;

        public IEnumerable<ExcelColumnWidth> Columns => _columnWidths.Values;

        public ExcelWidthCollection(double minWidth, double maxWidth, IEnumerable<ExcelColumnInfo> props)
        {
            _maxWidth = maxWidth;
            _columnWidths = ExcelColumnWidth.FromProps(props, minWidth).ToDictionary(x => x.Index);
        }

        public void AdjustWidth(int columnIndex, string columnValue)
        {
            if (!_columnWidths.TryGetValue(columnIndex, out var currentWidth) )
            {
                return;
            }

            var adjustedWidth = Math.Max(currentWidth.Width, GetApproximateCalibriWidth(columnValue));
            currentWidth.Width = Math.Min(_maxWidth, adjustedWidth);
        }

        /// <summary>
        /// Get the approximate width of the given text for Calibri 11pt
        /// </summary>
        /// <remarks>
        /// Rounds the result to 2 decimal places.
        /// </remarks>
        private static double GetApproximateCalibriWidth(string text)
        {
            double characterWidthFactor = 1.2;  // Estimated factor for Calibri, 11pt
            double padding = 2;  // Add some padding for extra spacing

            int characterCount = text.Length;
            double excelColumnWidth = (characterCount * characterWidthFactor) + padding;

            return Math.Round(excelColumnWidth, 2);
        }
    }
}
