﻿using Azure;
using ISC.Core.Dtos;
using ISC.Core.Interfaces;
using ISC.Core.Models;
using ISC.EF;
using ISC.Services.Helpers;
using ISC.Services.ISerivces;
using ISC.Services.ISerivces.IModelServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC.Services.Services.ModelSerivces
{
	public class CampServices:ICampServices
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<UserAccount> _userManager;
		public CampServices(IUnitOfWork unitOfWork,
			UserManager<UserAccount> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		public async Task<ServiceResponse<List<DisplayCampsDto>>> CampMentors()
		{
			ServiceResponse<List<DisplayCampsDto>> response = new ServiceResponse<List<DisplayCampsDto>>();

			var campMentor = _unitOfWork.Camps.getAllAsync().Result.Select(c => new DisplayCampsDto()
			{
				Id = c.Id,
				Name = c.Name
			}).ToList();

			if (campMentor == null)
			{
				response.Success = false;
				response.Comment = "No camp found";
				return response;
			}

			foreach(var camp in campMentor)
			{
				var mentors = _unitOfWork.Mentors.Get()
					.Include(u => u.Camps)
					.Where(u => u.Camps.Any(m => m.Id == camp.Id))
					.Select(i => i.Id).ToListAsync();

				if (mentors != null && mentors.Result.Count() > 0) 
				{
					camp.Mentors.AddRange(await _userManager.Users
										.Include(u => u.Mentor)
										.Where(u => u.Mentor != null && mentors.Result.Any(j=>j==u.Mentor.Id))
										.Select(u => u.FirstName + ' ' + u.MiddleName + ' ' + u.LastName).ToListAsync());
				}
			}

			response.Success= true;
			response.Entity = campMentor;

			return response;
		}
	}
}
