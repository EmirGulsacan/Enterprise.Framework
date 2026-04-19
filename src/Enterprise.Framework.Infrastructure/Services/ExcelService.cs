namespace Enterprise.Framework.Infrastructure.Services;

using ClosedXML.Excel;
using Enterprise.Framework.Application.Common.Interfaces;

public class ExcelService : IExcelService
{
    public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);
        var properties = typeof(T).GetProperties();

        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = properties[i].Name;
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        var dataList = data.ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            for (int j = 0; j < properties.Length; j++)
            {
                var value = properties[j].GetValue(dataList[i]);
                worksheet.Cell(i + 2, j + 1).Value = value?.ToString() ?? string.Empty;
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
