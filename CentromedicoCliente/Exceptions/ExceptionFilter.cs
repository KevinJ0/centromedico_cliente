﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Exceptions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var customError = false;
            if (context.Exception is EntityNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }


            switch (context.Exception)
            {

                case InsurancesEmptyException or ServicesEmptyException or 
                BadRequestException or ArgumentOutOfRangeException or ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    customError = true;
                    break;

                case UnauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    customError = true;
                    break;

                    

            }


            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                Exception = context.Exception,
                customError = customError
            });
        }
    }
}
