using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SecureTodo.Models;

namespace SecureTodo.Authorization;

public class CanEditTaskHandler : AuthorizationHandler<CanEditTaskRequirement, TaskItem>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanEditTaskRequirement requirement,
        TaskItem resource)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null && resource.UserId == userId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}