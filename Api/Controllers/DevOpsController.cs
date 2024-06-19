using Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("v1/devops")]
[AllowAnonymous]
public class DevOpsController(IDevOpsService devOpsService) : ControllerBase
{
    [HttpGet("ping")]
    public async Task<ActionResult> Ping()
        => Ok(await devOpsService.GetIntegridadeDoStatusCheckAsync());
}