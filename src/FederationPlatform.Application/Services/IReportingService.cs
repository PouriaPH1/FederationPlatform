using FederationPlatform.Application.DTOs;

namespace FederationPlatform.Application.Services;

public interface IReportingService
{
    Task<byte[]> GenerateActivityExcelReportAsync(List<ActivityReportDto> activities, string reportTitle);
    Task<byte[]> GenerateUniversityExcelReportAsync(List<UniversityReportDto> universities, string reportTitle);
    Task<byte[]> GenerateRepresentativeExcelReportAsync(List<RepresentativeReportDto> representatives, string reportTitle);
    Task<List<ActivityReportDto>> GetActivityReportAsync(ReportFilterDto filter);
    Task<List<UniversityReportDto>> GetUniversityReportAsync(ReportFilterDto filter);
    Task<List<RepresentativeReportDto>> GetRepresentativeReportAsync(int? universityId = null);
}
