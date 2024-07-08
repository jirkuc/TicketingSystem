using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using TicketingSystem.DTO;
using TicketingSystem.Enums;
using TicketingSystem.Migrations;
using TicketingSystem.Models;
using TicketingSystem.ViewModels;

namespace TicketingSystem.Services;
public class TicketService {
    private ApplicationDbContext _dbContext;
    private UserManager<AppUser> _userManager;
    private RoleManager<AppRole> _roleManager;
    public TicketService(ApplicationDbContext dbContext, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager) {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    DateTime activityTime = DateTime.UtcNow;
    public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync() {
        var allTickets = _dbContext.ITS_Tickets;
        var ticketsDtos = new List<TicketDTO>();
        foreach (var ticket in allTickets) {
            ticketsDtos.Add(await ModelToDto(ticket));
        }
        ticketsDtos.OrderByDescending(ticket => ticket.Id);
        return ticketsDtos;
    }

    public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(TicketState ticketState) {
        var allTickets = _dbContext.ITS_Tickets;
        var ticketsDtos = new List<TicketDTO>();
        foreach (var ticket in allTickets) {
            if (ticket.TicketState == ticketState) {
            ticketsDtos.Add(await ModelToDto(ticket));
            }
        }
        ticketsDtos.OrderByDescending(ticket => ticket.Id);
        return ticketsDtos;
    }

    public async Task<TicketDTO> GetTicketAsync(int id) {
        var ticket = await _dbContext.ITS_Tickets
            .FirstOrDefaultAsync(ticket => ticket.Id == id);
        if (ticket == null) { return null; }
        var ticketDTO = await ModelToDto(ticket);
        ticketDTO.TicketActivities = TicketActivities(ticket.Id);

        return ticketDTO;
    }

    public async Task CreateAsync(TicketDTO ticket, string userId) {
        AppUser createByUser = await GetUser(userId);
        AppRole createRole = await GetRole(createByUser.DefaultRoleId);
        //DateTime activityTime = DateTime.UtcNow;
        Ticket newTicket = new Ticket() {
            ProblemDescription = ticket.ProblemDescription,
            CreatedDate = activityTime,
            Priority = ticket.Priority,
            ReportSource = (createRole.NormalizedName.Equals("CUSTOMER") ? ReportSource.CUSTOMER : ticket.ReportSource),
            ReportSourceName = (createRole.NormalizedName.Equals("CUSTOMER") ? ReportSource.CUSTOMER.ToString() : ticket.ReportSource.ToString()),
            TicketState = TicketState.NEW,
            TicketStateName = TicketState.NEW.ToString(),
            CreatedByUser = createByUser,
            CreatedByUserName = createByUser.NormalizedUserName,
            CreatedByRoleId = createRole.Id,
            CreatedByRoleName = createRole.NormalizedName,
            ClosedFlag = false,
            LastModifiedDate = activityTime,
            OutageTime = 0,
            CurrentActivityNumber = 1,
            LastModifiedByUser = createByUser,
            LastModifiedByUserName = createByUser.NormalizedUserName,
            LastModifiedByRoleId = createRole.Id,
            LastModifiedByRoleName = createRole.NormalizedName,
            CustomerName = ticket.CustomerName.ToUpper(),
            LatestStatusComment = ticket.ProblemDescription,
            TicketActivities = new List<TicketActivity>()
        };
        await _dbContext.ITS_Tickets.AddAsync(newTicket);
        TicketActivity newActivity = await AddActivity(TicketActivityEnum.CREATE, activityTime, newTicket, newTicket.LatestStatusComment, TicketState.NONE, InternalFlag.CUSTOMER);

        newTicket.TicketActivities.Add(newActivity);
        await _dbContext.ITS_TicketActivities.AddAsync(newActivity);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<TicketActivity> AddActivity(
        TicketActivityEnum ticketActivity,
        DateTime activityTime,
        Ticket ticketToUpdate,
        string statusComment,
        TicketState oldTicketState,
        InternalFlag internalFlag) {
        return new TicketActivity {
            Ticket = ticketToUpdate,
            ActivityNumber = ticketToUpdate.CurrentActivityNumber,
            ActivityDate = activityTime,
            ActivityName = ticketActivity.ToString(),
            StatusComment = statusComment,
            ActivityUser = ticketToUpdate.LastModifiedByUser,
            ActivityUserName = ticketToUpdate.LastModifiedByUserName,
            ActivityRole = ticketToUpdate.LastModifiedByRole,
            ActivityRoleName = ticketToUpdate.LastModifiedByRoleName,
            CurrentOutageTime = ticketToUpdate.OutageTime,
            OldTicketState = oldTicketState,
            OldTicketStateName = oldTicketState.ToString(),
            NewTicketState = ticketToUpdate.TicketState,
            NewTicketStateName = ticketToUpdate.TicketStateName,
            InternalFlag = ((ticketActivity == TicketActivityEnum.COMMENT) ? internalFlag : InternalFlag.CUSTOMER)
        };
    }

    private async Task<TicketDTO> ModelToDto(Ticket Ticket) {
        TicketDTO ticketDTO = new TicketDTO() {
            Id = Ticket.Id,
            ProblemDescription = Ticket.ProblemDescription,
            CreatedDate = Ticket.CreatedDate,
            Priority = Ticket.Priority,
            ReportSource = Ticket.ReportSource,
            TicketState = Ticket.TicketState,
            CreatedByUserId = Ticket.CreatedByUserId,
            CreatedByUserName = Ticket.CreatedByUserName,
            CreatedByRoleId = Ticket.CreatedByRoleId,
            CreatedByRoleName = Ticket.CreatedByRoleName,
            ClosedFlag = Ticket.ClosedFlag,
            ClosedDate = Ticket.ClosedDate,
            ClosedByUserId = Ticket.ClosedByUserId,
            ClosedByUserName = Ticket.ClosedByUserName,
            ClosedByRoleId = Ticket.ClosedByRoleId,
            ClosedByRoleName = Ticket.ClosedByRoleName,
            LastModifiedDate = Ticket.LastModifiedDate,
            OutageTime = Ticket.OutageTime,
            CurrentActivityNumber = Ticket.CurrentActivityNumber,
            LastModifiedByUserId = Ticket.LastModifiedByUserId,
            LastModifiedByUserName = Ticket.LastModifiedByUserName,
            LastModifiedByRoleId = Ticket.LastModifiedByRoleId,
            LastModifiedByRoleName = Ticket.LastModifiedByRoleName,
            CustomerName = Ticket.CustomerName,
            LatestStatusComment = Ticket.LatestStatusComment,
//            TicketActivities = TicketActivities(Ticket.Id)
        };

        // Calculates actual ticket outage time to consider current time and state
        if (ticketDTO.TicketState == TicketState.NEW || ticketDTO.TicketState == TicketState.WORKING) {
            ticketDTO.OutageTime += await GetMinutesSinceLastChange(ticketDTO.LastModifiedDate);
        }
        return ticketDTO;
    }

    private List<TicketActivity> TicketActivities(int ticketId) {
        var activities = _dbContext.ITS_TicketActivities
            .Where(activity => activity.TicketId == ticketId)
            .OrderByDescending(activity => activity.ActivityNumber)
            .ToList();
        return activities;
    }

    private TicketActivityDTO TicketActivityModelToDto(TicketActivity ticketActivity) {
        return new TicketActivityDTO {
            Id = ticketActivity.Id,
            TicketId = ticketActivity.TicketId,
            ActivityNumber = ticketActivity.ActivityNumber,
            ActivityDate = ticketActivity.ActivityDate,
            ActivityName = ticketActivity.ActivityName,
            StatusComment = ticketActivity.StatusComment,
            ActivityUserId = ticketActivity.ActivityUserId,
            ActivityUserName = ticketActivity.ActivityUserName,
            ActivityRoleId = ticketActivity.ActivityRoleId,
            ActivityRoleName = ticketActivity.ActivityRoleName,
            CurrentOutageTime = ticketActivity.CurrentOutageTime,
            OldTicketState = ticketActivity.OldTicketState,
            NewTicketState = ticketActivity.NewTicketState
        };
    }

    public async Task<AppUser> GetUser(string userId) {
        return await _userManager.Users.FirstOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<AppRole> GetRole(string roleId) {
        return await _roleManager.Roles.FirstOrDefaultAsync(role => role.Id == roleId);
    }

    private async Task<int> GetMinutesSinceLastChange(DateTime lastChangeTime) {
        TimeSpan timeDifference = DateTime.UtcNow - lastChangeTime;
        return (int)timeDifference.TotalMinutes;
    }

    private async Task<Ticket> DtoToModel(TicketDTO TicketDTO) {
        return new Ticket {
            Id = TicketDTO.Id,
            ProblemDescription = TicketDTO.ProblemDescription,
            CreatedDate = TicketDTO.CreatedDate,
            Priority = TicketDTO.Priority,
            ReportSource = TicketDTO.ReportSource,
            TicketState = TicketDTO.TicketState,
            CreatedByUserId = TicketDTO.CreatedByUserId,
            CreatedByRoleId = TicketDTO.CreatedByRoleId,
            ClosedFlag = TicketDTO.ClosedFlag,
            ClosedDate = TicketDTO.ClosedDate,
            ClosedByUserId = TicketDTO.ClosedByUserId,
            ClosedByRoleId = TicketDTO.ClosedByRoleId,
            LastModifiedDate = TicketDTO.LastModifiedDate,
            OutageTime = TicketDTO.OutageTime,
            CurrentActivityNumber = TicketDTO.CurrentActivityNumber,
            LastModifiedByUserId = TicketDTO.LastModifiedByUserId,
            LastModifiedByRoleId = TicketDTO.LastModifiedByRoleId
        };
    }

    public async Task<RoleDropdownViewModel> GetRoleDropdownsData() {
        var rolesDropDownData = new RoleDropdownViewModel() {
            Roles = await _roleManager.Roles
            .Where(role => role.IsActive == true)
            .OrderBy(role => role.NormalizedName)
            .ToListAsync()
        };
        return rolesDropDownData;
    }
    public async Task<AppUserDropdownViewModel> GetUserDropdownsData() {
        var customerRole = await _roleManager.Roles
            .FirstOrDefaultAsync(role => role.NormalizedName == "CUSTOMER");

        var usersDropDownData = new AppUserDropdownViewModel() {
            Users = await _userManager.Users
            .Where(user => user.IsActive == true)
            .Where(user => user.DefaultRoleId == customerRole.Id)
            .OrderBy(user => user.NormalizedUserName)
            .ToListAsync()
        };
        return usersDropDownData;
    }

    internal async Task ModifyAsync(int id, TicketDTO ticketDTO, string? authenticatedUserId, string statusComment) {
        Ticket ticketToUpdate = await TicketModifications(id, authenticatedUserId, statusComment);

        StringBuilder modifyStatusComment = new StringBuilder();
        modifyStatusComment.AppendLine($"\r\n\r\n//// ticket Modifications: ////");
        int changeCount = 0;
        if (!(ticketToUpdate.CustomerName.ToUpper()).Equals(ticketDTO.CustomerName.ToUpper())) {
            modifyStatusComment.AppendLine("// Old Customer Name: //");
            modifyStatusComment.AppendLine(ticketToUpdate.CustomerName);
            modifyStatusComment.AppendLine("// New Customer Name: //");
            modifyStatusComment.AppendLine(ticketDTO.CustomerName);
            ticketToUpdate.CustomerName = ticketDTO.CustomerName.ToUpper();
            changeCount++;
        }
        if (!(ticketToUpdate.ProblemDescription).Equals(ticketDTO.ProblemDescription)) {
            modifyStatusComment.AppendLine("// Old Problem Description: //");
            modifyStatusComment.AppendLine(ticketToUpdate.ProblemDescription);
            modifyStatusComment.AppendLine("// New Problem Description: //");
            modifyStatusComment.AppendLine(ticketDTO.ProblemDescription);
            ticketToUpdate.ProblemDescription = ticketDTO.ProblemDescription;
            changeCount++;
        }
        if (ticketToUpdate.Priority != ticketDTO.Priority) {
            modifyStatusComment.AppendLine("// Old Priority: //");
            modifyStatusComment.AppendLine(ticketToUpdate.Priority.ToString());
            modifyStatusComment.AppendLine("// New Priority: //");
            modifyStatusComment.AppendLine(ticketDTO.Priority.ToString());
            ticketToUpdate.Priority = ticketDTO.Priority;
            changeCount++;
        }
        if (ticketToUpdate.ReportSource != ticketDTO.ReportSource) {
            modifyStatusComment.AppendLine("// Old Report Source: //");
            modifyStatusComment.AppendLine(ticketToUpdate.ReportSource.ToString());
            modifyStatusComment.AppendLine("// New Report Source: //");
            modifyStatusComment.AppendLine(ticketDTO.ReportSource.ToString());
            ticketToUpdate.ReportSource = ticketDTO.ReportSource;
            ticketToUpdate.ReportSourceName = ticketDTO.ReportSource.ToString();
            changeCount++;
        }
        if (changeCount > 0) {
            ticketToUpdate.LatestStatusComment += modifyStatusComment.ToString();
            statusComment += modifyStatusComment.ToString();
        }

        _dbContext.ITS_Tickets.Update(ticketToUpdate);
        await _dbContext.SaveChangesAsync();

        TicketActivity newActivity = await AddActivity(
            TicketActivityEnum.MODIFY,
            activityTime,
            ticketToUpdate,
            statusComment,
            ticketDTO.TicketState,
            InternalFlag.CUSTOMER
            );

        await _dbContext.ITS_TicketActivities.AddAsync(newActivity);
        await _dbContext.SaveChangesAsync();
    }

    internal async Task Comment(
        int id,
        TicketDTO ticketDTO,
        string? authenticatedUserId,
        string statusComment,
        InternalFlag internalComment
        ) {

        Ticket ticketToUpdate = await TicketModifications(
            id,
            authenticatedUserId,
            (internalComment == InternalFlag.INTERNAL) ? ticketDTO.LatestStatusComment : statusComment
            );

        _dbContext.ITS_Tickets.Update(ticketToUpdate);
        await _dbContext.SaveChangesAsync();

        TicketActivity newActivity = await AddActivity(
            TicketActivityEnum.COMMENT,
            activityTime,
            ticketToUpdate,
            statusComment,
            ticketDTO.TicketState,
            internalComment);

        await _dbContext.ITS_TicketActivities.AddAsync(newActivity);
        await _dbContext.SaveChangesAsync();
    }


    internal async Task ModifyState(
        int id,
        TicketDTO ticketDTO,
        string? authenticatedUserId,
        string statusComment,
        InternalFlag internalComment,
        TicketActivityEnum ticketActivityEnumValue
        ) {
        Ticket ticketToUpdate = await TicketModifications(id,
            authenticatedUserId,
            (internalComment == InternalFlag.INTERNAL) ? ticketDTO.LatestStatusComment : statusComment
            );

        switch (ticketActivityEnumValue) {
            case TicketActivityEnum.STARTWORK:
                ticketToUpdate.TicketState = TicketState.WORKING;
                ticketToUpdate.TicketStateName = TicketState.WORKING.ToString();
                break;
            case TicketActivityEnum.CUSTOMERONHOLD:
                ticketToUpdate.TicketState = TicketState.CUSTOMERONHOLD;
                ticketToUpdate.TicketStateName = TicketState.CUSTOMERONHOLD.ToString();
                break;
            case TicketActivityEnum.RESOLVE:
                ticketToUpdate.TicketState = TicketState.RESOLVED;
                ticketToUpdate.TicketStateName = TicketState.RESOLVED.ToString();
                break;
            case TicketActivityEnum.CLOSE:
                ticketToUpdate.TicketState = TicketState.CLOSED;
                ticketToUpdate.TicketStateName = TicketState.CLOSED.ToString();
                TicketClosureModification(ticketToUpdate);
                break;
            case TicketActivityEnum.VOID:
                ticketToUpdate.TicketState = TicketState.VOID;
                ticketToUpdate.TicketStateName = TicketState.VOID.ToString();
                TicketClosureModification(ticketToUpdate);
                break;
            default:

                break;
        }

        _dbContext.ITS_Tickets.Update(ticketToUpdate);
        await _dbContext.SaveChangesAsync();

        TicketActivity newActivity = await AddActivity(
            ticketActivityEnumValue,
            activityTime,
            ticketToUpdate,
            statusComment,
            ticketDTO.TicketState,
            internalComment
            );

        await _dbContext.ITS_TicketActivities.AddAsync(newActivity);
        await _dbContext.SaveChangesAsync();
    }

    private void TicketClosureModification(Ticket ticketToUpdate) {
        ticketToUpdate.ClosedDate = activityTime;
        ticketToUpdate.ClosedByUser = ticketToUpdate.LastModifiedByUser;
        ticketToUpdate.ClosedByUserName = ticketToUpdate.LastModifiedByUserName;
        ticketToUpdate.ClosedByRole = ticketToUpdate.LastModifiedByRole;
        ticketToUpdate.ClosedByRoleName = ticketToUpdate.LastModifiedByRoleName;
        ticketToUpdate.ClosedFlag = true;
    }

    private async Task<Ticket> TicketModifications(int id, string? authenticatedUserId, string statusComment) {
        Ticket ticketToUpdate = await _dbContext.ITS_Tickets.FirstOrDefaultAsync(ticket => ticket.Id == id);
        AppUser modifiedByUser = await GetUser((string)authenticatedUserId);
        AppRole modifiedByRole = await GetRole(modifiedByUser.DefaultRoleId);

        if (ticketToUpdate.TicketState == TicketState.NEW || ticketToUpdate.TicketState == TicketState.WORKING) {
            ticketToUpdate.OutageTime += await GetMinutesSinceLastChange(ticketToUpdate.LastModifiedDate);
        }
        ticketToUpdate.LastModifiedDate = activityTime;
        ticketToUpdate.CurrentActivityNumber++;
        ticketToUpdate.LastModifiedByUser = modifiedByUser;
        ticketToUpdate.LastModifiedByUserName = modifiedByUser.NormalizedUserName;
        ticketToUpdate.LastModifiedByRole = modifiedByRole;
        ticketToUpdate.LastModifiedByRoleName = modifiedByRole.NormalizedName;
        ticketToUpdate.LatestStatusComment = statusComment;

        return ticketToUpdate;
    }
}
