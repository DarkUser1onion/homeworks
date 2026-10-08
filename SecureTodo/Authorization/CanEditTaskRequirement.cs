using Microsoft.AspNetCore.Authorization;

namespace SecureTodo.Authorization;

public class CanEditTaskRequirement : IAuthorizationRequirement
{
}