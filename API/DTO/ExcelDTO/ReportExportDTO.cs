namespace UnitTests_ExpenseAPI.DTO.ExcelDTO
{
    public class ReportExportDTO
    {
        public bool Success { get; set; }
        public string? FilePath { get; set; }
        public string? ExportStatus {  get; set; }
    }
}
