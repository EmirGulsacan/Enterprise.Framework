namespace Enterprise.Framework.Application.Common.Interfaces;

public interface IExcelService {

byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1");
}



