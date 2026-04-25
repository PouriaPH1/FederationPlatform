using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Enums;
using OfficeOpenXml;

namespace FederationPlatform.Application.Services;

public class ReportingService : IReportingService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        // EPPlus 7.0+ requires license context to be set for Community use
        // This is set at application startup, so no explicit setup needed here
    }

    public async Task<List<ActivityReportDto>> GetActivityReportAsync(ReportFilterDto filter)
    {
        var activities = await _unitOfWork.Activities.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();
        var userProfiles = await _unitOfWork.UserProfiles.GetAllAsync();

        var query = activities.AsEnumerable();

        if (filter.UniversityId.HasValue)
            query = query.Where(a => a.UniversityId == filter.UniversityId.Value);

        if (filter.OrganizationId.HasValue)
            query = query.Where(a => a.OrganizationId == filter.OrganizationId.Value);

        if (filter.StartDate.HasValue)
            query = query.Where(a => a.StartDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(a => a.EndDate <= filter.EndDate.Value);

        if (!string.IsNullOrEmpty(filter.Status))
        {
            if (Enum.TryParse<ActivityStatus>(filter.Status, out var status))
                query = query.Where(a => a.Status == status);
        }

        return query.Select(a => new ActivityReportDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            ActivityType = a.ActivityType.ToString(),
            UniversityName = a.University?.Name ?? "نامشخص",
            RepresentativeName = a.User?.UserProfile != null
                ? $"{a.User.UserProfile.FirstName} {a.User.UserProfile.LastName}".Trim()
                : a.User?.Username ?? "نامشخص",
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt,
            FileCount = a.ActivityFiles?.Count ?? 0
        }).OrderByDescending(a => a.CreatedAt).ToList();
    }

    public async Task<List<UniversityReportDto>> GetUniversityReportAsync(ReportFilterDto filter)
    {
        var universities = await _unitOfWork.Universities.GetAllAsync();
        var activities = await _unitOfWork.Activities.GetAllAsync();

        var report = new List<UniversityReportDto>();

        foreach (var university in universities)
        {
            var univActivities = activities.Where(a => a.UniversityId == university.Id).ToList();

            if (univActivities.Count == 0)
                continue;

            var approved = univActivities.Count(a => a.Status.ToString() == "Approved");
            var pending = univActivities.Count(a => a.Status.ToString() == "Pending");
            var rejected = univActivities.Count(a => a.Status.ToString() == "Rejected");

            var topTypes = univActivities
                .GroupBy(a => a.ActivityType.ToString())
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            report.Add(new UniversityReportDto
            {
                UniversityName = university.Name,
                TotalActivities = univActivities.Count,
                ApprovedActivities = approved,
                PendingActivities = pending,
                RejectedActivities = rejected,
                ApprovalRate = univActivities.Count > 0 ? (approved * 100m) / univActivities.Count : 0,
                TopActivityTypes = topTypes
            });
        }

        return report.OrderByDescending(r => r.TotalActivities).ToList();
    }

    public async Task<List<RepresentativeReportDto>> GetRepresentativeReportAsync(int? universityId = null)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var profiles = await _unitOfWork.UserProfiles.GetAllAsync();
        var activities = await _unitOfWork.Activities.GetAllAsync();

        var representatives = users.Where(u => u.Role == UserRole.Representative).ToList();
        var report = new List<RepresentativeReportDto>();

        foreach (var rep in representatives)
        {
            var profile = profiles.FirstOrDefault(p => p.UserId == rep.Id);
            if (profile == null)
                continue;

            if (universityId.HasValue && profile.UniversityId != universityId.Value)
                continue;

            var repActivities = activities.Where(a => a.UserId == rep.Id).ToList();

            if (repActivities.Count == 0)
                continue;

            var approved = repActivities.Count(a => a.Status.ToString() == "Approved");
            var pending = repActivities.Count(a => a.Status.ToString() == "Pending");

            var activityDtos = repActivities.Select(a => new ActivityReportDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                ActivityType = a.ActivityType.ToString(),
                UniversityName = a.University?.Name ?? "نامشخص",
                RepresentativeName = $"{profile.FirstName} {profile.LastName}".Trim(),
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Status = a.Status.ToString(),
                CreatedAt = a.CreatedAt,
                FileCount = a.ActivityFiles?.Count ?? 0
            }).ToList();

            report.Add(new RepresentativeReportDto
            {
                RepresentativeName = $"{profile.FirstName} {profile.LastName}".Trim(),
                UniversityName = profile.University?.Name ?? "نامشخص",
                TotalActivities = repActivities.Count,
                ApprovedActivities = approved,
                PendingActivities = pending,
                ApprovalRate = repActivities.Count > 0 ? (approved * 100m) / repActivities.Count : 0,
                Activities = activityDtos
            });
        }

        return report.OrderByDescending(r => r.TotalActivities).ToList();
    }

    public Task<byte[]> GenerateActivityExcelReportAsync(List<ActivityReportDto> activities, string reportTitle)
    {
        return Task.FromResult(GenerateActivityExcelReportInternal(activities, reportTitle));
    }

    private byte[] GenerateActivityExcelReportInternal(List<ActivityReportDto> activities, string reportTitle)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("فعالیت‌ها");

            // Set RTL
            worksheet.View.RightToLeft = true;

            // Header
            worksheet.Cells[1, 1].Value = reportTitle;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells["A1:I1"].Merge = true;

            // Column headers
            var headers = new[] { "شناسه", "عنوان", "نوع", "دانشگاه", "نماینده", "تاریخ شروع", "تاریخ پایان", "وضعیت", "تعداد فایل" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[3, i + 1].Value = headers[i];
                worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                worksheet.Cells[3, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            // Data
            int row = 4;
            foreach (var activity in activities)
            {
                worksheet.Cells[row, 1].Value = activity.Id;
                worksheet.Cells[row, 2].Value = activity.Title;
                worksheet.Cells[row, 3].Value = activity.ActivityType;
                worksheet.Cells[row, 4].Value = activity.UniversityName;
                worksheet.Cells[row, 5].Value = activity.RepresentativeName;
                worksheet.Cells[row, 6].Value = activity.StartDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 7].Value = activity.EndDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 8].Value = activity.Status;
                worksheet.Cells[row, 9].Value = activity.FileCount;
                row++;
            }

            // Auto-fit columns
            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 25;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 20;
            worksheet.Column(6).Width = 15;
            worksheet.Column(7).Width = 15;
            worksheet.Column(8).Width = 12;
            worksheet.Column(9).Width = 12;

            return package.GetAsByteArray();
        }
    }

    public Task<byte[]> GenerateUniversityExcelReportAsync(List<UniversityReportDto> universities, string reportTitle)
    {
        return Task.FromResult(GenerateUniversityExcelReportInternal(universities, reportTitle));
    }

    private byte[] GenerateUniversityExcelReportInternal(List<UniversityReportDto> universities, string reportTitle)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("دانشگاه‌ها");
            worksheet.View.RightToLeft = true;

            // Header
            worksheet.Cells[1, 1].Value = reportTitle;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells["A1:G1"].Merge = true;

            // Column headers
            var headers = new[] { "دانشگاه", "کل فعالیت‌ها", "تایید شده", "درانتظار", "رد شده", "نرخ تایید %", "اقسام برتر" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[3, i + 1].Value = headers[i];
                worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                worksheet.Cells[3, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }

            // Data
            int row = 4;
            foreach (var uni in universities)
            {
                worksheet.Cells[row, 1].Value = uni.UniversityName;
                worksheet.Cells[row, 2].Value = uni.TotalActivities;
                worksheet.Cells[row, 3].Value = uni.ApprovedActivities;
                worksheet.Cells[row, 4].Value = uni.PendingActivities;
                worksheet.Cells[row, 5].Value = uni.RejectedActivities;
                worksheet.Cells[row, 6].Value = uni.ApprovalRate.ToString("F2");
                worksheet.Cells[row, 7].Value = string.Join("، ", uni.TopActivityTypes);
                row++;
            }

            // Auto-fit columns
            for (int i = 1; i <= 7; i++)
                worksheet.Column(i).Width = 18;

            return package.GetAsByteArray();
        }
    }

    public Task<byte[]> GenerateRepresentativeExcelReportAsync(List<RepresentativeReportDto> representatives, string reportTitle)
    {
        return Task.FromResult(GenerateRepresentativeExcelReportInternal(representatives, reportTitle));
    }

    private byte[] GenerateRepresentativeExcelReportInternal(List<RepresentativeReportDto> representatives, string reportTitle)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("نمایندگان");
            worksheet.View.RightToLeft = true;

            // Header
            worksheet.Cells[1, 1].Value = reportTitle;
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells["A1:F1"].Merge = true;

            // Column headers
            var headers = new[] { "نام نماینده", "دانشگاه", "کل فعالیت‌ها", "تایید شده", "درانتظار", "نرخ تایید %" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[3, i + 1].Value = headers[i];
                worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                worksheet.Cells[3, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            }

            // Data
            int row = 4;
            foreach (var rep in representatives)
            {
                worksheet.Cells[row, 1].Value = rep.RepresentativeName;
                worksheet.Cells[row, 2].Value = rep.UniversityName;
                worksheet.Cells[row, 3].Value = rep.TotalActivities;
                worksheet.Cells[row, 4].Value = rep.ApprovedActivities;
                worksheet.Cells[row, 5].Value = rep.PendingActivities;
                worksheet.Cells[row, 6].Value = rep.ApprovalRate.ToString("F2");
                row++;
            }

            // Auto-fit columns
            for (int i = 1; i <= 6; i++)
                worksheet.Column(i).Width = 20;

            return package.GetAsByteArray();
        }
    }
}
