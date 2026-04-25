using AutoMapper;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Application.Interfaces;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.Application.Services;

public class WorkshopService : IWorkshopService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WorkshopService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WorkshopDto>> GetAllWorkshopsAsync()
    {
        var workshops = await _unitOfWork.Workshops.GetAllAsync();
        return _mapper.Map<IEnumerable<WorkshopDto>>(workshops);
    }

    public async Task<IEnumerable<WorkshopDto>> GetActiveWorkshopsAsync()
    {
        var workshops = await _unitOfWork.Workshops.GetActiveAsync();
        return _mapper.Map<IEnumerable<WorkshopDto>>(workshops);
    }

    public async Task<IEnumerable<WorkshopDto>> GetUpcomingWorkshopsAsync()
    {
        var workshops = await _unitOfWork.Workshops.GetUpcomingAsync();
        return _mapper.Map<IEnumerable<WorkshopDto>>(workshops);
    }

    public async Task<WorkshopDto?> GetWorkshopByIdAsync(int id)
    {
        var workshop = await _unitOfWork.Workshops.GetByIdAsync(id);
        return workshop == null ? null : _mapper.Map<WorkshopDto>(workshop);
    }

    public async Task<WorkshopDto> CreateWorkshopAsync(int adminUserId, CreateWorkshopDto dto)
    {
        var workshop = _mapper.Map<Workshop>(dto);
        workshop.CreatedBy = adminUserId;
        workshop.IsActive = true;

        await _unitOfWork.Workshops.AddAsync(workshop);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<WorkshopDto>(workshop);
    }

    public async Task<bool> UpdateWorkshopAsync(int id, UpdateWorkshopDto dto)
    {
        var workshop = await _unitOfWork.Workshops.GetByIdAsync(id);
        if (workshop == null) return false;

        _mapper.Map(dto, workshop);
        await _unitOfWork.Workshops.UpdateAsync(workshop);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWorkshopAsync(int id)
    {
        if (!await _unitOfWork.Workshops.ExistsAsync(id)) return false;
        await _unitOfWork.Workshops.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
