﻿global using Beis.HelpToGrow.Persistence;
global using Beis.HelpToGrow.Persistence.Models;
global using Beis.VendorManagement.Web.Constants;
global using Beis.VendorManagement.Web.Controllers;
global using Beis.VendorManagement.Web.Extensions;
global using Beis.VendorManagement.Web.Filters;
global using Beis.VendorManagement.Web.Models;
global using Beis.VendorManagement.Web.Models.Enums;
global using Beis.VendorManagement.Web.Models.Pricing;
global using Beis.VendorManagement.Web.Services.Interface;
global using Beis.VendorManagement.Web.Validators;
global using FluentAssertions;
global using MediatR;
global using Microsoft.AspNetCore.Authentication.OpenIdConnect;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.AspNetCore.Mvc.ViewFeatures;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Moq;
global using System.Security.Claims;
global using Xunit;