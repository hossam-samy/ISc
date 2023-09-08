﻿using ISC.API.ISerivces;
using ISC.API.Services;
using ISC.Core.Interfaces;
using ISC.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ISC.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize]
	public class PublicController : ControllerBase
	{
		private readonly RoleManager<IdentityRole> _RoleManager;
		private readonly UserManager<UserAccount> _UserManager;
		private readonly IAuthanticationServices _Auth;
		private readonly IOnlineJudgeServices _onlineJudgeServices;
		private readonly IUnitOfWork _UnitOfWork;
		public PublicController(RoleManager<IdentityRole> roleManager, UserManager<UserAccount> userManager, IAuthanticationServices auth, IUnitOfWork unitofwork, IOnlineJudgeServices onlineJudgeServices)
		{
			_RoleManager = roleManager;
			_UserManager = userManager;
			_Auth = auth;
			_UnitOfWork = unitofwork;
			_onlineJudgeServices = onlineJudgeServices;
		}
		[HttpGet]
		public async Task<IActionResult>getcontest(string contestid)
		{
			//return Ok(await _onlineJudgeServices.getContestStandingAsync(contestid,50,true));
			//return Ok(await _onlineJudgeServices.getContestStatus(contestid));
			//return Ok(await _onlineJudgeServices.getUserStatusAsync());
			//return Ok(await new ScheduleSerives(_UnitOfWork, _onlineJudgeServices, _UserManager).updateTraineeSolveCurrentAccessAsync());
			return Ok();
		}
	}
}