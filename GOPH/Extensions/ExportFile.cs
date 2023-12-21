using OfficeOpenXml.Table;
using OfficeOpenXml;

namespace GOPH.Extensions
{
    public static class ExportFile
    {

        public static byte[] ExporttoExcel<T>(List<T> table, string filename)
        {
            using ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
            ws.Cells["A1"].LoadFromCollection(table, true, TableStyles.Light1);
            return pack.GetAsByteArray();
        }
    }
}
