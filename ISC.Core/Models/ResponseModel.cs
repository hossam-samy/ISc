﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC.Core.Models
{
	public class ResponseModel<T>
	{
		public bool State { get; set; }
		public T? Entity { get; set; }
		public string Comment { get; set; }
	}
}