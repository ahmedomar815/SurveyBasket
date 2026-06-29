using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SurveyBasket.Contracts.Roles;
using System.Data;

namespace SurveyBasket.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext Context): IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager= roleManager;
    private readonly ApplicationDbContext _Context= Context;

    public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? includeDisabled = false,CancellationToken cancellationToken=default)
    {
          return await _roleManager.Roles.Where
           (x => !x.IsDefault && (!x.IsDeleted || (includeDisabled == true)))
           .ProjectToType<RoleResponse>()
           .ToListAsync(cancellationToken);
    }
    public async Task<Result<RoleDetailsResponse>> GetRole(string rollId, CancellationToken cancellationToken = default)
    {
        if (await _roleManager.FindByIdAsync(rollId) is not { } role)
            return Result.Failure<RoleDetailsResponse>(RoleErros.RoleNotFound);
        var permission = await _roleManager.GetClaimsAsync(role);
        var response = new RoleDetailsResponse(rollId, role.Name!, role.IsDeleted, permission.Select(x => x.Value));
        return Result.Success<RoleDetailsResponse>(response);
    }
    public async Task<Result<RoleDetailsResponse>>AddRoleAsync(RoleRequest request)
    {
        var roleIsExist = await _roleManager.RoleExistsAsync(request.Name);
        if (roleIsExist) 
            return Result.Failure<RoleDetailsResponse>(RoleErros.DuplicatedNameRole);

        var allowedPermissions = Permissions.GetAllPermissions();
        if(request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErros.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name=request.Name,
            ConcurrencyStamp=Guid.NewGuid().ToString(),
        };
        var result = await _roleManager.CreateAsync(role);
        if(result.Succeeded)
        {
            var permissions = request.Permissions.Select(x => new IdentityRoleClaim<string> { ClaimType = Permissions.Type, ClaimValue = x, RoleId = role.Id });
            await _Context.AddRangeAsync(permissions);
            await _Context.SaveChangesAsync();
            var response = new RoleDetailsResponse(role.Id, role.Name, role.IsDeleted, permissions.Select(x => x.ClaimValue!));
            return Result.Success<RoleDetailsResponse>(response);
        }
        var error = result.Errors.First();
        return Result.Failure<RoleDetailsResponse>(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));

    }

    public async Task<Result>UpdateRoleAsync(string rollId, RoleRequest request)
    {
        if (await _roleManager.FindByIdAsync(rollId) is not { } role)
            return Result.Failure(RoleErros.RoleNotFound);
    var roleIsExist=await _roleManager.Roles.AnyAsync(x=>x.Name== rollId&&x.Id!=rollId);
        if(roleIsExist)
        return Result.Failure(RoleErros.DuplicatedNameRole);
        var allowedPermissions = Permissions.GetAllPermissions();
        if (request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure(RoleErros.InvalidPermissions);
       


        role.Name=request.Name;
        var result = await _roleManager.UpdateAsync(role);
        if(result.Succeeded)
        {
            var currentPermissions = await _Context.RoleClaims.Where
                (x => x.RoleId == rollId && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync();
            var newPermission = request.Permissions.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                { ClaimType = Permissions.Type, ClaimValue = x, RoleId = role.Id });

            var removePermission = currentPermissions.Except(request.Permissions);
            await _Context.RoleClaims
                .Where(x => x.RoleId == rollId && removePermission.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();
            await _Context.AddRangeAsync(newPermission);
            await _Context.SaveChangesAsync();
            return Result.Success();

        }
        var error = result.Errors.First();
        return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result>ToggleStatusAsync(string rollId)
    {
        if (await _roleManager.FindByIdAsync(rollId) is not { } role)
            return Result.Failure(RoleErros.RoleNotFound);

        role.IsDeleted=!role.IsDeleted;  
        await _roleManager.UpdateAsync(role);
        return Result.Success();
    }

}





